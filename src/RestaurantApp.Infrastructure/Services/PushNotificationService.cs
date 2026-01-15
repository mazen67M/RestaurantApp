using Microsoft.Extensions.Logging;

namespace RestaurantApp.Infrastructure.Services;

public interface IPushNotificationService
{
    Task SendOrderNotificationAsync(int orderId, string userId, string title, string body);
    Task SendToDeviceAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null);
    Task SendToTopicAsync(string topic, string title, string body);
    Task SubscribeToTopicAsync(string deviceToken, string topic);
    Task UnsubscribeFromTopicAsync(string deviceToken, string topic);
}

public class PushNotificationService : IPushNotificationService
{
    private readonly ILogger<PushNotificationService> _logger;

    public PushNotificationService(ILogger<PushNotificationService> logger)
    {
        _logger = logger;
        _logger.LogInformation("Push Notification Service initialized (Demo Mode)");
    }

    public async Task SendOrderNotificationAsync(int orderId, string userId, string title, string body)
    {
        var data = new Dictionary<string, string>
        {
            { "type", "order_update" },
            { "orderId", orderId.ToString() },
            { "userId", userId }
        };

        _logger.LogInformation(
            "📱 [PUSH NOTIFICATION] Order #{OrderId} to User {UserId}: {Title} - {Body}",
            orderId, userId, title, body
        );

        // In production: 
        // 1. Get user's device tokens from database
        // 2. Send to Firebase Cloud Messaging (FCM)
        // 3. Handle success/failure responses
        
        await Task.CompletedTask;
    }

    public async Task SendToDeviceAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null)
    {
        _logger.LogInformation(
            "📱 [PUSH NOTIFICATION] To Device {DeviceToken}: {Title} - {Body}",
            deviceToken?.Substring(0, Math.Min(20, deviceToken?.Length ?? 0)) + "...",
            title,
            body
        );

        if (data != null && data.Any())
        {
            _logger.LogInformation("   Data: {Data}", string.Join(", ", data.Select(kv => $"{kv.Key}={kv.Value}")));
        }

        // TODO: Integrate with Firebase Cloud Messaging
        // var message = new Message
        // {
        //     Token = deviceToken,
        //     Notification = new Notification { Title = title, Body = body },
        //     Data = data
        // };
        // await FirebaseMessaging.DefaultInstance.SendAsync(message);

        await Task.CompletedTask;
    }

    public async Task SendToTopicAsync(string topic, string title, string body)
    {
        _logger.LogInformation(
            "📱 [PUSH NOTIFICATION] To Topic '{Topic}': {Title} - {Body}",
            topic, title, body
        );

        // TODO: Integrate with Firebase Cloud Messaging
        // var message = new Message
        // {
        //     Topic = topic,
        //     Notification = new Notification { Title = title, Body = body }
        // };
        // await FirebaseMessaging.DefaultInstance.SendAsync(message);

        await Task.CompletedTask;
    }

    public async Task SubscribeToTopicAsync(string deviceToken, string topic)
    {
        _logger.LogInformation(
            "📱 [PUSH NOTIFICATION] Subscribing device to topic '{Topic}'",
            topic
        );

        // TODO: Integrate with Firebase Cloud Messaging
        // await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(
        //     new[] { deviceToken }, topic
        // );

        await Task.CompletedTask;
    }

    public async Task UnsubscribeFromTopicAsync(string deviceToken, string topic)
    {
        _logger.LogInformation(
            "📱 [PUSH NOTIFICATION] Unsubscribing device from topic '{Topic}'",
            topic
        );

        // TODO: Integrate with Firebase Cloud Messaging
        // await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(
        //     new[] { deviceToken }, topic
        // );

        await Task.CompletedTask;
    }
}
