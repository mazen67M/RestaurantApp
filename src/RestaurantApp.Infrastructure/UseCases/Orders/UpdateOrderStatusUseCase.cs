using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Order;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Enums;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.UseCases.Orders;

/// <summary>
/// Use case for updating order status
/// Handles status transitions, notifications, and side effects
/// </summary>
public class UpdateOrderStatusUseCase
{
    private readonly ApplicationDbContext _context;
    private readonly ILoyaltyService _loyaltyService;
    private readonly IOrderNotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly ILogger<UpdateOrderStatusUseCase> _logger;

    public UpdateOrderStatusUseCase(
        ApplicationDbContext context,
        ILoyaltyService loyaltyService,
        IOrderNotificationService notificationService,
        IEmailService emailService,
        ILogger<UpdateOrderStatusUseCase> logger)
    {
        _context = context;
        _loyaltyService = loyaltyService;
        _notificationService = notificationService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<ApiResponse> ExecuteAsync(int orderId, OrderStatus newStatus, string? changedBy = null)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return ApiResponse.ErrorResponse("Order not found");
        }

        var previousStatus = order.Status;

        // Only proceed if status actually changed
        if (previousStatus == newStatus)
        {
            return ApiResponse.SuccessResponse("Order status unchanged");
        }

        // Record status change in history
        var statusHistory = new OrderStatusHistory
        {
            OrderId = orderId,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            ChangedBy = changedBy ?? "System",
            Notes = null
        };

        _context.OrderStatusHistories.Add(statusHistory);

        // Update order status
        order.Status = newStatus;

        // Handle status-specific side effects
        await HandleStatusSideEffectsAsync(order, newStatus);

        await _context.SaveChangesAsync();

        // Send notifications (outside transaction)
        await SendStatusNotificationsAsync(order, newStatus);

        return ApiResponse.SuccessResponse("Order status updated");
    }

    private async Task HandleStatusSideEffectsAsync(Order order, OrderStatus newStatus)
    {
        if (newStatus == OrderStatus.Delivered)
        {
            order.ActualDeliveryTime = DateTime.UtcNow;
            order.PaymentStatus = PaymentStatus.Paid;

            // Award loyalty points
            try
            {
                await _loyaltyService.AwardPointsAsync(
                    order.UserId.ToString(),
                    order.Id,
                    order.Total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to award loyalty points for order {OrderId}", order.Id);
            }
        }
    }

    private async Task SendStatusNotificationsAsync(Order order, OrderStatus newStatus)
    {
        // Send real-time notification
        try
        {
            await _notificationService.NotifyStatusUpdate(
                order.Id,
                order.UserId.ToString(),
                newStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send notification for order {OrderId}", order.Id);
        }

        // Send email notification
        try
        {
            var user = await _context.Users.FindAsync(order.UserId);
            if (user?.Email != null)
            {
                await _emailService.SendOrderStatusUpdateAsync(
                    user.Email, order.OrderNumber, newStatus.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send email for order {OrderId}", order.Id);
        }
    }
}
