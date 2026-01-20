namespace RestaurantApp.Web.Services;

public class SidebarService
{
    public bool IsSidebarOpen { get; private set; } = false;
    public bool IsDesktopCollapsed { get; private set; } = false;

    public event Action? OnChange;

    public void ToggleMobile()
    {
        IsSidebarOpen = !IsSidebarOpen;
        NotifyStateChanged();
    }

    public void ToggleDesktop()
    {
        IsDesktopCollapsed = !IsDesktopCollapsed;
        NotifyStateChanged();
    }

    public void CloseMobile()
    {
        if (IsSidebarOpen)
        {
            IsSidebarOpen = false;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
