namespace RestaurantApp.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string verificationLink);
    Task SendPasswordResetAsync(string email, string resetLink);
    Task SendOrderConfirmationAsync(string email, string orderNumber, decimal total);
    Task SendOrderStatusUpdateAsync(string email, string orderNumber, string status);
}
