using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Application.DTOs.Auth;

public record RegisterDto(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,
    
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 12, ErrorMessage = "Password must be at least 12 characters long")]
    string Password,
    
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 200 characters")]
    string FullName,
    
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    string Phone,
    
    [StringLength(10)]
    string PreferredLanguage = "ar"
);

public record LoginDto(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,
    
    [Required(ErrorMessage = "Password is required")]
    string Password
);

public record AuthResponseDto(
    int UserId,
    string Email,
    string FullName,
    string Token,
    DateTime ExpiresAt,
    string Role,
    string PreferredLanguage,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);

public record RefreshTokenRequestDto(
    [Required(ErrorMessage = "Refresh token is required")]
    string RefreshToken
);

public record UserProfileDto(
    int Id,
    string Email,
    string FullName,
    string? Phone,
    string? ProfileImageUrl,
    string PreferredLanguage,
    bool EmailConfirmed
);

public record UpdateProfileDto(
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(200, MinimumLength = 2)]
    string FullName,
    
    [Phone(ErrorMessage = "Invalid phone number format")]
    string? Phone,
    
    [Required]
    [StringLength(10)]
    string PreferredLanguage
);

public record ForgotPasswordDto(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email
);

public record ResetPasswordDto(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,
    
    [Required(ErrorMessage = "Token is required")]
    string Token,
    
    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 12, ErrorMessage = "Password must be at least 12 characters long")]
    string NewPassword
);

public record ChangePasswordDto(
    [Required(ErrorMessage = "Current password is required")]
    string CurrentPassword,
    
    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 12, ErrorMessage = "Password must be at least 12 characters long")]
    string NewPassword
);

public record VerifyEmailDto(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,
    
    [Required(ErrorMessage = "Token is required")]
    string Token
);
