using System.Net.Http.Json;

namespace RestaurantApp.Web.Services;

public class DeliveryApiService
{
    private readonly HttpClient _httpClient;

    public DeliveryApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DeliveryDto>> GetAllDeliveriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/deliveries");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DeliveryDto>>>();
                return result?.Data ?? new List<DeliveryDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching deliveries: {ex.Message}");
        }
        return new List<DeliveryDto>();
    }

    public async Task<List<DeliveryDto>> GetAvailableDeliveriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/deliveries/available");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DeliveryDto>>>();
                return result?.Data ?? new List<DeliveryDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching available deliveries: {ex.Message}");
        }
        return new List<DeliveryDto>();
    }

    public async Task<DeliveryDto?> GetDeliveryByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/deliveries/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<DeliveryDto>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching delivery {id}: {ex.Message}");
        }
        return null;
    }

    public async Task<bool> CreateDeliveryAsync(object dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/deliveries", dto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating delivery: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateDeliveryAsync(int id, object dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/deliveries/{id}", dto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating delivery: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteDeliveryAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/deliveries/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting delivery: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SetAvailabilityAsync(int id, bool isAvailable)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/deliveries/{id}/availability", isAvailable);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting availability: {ex.Message}");
            return false;
        }
    }

    public async Task<DeliveryStatsDto?> GetDeliveryStatsAsync(int id, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var query = new List<string>();
            if (startDate.HasValue) query.Add($"startDate={startDate:yyyy-MM-dd}");
            if (endDate.HasValue) query.Add($"endDate={endDate:yyyy-MM-dd}");
            var queryString = query.Any() ? "?" + string.Join("&", query) : "";

            var response = await _httpClient.GetAsync($"/api/deliveries/{id}/stats{queryString}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<DeliveryStatsDto>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching delivery stats: {ex.Message}");
        }
        return null;
    }

    public async Task<List<DeliveryStatsDto>> GetAllDeliveryStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var query = new List<string>();
            if (startDate.HasValue) query.Add($"startDate={startDate:yyyy-MM-dd}");
            if (endDate.HasValue) query.Add($"endDate={endDate:yyyy-MM-dd}");
            var queryString = query.Any() ? "?" + string.Join("&", query) : "";

            var response = await _httpClient.GetAsync($"/api/deliveries/stats{queryString}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DeliveryStatsDto>>>();
                return result?.Data ?? new List<DeliveryStatsDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching all delivery stats: {ex.Message}");
        }
        return new List<DeliveryStatsDto>();
    }

    public async Task<bool> AssignDeliveryToOrderAsync(int orderId, int deliveryId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/admin/orders/{orderId}/assign-delivery", new { DeliveryId = deliveryId });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error assigning delivery to order: {ex.Message}");
            return false;
        }
    }

}

public class DeliveryDto
{
    public int Id { get; set; }
    public string NameAr { get; set; } = "";
    public string NameEn { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailable { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class DeliveryStatsDto
{
    public int DeliveryId { get; set; }
    public string DeliveryName { get; set; } = "";
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int OrdersToday { get; set; }
    public decimal RevenueToday { get; set; }
    public int OrdersThisWeek { get; set; }
    public decimal RevenueThisWeek { get; set; }
    public int OrdersThisMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
}
