using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestaurantApp.Application.Interfaces;

namespace RestaurantApp.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailVerificationAsync(string email, string verificationLink)
    {
        // In production, integrate with SendGrid, Mailgun, etc.
        _logger.LogInformation("Sending verification email to {Email} with link: {Link}", email, verificationLink);
        
        // Simulate sending
        await Task.Delay(100);
    }

    public async Task SendPasswordResetAsync(string email, string resetLink)
    {
        _logger.LogInformation("Sending password reset email to {Email} with link: {Link}", email, resetLink);
        await Task.Delay(100);
    }

    public async Task SendOrderConfirmationAsync(string email, string orderNumber, decimal total)
    {
        _logger.LogInformation("Sending order confirmation to {Email} for order {OrderNumber}, total: {Total}", 
            email, orderNumber, total);
        await Task.Delay(100);
    }

    public async Task SendOrderStatusUpdateAsync(string email, string orderNumber, string status)
    {
        _logger.LogInformation("Sending order status update to {Email} for order {OrderNumber}, status: {Status}", 
            email, orderNumber, status);
        await Task.Delay(100);
    }
}
