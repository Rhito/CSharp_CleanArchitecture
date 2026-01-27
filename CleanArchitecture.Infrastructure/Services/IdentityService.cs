using Azure.Core;
using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.Exceptions;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace CleanArchitecture.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        public IdentityService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext context 
            )
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        // Implement LoginAsync
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            ApplicationUser? user;

            if(request.Identifier.Contains('@'))
            {
                user = await _userManager.FindByEmailAsync(request.Identifier);
            }
            else
            {
                user = await _userManager.FindByNameAsync(request.Identifier);
            }

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new ValidationException("Credentials does not match!.");
            }

            var jwtTokenObject = await GenerateJwtToken(user);
            var jwtTokenString = new JwtSecurityTokenHandler().WriteToken(jwtTokenObject);

            var refreshToken = GenerateRefreshToken();
            refreshToken.UserId = user.Id;


            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                Roles = [.. (await _userManager.GetRolesAsync(user))],
                IsVerified = user.EmailConfirmed,
                JwToken = jwtTokenString,       // Trả về chuỗi String
                RefreshToken = refreshToken.Token // Trả về RefreshToken

            };

        }
        // Implement RegisterAsync
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,

            };

            // Tạo User
            var result = await _userManager.CreateAsync(user, request.Password);

            // Kiểm tra kết quả
            if (!result.Succeeded)
            {
                // Gom các lỗi lại thành 1 chuỗi để ném ra (VD: "Password too short, Email taken")
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new ValidationException(errors);
            }
            // Gắn role vào db
            await _userManager.AddToRoleAsync(user, "User"); 
            
            var jwtTokenObject = await GenerateJwtToken(user);
            var jwtTokenString = new JwtSecurityTokenHandler().WriteToken(jwtTokenObject);

            // Tạo & Lưu Refresh Token (QUAN TRỌNG)
            var refreshToken = GenerateRefreshToken();
            refreshToken.UserId = user.Id;

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Roles = ["User"],
                JwToken = jwtTokenString,
                IsVerified = false,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<UserProfileResponse> GetProfileByIdAsync(string userId)
        {
            ArgumentNullException.ThrowIfNull(userId);
            var userInfo = await _userManager.FindByIdAsync(userId) ?? throw new NotFoundException($"Not found user with Id: {userId}");

            return new UserProfileResponse
            {
                UserName = userInfo.UserName ?? "",
                FullName = userInfo.FullName ?? "",
                Email = userInfo.Email ?? "",
                PhoneNumber = userInfo.PhoneNumber ?? "",
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(TokenModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var principal = GetClaimsPrincipalFromExpiredToken(model.AccessToken) ?? throw new ValidationException("Invalid access token");

            var usename = principal.Identity!.Name;
            var user = await _userManager.FindByNameAsync(usename!) ?? throw new ValidationException("User not found");
            var storedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == model.RefreshToken) ??
                throw new ValidationException("RefereshToken has expired");
            if (storedRefreshToken.IsExprires)
                throw new ValidationException("Refresh token has expried");
            if (storedRefreshToken.IsRevoked)
                throw new ValidationException("Redfresh token has revoked");
            storedRefreshToken.Revoked = DateTime.UtcNow;
            storedRefreshToken.ReasonRevoked = "Replace new token";
            _context.RefreshTokens.Update(storedRefreshToken);

            // creat new a pair token
            var newJwtToken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            newRefreshToken.UserId =  user.Id;

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                Roles = [.. (await _userManager.GetRolesAsync(user))],
                IsVerified = user.EmailConfirmed,
                JwToken = new JwtSecurityTokenHandler().WriteToken(newJwtToken),
                RefreshToken = newRefreshToken.Token
            };
        }

        private ClaimsPrincipal? GetClaimsPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSetting:Key"]!)),
                ValidateLifetime = false,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }

        private async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            // 1. Tạo danh sách các thông tin (Claims) muốn lưu trong Token
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new("uid", user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Name, user.UserName ?? ""),
            };
            foreach ( var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 2. Take key from the config and encryption
            var keyContent = _configuration["JwtSettings:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyContent!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            if (!double.TryParse(_configuration["JwtSettings:DurationInMinutes"], out double durationInMinutes))
            {
                durationInMinutes = 15; // Mặc định
            }
            // 3.Generate token
            var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:Issuer"],   // Ai phát hành?
                    audience: _configuration["JwtSettings:Audience"],// Dành cho ai?
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(durationInMinutes), // thời gian hết hạn
                    signingCredentials: creds
                );
            return token;
        }

        private string GetIpAddress()
        {
            if (_httpContextAccessor.HttpContext?.Request.Headers.ContainsKey("X-Forwarded-For") == true)
                return _httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].ToString() ?? "Unknown";
            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(15),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = GetIpAddress(),
            };
        }
    }
}
