using RestaurantApp.Domain.Enums;

namespace RestaurantApp.Application.Interfaces;

public interface IOrderNotificationService
{
    Task NotifyNewOrder(int orderId, string orderNumber, string customerName, int itemCount, decimal total, OrderType orderType);
    Task NotifyStatusUpdate(int orderId, string userId, OrderStatus newStatus, string? estimatedTime = null);
}
