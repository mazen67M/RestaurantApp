using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Auth;

namespace RestaurantApp.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<ApiResponse> VerifyEmailAsync(VerifyEmailDto dto);
    Task<ApiResponse<UserProfileDto>> GetProfileAsync(int userId);
    Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto dto);
    Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto dto);
    Task<ApiResponse> LogoutAsync(string token);
}
