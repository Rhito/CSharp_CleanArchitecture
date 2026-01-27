using CleanArchitecture.Application.DTOs.Auth;


namespace CleanArchitecture.Application.IService
{
    public interface IIdentityService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);

        Task<AuthResponse> RegisterAsync(RegisterRequest request);

        Task<UserProfileResponse> GetProfileByIdAsync(string userID);
        Task<AuthResponse> RefreshTokenAsync(TokenModel model);

    }
}
