using System.Net.Http.Json;

namespace RestaurantApp.Web.Services;

public class MenuApiService : BaseApiService
{
    public MenuApiService(HttpClient httpClient, Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authProvider) 
        : base(httpClient, authProvider)
    {
    }

    public async Task<List<MenuItemDto>> GetMenuItemsAsync(int? categoryId = null)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            // The API endpoint for menu items by category
            var url = categoryId.HasValue 
                ? $"/api/menu/categories/{categoryId}/items" 
                : "/api/menu/items";
            
            // Try getting items - if /items doesn't exist, try the categories endpoint
            var response = await HttpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode && !categoryId.HasValue)
            {
                // Try alternate endpoint - get all categories and their items
                response = await HttpClient.GetAsync("/api/menu/categories");
            }
            
            if (response.IsSuccessStatusCode)
            {
                if (url.Contains("/items") && !categoryId.HasValue)
                {
                    // Handling paginated response for all items
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<MenuItemDto>>>();
                    return result?.Data?.Items ?? new List<MenuItemDto>();
                }
                else
                {
                    // Handling non-paginated category items or list fallback
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<MenuItemDto>>>();
                    return result?.Data ?? new List<MenuItemDto>();
                }
            }
            Console.WriteLine($"Get menu items failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching menu items: {ex.Message}");
        }
        return new List<MenuItemDto>();
    }

    public async Task<MenuItemDto?> GetMenuItemByIdAsync(int id)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.GetAsync($"/api/menu/items/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<MenuItemDto>>();
                return result?.Data;
            }
        }
        catch
        {
            return null;
        }
        return null;
    }

    public async Task<bool> CreateMenuItemAsync(MenuItemDto item)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var createDto = new
            {
                item.CategoryId,
                item.NameAr,
                item.NameEn,
                item.DescriptionAr,
                item.DescriptionEn,
                item.Price,
                item.ImageUrl,
                PreparationTimeMinutes = item.PreparationTimeMinutes > 0 ? item.PreparationTimeMinutes : 15,
                Calories = item.Calories
            };
            
            var response = await HttpClient.PostAsJsonAsync("/api/menu/items", createDto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Console.WriteLine($"Create menu item failed: {response.StatusCode}. Error: {error}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error creating menu item: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateMenuItemAsync(int id, MenuItemDto item)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var updateDto = new
            {
                item.NameAr,
                item.NameEn,
                item.DescriptionAr,
                item.DescriptionEn,
                item.Price,
                item.ImageUrl,
                PreparationTimeMinutes = item.PreparationTimeMinutes > 0 ? item.PreparationTimeMinutes : 15,
                item.IsAvailable,
                IsPopular = item.IsFeatured, // Mapping IsFeatured to IsPopular for the API
                item.Calories
            };
            
            var response = await HttpClient.PutAsJsonAsync($"/api/menu/items/{id}", updateDto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Console.WriteLine($"Update menu item failed: {response.StatusCode}. Error: {error}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error updating menu item: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ToggleAvailabilityAsync(int id)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.PostAsync($"/api/menu/items/{id}/toggle-availability", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteMenuItemAsync(int id)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.DeleteAsync($"/api/menu/items/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

public class MenuItemDto
{
    public int Id { get; set; }
    public string NameEn { get; set; } = "";
    public string NameAr { get; set; } = "";
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsPopular { get; set; }
    public bool IsFeatured { get => IsPopular; set => IsPopular = value; } // UI Binding
    public int PreparationTimeMinutes { get; set; } = 15;
    public int? Calories { get; set; }
}
