using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.Exceptions;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace CleanArchitecture.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public IdentityService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        // Implement LoginAsync
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            ApplicationUser? user = null;

            if(request.Identifier.Contains("@"))
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

            return new LoginResponse
            {
                Username = user.UserName ?? "",
                FullName = user.FullName,
                Token = GenerateJwtToken(user),
            };
                
        }
        // Implement RegisterAsync
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,

            };

            // 1. Tạo User
            var result = await _userManager.CreateAsync(user, request.Password);

            // 2. Kiểm tra kết quả
            if (!result.Succeeded)
            {
                // Gom các lỗi lại thành 1 chuỗi để ném ra (VD: "Password too short, Email taken")
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new ValidationException(errors);
                // Lưu ý: Sau này ta sẽ tạo Custom Exception để xử lý mượt hơn
            }

            return new RegisterResponse
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Token = GenerateJwtToken(user)
            };
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            // 1. Tạo danh sách các thông tin (Claims) muốn lưu trong Token
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("UserName", user.UserName ?? ""),
            };

            // 2. Take key from the config and encryption
            var keyContent = _configuration["JwtSettings:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyContent!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3.Generate token
            var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:Issuer"],   // Ai phát hành?
                    audience: _configuration["JwtSettings:Audience"],// Dành cho ai?
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:DurationInMinutes"]!)), // thời gian hết hạn
                    signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
