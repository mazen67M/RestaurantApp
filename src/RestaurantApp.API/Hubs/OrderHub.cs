using Microsoft.AspNetCore.SignalR;
using RestaurantApp.Domain.Enums;

namespace RestaurantApp.API.Hubs;

/// <summary>
/// SignalR Hub for real-time order updates
/// </summary>
public class OrderHub : Hub
{
    private readonly ILogger<OrderHub> _logger;

    public OrderHub(ILogger<OrderHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a group to receive updates for a specific order
    /// </summary>
    public async Task JoinOrderGroup(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderId}");
        _logger.LogInformation("Client {ConnectionId} joined order group: {OrderId}", Context.ConnectionId, orderId);
    }

    /// <summary>
    /// Leave a specific order group
    /// </summary>
    public async Task LeaveOrderGroup(string orderId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order-{orderId}");
        _logger.LogInformation("Client {ConnectionId} left order group: {OrderId}", Context.ConnectionId, orderId);
    }

    /// <summary>
    /// Join the kitchen group for a specific branch to receive new orders
    /// </summary>
    public async Task JoinKitchenGroup(string branchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"kitchen-{branchId}");
        _logger.LogInformation("Client {ConnectionId} joined kitchen group: {BranchId}", Context.ConnectionId, branchId);
    }

    /// <summary>
    /// Leave the kitchen group
    /// </summary>
    public async Task LeaveKitchenGroup(string branchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"kitchen-{branchId}");
        _logger.LogInformation("Client {ConnectionId} left kitchen group: {BranchId}", Context.ConnectionId, branchId);
    }

    /// <summary>
    /// Join the admin group to receive all order notifications
    /// </summary>
    public async Task JoinAdminGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "admin");
        _logger.LogInformation("Client {ConnectionId} joined admin group", Context.ConnectionId);
    }

    /// <summary>
    /// Leave the admin group
    /// </summary>
    public async Task LeaveAdminGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "admin");
        _logger.LogInformation("Client {ConnectionId} left admin group", Context.ConnectionId);
    }

    /// <summary>
    /// Join a customer-specific group to receive their order updates
    /// </summary>
    public async Task JoinCustomerGroup(string customerId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"customer-{customerId}");
        _logger.LogInformation("Client {ConnectionId} joined customer group: {CustomerId}", Context.ConnectionId, customerId);
    }

    /// <summary>
    /// Leave the customer group
    /// </summary>
    public async Task LeaveCustomerGroup(string customerId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"customer-{customerId}");
        _logger.LogInformation("Client {ConnectionId} left customer group: {CustomerId}", Context.ConnectionId, customerId);
    }
}

/// <summary>
/// Extension methods for sending order-related notifications via SignalR
/// </summary>
public static class OrderHubExtensions
{
    /// <summary>
    /// Notify about a new order being placed
    /// </summary>
    public static async Task SendNewOrderNotification(
        this IHubContext<OrderHub> hubContext,
        int branchId,
        OrderNotificationDto order)
    {
        // Notify kitchen staff
        await hubContext.Clients.Group($"kitchen-{branchId}")
            .SendAsync("NewOrder", order);
        
        // Notify admin dashboard
        await hubContext.Clients.Group("admin")
            .SendAsync("NewOrder", order);
    }

    /// <summary>
    /// Notify about an order status update
    /// </summary>
    public static async Task SendOrderStatusUpdate(
        this IHubContext<OrderHub> hubContext,
        int orderId,
        string customerId,
        OrderStatus newStatus,
        string? estimatedTime = null)
    {
        var update = new OrderStatusUpdateDto
        {
            OrderId = orderId,
            Status = newStatus,
            StatusText = GetStatusText(newStatus),
            EstimatedTime = estimatedTime,
            UpdatedAt = DateTime.UtcNow
        };

        // Notify the specific order group (for order tracking page)
        await hubContext.Clients.Group($"order-{orderId}")
            .SendAsync("OrderStatusUpdated", update);

        // Notify the customer
        await hubContext.Clients.Group($"customer-{customerId}")
            .SendAsync("OrderStatusUpdated", update);

        // Notify admin dashboard
        await hubContext.Clients.Group("admin")
            .SendAsync("OrderStatusUpdated", update);
    }

    /// <summary>
    /// Notify when an order is ready for pickup/delivery
    /// </summary>
    public static async Task SendOrderReadyNotification(
        this IHubContext<OrderHub> hubContext,
        int orderId,
        string customerId,
        bool isDelivery)
    {
        var notification = new OrderReadyNotificationDto
        {
            OrderId = orderId,
            Message = isDelivery 
                ? "Your order is out for delivery!" 
                : "Your order is ready for pickup!",
            IsDelivery = isDelivery
        };

        await hubContext.Clients.Group($"customer-{customerId}")
            .SendAsync("OrderReady", notification);
    }

    private static string GetStatusText(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "Order Received",
        OrderStatus.Confirmed => "Order Confirmed",
        OrderStatus.Preparing => "Preparing Your Order",
        OrderStatus.Ready => "Ready for Pickup",
        OrderStatus.OutForDelivery => "Out for Delivery",
        OrderStatus.Delivered => "Delivered",
        OrderStatus.Cancelled => "Order Cancelled",
        OrderStatus.Rejected => "Order Rejected",
        _ => status.ToString()
    };
}

/// <summary>
/// DTO for new order notifications
/// </summary>
public class OrderNotificationDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for order status updates
/// </summary>
public class OrderStatusUpdateDto
{
    public int OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public string? EstimatedTime { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for order ready notifications
/// </summary>
public class OrderReadyNotificationDto
{
    public int OrderId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsDelivery { get; set; }
}
