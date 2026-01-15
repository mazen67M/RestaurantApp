using Microsoft.AspNetCore.SignalR;
using RestaurantApp.API.Hubs;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Enums;

namespace RestaurantApp.API.Services;

public class OrderNotificationService : IOrderNotificationService
{
    private readonly IHubContext<OrderHub> _hubContext;
    private readonly ILogger<OrderNotificationService> _logger;

    public OrderNotificationService(
        IHubContext<OrderHub> hubContext,
        ILogger<OrderNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyNewOrder(int orderId, string orderNumber, string customerName, int itemCount, decimal total, OrderType orderType)
    {
        var notification = new OrderNotificationDto
        {
            OrderId = orderId,
            OrderNumber = orderNumber,
            CustomerName = customerName,
            ItemCount = itemCount,
            Total = total,
            OrderType = orderType.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Sending NewOrder notification for {OrderNumber}", orderNumber);
        
        // Notify admin group
        await _hubContext.Clients.Group("admin").SendAsync("NewOrder", notification);
        
        // Also notify branch kitchen
        // Note: For now we'll just send to admin as the dashboard is the primary focus
    }

    public async Task NotifyStatusUpdate(int orderId, string userId, OrderStatus newStatus, string? estimatedTime = null)
    {
        var update = new OrderStatusUpdateDto
        {
            OrderId = orderId,
            Status = newStatus,
            StatusText = newStatus.ToString(), // Simple mapping for now
            EstimatedTime = estimatedTime,
            UpdatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Sending StatusUpdate notification for Order {OrderId} to User {UserId}", orderId, userId);

        // Notify customer
        await _hubContext.Clients.Group($"customer-{userId}").SendAsync("OrderStatusUpdated", update);
        
        // Notify admin
        await _hubContext.Clients.Group("admin").SendAsync("OrderStatusUpdated", update);
        
        // Notify specific order trackers
        await _hubContext.Clients.Group($"order-{orderId}").SendAsync("OrderStatusUpdated", update);
    }
}
