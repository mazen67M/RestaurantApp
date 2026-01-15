using System.Net.Http.Json;
using System.Text.Json;

namespace RestaurantApp.Web.Services;

public class OrderApiService : BaseApiService
{
    public OrderApiService(HttpClient httpClient, Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authProvider) 
        : base(httpClient, authProvider)
    {
    }

    public async Task<List<OrderDto>> GetOrdersAsync(string? status = null, int? branchId = null, DateTime? date = null)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var query = new List<string>();
            if (!string.IsNullOrEmpty(status)) query.Add($"status={status}");
            if (branchId.HasValue) query.Add($"branchId={branchId}");
            if (date.HasValue) query.Add($"date={date:yyyy-MM-dd}");

            var queryString = query.Any() ? "?" + string.Join("&", query) : "";
            var response = await HttpClient.GetAsync($"/api/admin/orders{queryString}");
            
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                };
                options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<OrderDto>>>(options);
                return result?.Data?.Items ?? new List<OrderDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching orders: {ex.Message}");
        }

        return new List<OrderDto>();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            
            var response = await HttpClient.GetAsync($"/api/orders/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<OrderDto>>(options);
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching order {id}: {ex.Message}");
        }
        return null;
    }

    public async Task<bool> UpdateOrderStatusAsync(int id, string status)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.PutAsJsonAsync($"/api/admin/orders/{id}/status", new { status });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating order status: {ex.Message}");
            return false;
        }
    }
}

public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public string BranchNameEn { get; set; } = "";
    public string BranchNameAr { get; set; } = "";
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = "";
    public string OrderType { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
