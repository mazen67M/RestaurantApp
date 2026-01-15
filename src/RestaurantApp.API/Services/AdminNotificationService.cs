using Microsoft.AspNetCore.SignalR;
using RestaurantApp.API.Hubs;

namespace RestaurantApp.API.Services;

public interface IAdminNotificationService
{
    Task NotifyMenuUpdated(int menuItemId, string action);
    Task NotifyCategoryUpdated(int categoryId, string action);
    Task NotifyOfferUpdated(int offerId, string action);
    Task NotifyPriceChanged(int menuItemId, decimal newPrice);
}

public class AdminNotificationService : IAdminNotificationService
{
    private readonly IHubContext<OrderHub> _hubContext;
    private readonly ILogger<AdminNotificationService> _logger;

    public AdminNotificationService(
        IHubContext<OrderHub> hubContext,
        ILogger<AdminNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyMenuUpdated(int menuItemId, string action)
    {
        _logger.LogInformation("Broadcasting menu update: Item {ItemId}, Action: {Action}", menuItemId, action);
        
        await _hubContext.Clients.All.SendAsync("MenuUpdated", new
        {
            menuItemId,
            action, // "created", "updated", "deleted", "availability_changed"
            timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyCategoryUpdated(int categoryId, string action)
    {
        _logger.LogInformation("Broadcasting category update: Category {CategoryId}, Action: {Action}", categoryId, action);
        
        await _hubContext.Clients.All.SendAsync("CategoryUpdated", new
        {
            categoryId,
            action,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyOfferUpdated(int offerId, string action)
    {
        _logger.LogInformation("Broadcasting offer update: Offer {OfferId}, Action: {Action}", offerId, action);
        
        await _hubContext.Clients.All.SendAsync("OfferUpdated", new
        {
            offerId,
            action,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyPriceChanged(int menuItemId, decimal newPrice)
    {
        _logger.LogInformation("Broadcasting price change: Item {ItemId}, New Price: {Price}", menuItemId, newPrice);
        
        await _hubContext.Clients.All.SendAsync("PriceChanged", new
        {
            menuItemId,
            newPrice,
            timestamp = DateTime.UtcNow
        });
    }
}
