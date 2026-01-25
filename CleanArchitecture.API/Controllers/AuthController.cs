using CleanArchitecture.Application.DTOs.Auth;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Application.Wrappers;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CleanArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;

        // Ijection IIdentityService to using service
        public AuthController(IIdentityService identityService, IConfiguration configuration)
        {
            _identityService = identityService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Application.DTOs.Auth.RegisterRequest request)
        {
            var result = await _identityService.RegisterAsync(request);
            var response = ApiResponse<RegisterResponse>.Success(result);
            return Ok(response);
        }

       
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Application.DTOs.Auth.LoginRequest request)
        {
            var result = await _identityService.LoginAsync(request);
            var response = ApiResponse<LoginResponse>.Success(result);
            return Ok(response);
        }
        [HttpPost("check-token")]
        public IActionResult CheckToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                // 1. Chuyển Secret Key thành mảng bytes
                var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, // Bật lên nếu bạn muốn check cả Issuer (người phát hành)
                    ValidateAudience = false, // Bật lên nếu bạn muốn check cả Audience (người nhận)
                                              // Quan trọng: Set về 0 để token hết hạn là báo lỗi ngay lập tức
                    ClockSkew = TimeSpan.Zero
                };

                // 2. Thực hiện validate. Nếu token sai, hàm này sẽ ném ra lỗi (Exception)
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                // 3. Nếu code chạy đến dòng này nghĩa là Token "sạch"
                // principal chứa toàn bộ thông tin (Claims) của user
                return Ok($"Token hợp lệ! Xin chào user: {principal.Identity?.Name}");
            }
            catch (Exception ex)
            {
                // 4. Bắt lỗi nếu token hết hạn, sai chữ ký, hoặc format sai
                return Unauthorized(new { Message = "Token không hợp lệ", Error = ex.Message });
            }
        }
    }
}
