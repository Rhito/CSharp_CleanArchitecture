using CleanArchitecture.Application.DTOs.Auth;


namespace CleanArchitecture.Application.IService
{
    public interface IIdentityService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);

        Task<RegisterResponse> RegisterAsync(RegisterRequest request); 
    }
}
