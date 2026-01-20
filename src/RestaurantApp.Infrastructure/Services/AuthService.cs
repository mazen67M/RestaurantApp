using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Auth;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ITokenBlacklistService _blacklistService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IEmailService emailService,
        ITokenBlacklistService blacklistService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _emailService = emailService;
        _blacklistService = blacklistService;
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse("Email already registered");
        }

        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.Phone,
            PreferredLanguage = dto.PreferredLanguage
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse(
                "Registration failed",
                result.Errors.Select(e => e.Description).ToList());
        }

        await _userManager.AddToRoleAsync(user, "Customer");

        // Generate email verification token
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
        
        // Send verification email (in production, this would be a link to the app)
        await _emailService.SendEmailVerificationAsync(user.Email!, encodedToken);

        var authResponse = await GenerateAuthResponse(user);
        return ApiResponse<AuthResponseDto>.SuccessResponse(authResponse, "Registration successful. Please verify your email.");
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        Console.WriteLine($"[AuthService] Login attempt for: {dto.Email}");
        
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            Console.WriteLine("[AuthService] User not found");
            return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");
        }
        
        Console.WriteLine($"[AuthService] User found - IsActive: {user.IsActive}, EmailConfirmed: {user.EmailConfirmed}");
        
        if (!user.IsActive)
        {
            Console.WriteLine("[AuthService] User is not active");
            return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");
        }

        // Check if account is locked out
        if (await _userManager.IsLockedOutAsync(user))
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse("Account locked. Please try again later.");
        }

        // Use CheckPasswordAsync to avoid email confirmation requirement from SignInManager
        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        Console.WriteLine($"[AuthService] Password valid: {passwordValid}");
        
        if (!passwordValid)
        {
            // Increment failed access count for lockout
            await _userManager.AccessFailedAsync(user);
            return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");
        }

        // Reset failed access count on successful login
        await _userManager.ResetAccessFailedCountAsync(user);
        
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var authResponse = await GenerateAuthResponse(user);
        Console.WriteLine("[AuthService] Login successful");
        return ApiResponse<AuthResponseDto>.SuccessResponse(authResponse);
    }

    public async Task<ApiResponse> VerifyEmailAsync(VerifyEmailDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return ApiResponse.ErrorResponse("User not found");
        }

        var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(dto.Token));
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        
        if (!result.Succeeded)
        {
            return ApiResponse.ErrorResponse("Email verification failed", 
                result.Errors.Select(e => e.Description).ToList());
        }

        return ApiResponse.SuccessResponse("Email verified successfully");
    }

    public async Task<ApiResponse<UserProfileDto>> GetProfileAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return ApiResponse<UserProfileDto>.ErrorResponse("User not found");
        }

        var profile = new UserProfileDto(
            user.Id,
            user.Email!,
            user.FullName,
            user.PhoneNumber,
            user.ProfileImageUrl,
            user.PreferredLanguage,
            user.EmailConfirmed
        );

        return ApiResponse<UserProfileDto>.SuccessResponse(profile);
    }

    public async Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return ApiResponse<UserProfileDto>.ErrorResponse("User not found");
        }

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.Phone;
        user.PreferredLanguage = dto.PreferredLanguage;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return ApiResponse<UserProfileDto>.ErrorResponse("Update failed",
                result.Errors.Select(e => e.Description).ToList());
        }

        return await GetProfileAsync(userId);
    }

    public async Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return ApiResponse.ErrorResponse("User not found");
        }

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            return ApiResponse.ErrorResponse("Password change failed",
                result.Errors.Select(e => e.Description).ToList());
        }

        return ApiResponse.SuccessResponse("Password changed successfully");
    }

    public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            // Don't reveal if user exists
            return ApiResponse.SuccessResponse("If the email exists, a reset link has been sent.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
        
        await _emailService.SendPasswordResetAsync(user.Email!, encodedToken);

        return ApiResponse.SuccessResponse("If the email exists, a reset link has been sent.");
    }

    public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return ApiResponse.ErrorResponse("Invalid request");
        }

        var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(dto.Token));
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
        
        if (!result.Succeeded)
        {
            return ApiResponse.ErrorResponse("Password reset failed",
                result.Errors.Select(e => e.Description).ToList());
        }

        return ApiResponse.SuccessResponse("Password reset successfully");
    }

    public async Task<ApiResponse> LogoutAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
            return ApiResponse.ErrorResponse("Token is required");

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expiry = jwtToken.ValidTo - DateTime.UtcNow;

            if (expiry > TimeSpan.Zero)
            {
                await _blacklistService.BlacklistTokenAsync(token, expiry);
            }

            return ApiResponse.SuccessResponse("Logged out successfully");
        }
        catch (Exception)
        {
            return ApiResponse.ErrorResponse("Invalid token");
        }
    }

    private async Task<AuthResponseDto> GenerateAuthResponse(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Customer";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, role),
            new("language", user.PreferredLanguage)
        };

        var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
                     ?? _configuration["Jwt:Key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(7);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto(
            user.Id,
            user.Email!,
            user.FullName,
            tokenString,
            expires,
            role,
            user.PreferredLanguage
        );
    }
}
