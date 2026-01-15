using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using RestaurantApp.Web.Services;

namespace RestaurantApp.Web.Services;

public class NotificationService : IAsyncDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly OrderApiService _orderApi;
    private HubConnection? _hubConnection;
    private bool _isInitialized;

    public int PendingOrdersCount { get; private set; }
    public int NotificationCount { get; private set; }
    public List<string> ActiveToasts { get; } = new();

    public event Action? OnChange;
    public event Action<string>? OnNewNotification;

    public NotificationService(NavigationManager navigationManager, OrderApiService orderApi)
    {
        _navigationManager = navigationManager;
        _orderApi = orderApi;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        try
        {
            // Load initial stats
            var pendingOrders = await _orderApi.GetOrdersAsync(status: "Pending");
            PendingOrdersCount = pendingOrders?.Count ?? 0;
            NotificationCount = PendingOrdersCount;

            // Setup SignalR
            var hubUrl = _navigationManager.ToAbsoluteUri("http://localhost:5009/hubs/orders");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<object>("NewOrder", (order) =>
            {
                PendingOrdersCount++;
                NotificationCount++;
                var message = "A new order has been placed!";
                ActiveToasts.Add(message);
                
                OnNewNotification?.Invoke(message);
                NotifyStateChanged();

                // Auto-remove toast after 5 seconds
                _ = Task.Delay(5000).ContinueWith(_ => RemoveToast(message));
            });

            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync("JoinAdminGroup");

            _isInitialized = true;
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"NotificationService Initialization Error: {ex.Message}");
        }
    }

    public void RemoveToast(string toast)
    {
        if (ActiveToasts.Contains(toast))
        {
            ActiveToasts.Remove(toast);
            NotifyStateChanged();
        }
    }

    public void ClearNotifications()
    {
        NotificationCount = 0;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
