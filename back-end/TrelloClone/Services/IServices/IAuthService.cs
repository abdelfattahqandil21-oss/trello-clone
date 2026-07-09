using TrelloClone.DTOs;

namespace TrelloClone.Services.IServices;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> LogoutAsync(string userId);
    Task<AuthResponse> RefreshTokenAsync(string? refreshToken);
    Task<AuthResponse> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request);
    Task<AuthResponse> VerifyEmailAsync(string email, string token);
    Task<AuthResponse> ResendVerificationAsync(ResendVerificationRequest request);
}
