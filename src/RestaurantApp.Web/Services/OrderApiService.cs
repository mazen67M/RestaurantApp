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

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<OrderDtoRaw>>>(options);
                return result?.Data?.Items?.Select(x => x.ToOrderDto()).ToList() ?? new List<OrderDto>();
            }
            else
            {
                Console.WriteLine($"Error fetching orders: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching orders: {ex.Message}");
        }

        return new List<OrderDto>();
    }

    public async Task<OrderDetailDto?> GetOrderByIdAsync(int id)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            var response = await HttpClient.GetAsync($"/api/admin/orders/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<OrderDetailDto>>(options);
                return result?.Data;
            }
            else
            {
                Console.WriteLine($"Error fetching order {id}: {response.StatusCode}");
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
            
            // Send status as string - ASP.NET Core will convert to enum
            var response = await HttpClient.PutAsJsonAsync($"/api/admin/orders/{id}/status", new { Status = status });
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Update status request: Status={status}");
            Console.WriteLine($"Update status response: {response.StatusCode}, Body: {responseBody}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating order status: {ex.Message}");
            return false;
        }
    }
}

// Uses integers for Status and converts to strings
public class OrderDtoRaw
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public string BranchNameEn { get; set; } = "";
    public string BranchNameAr { get; set; } = "";
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = ""; // Changed from int to string
    public string OrderType { get; set; } = ""; // Changed from int to string
    public DateTime CreatedAt { get; set; }
    public string? DeliveryName { get; set; }
    public List<OrderItemSummaryDto> Items { get; set; } = new();
    
    public OrderDto ToOrderDto() => new()
    {
        Id = Id,
        OrderNumber = OrderNumber,
        CustomerName = CustomerName,
        CustomerPhone = CustomerPhone,
        BranchNameEn = BranchNameEn,
        BranchNameAr = BranchNameAr,
        ItemCount = ItemCount,
        Total = Total,
        Status = Status, // Direct assignment now
        OrderType = OrderType, // Direct assignment now
        CreatedAt = CreatedAt,
        DeliveryName = DeliveryName,
        Items = Items
    };
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
    public string? DeliveryName { get; set; }
    public List<OrderItemSummaryDto> Items { get; set; } = new();
}

public class OrderItemSummaryDto
{
    public string MenuItemNameAr { get; set; } = "";
    public string MenuItemNameEn { get; set; } = "";
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public List<OrderItemAddOnSummaryDto> AddOns { get; set; } = new();
}

public class OrderItemAddOnSummaryDto
{
    public string NameAr { get; set; } = "";
    public string NameEn { get; set; } = "";
}

public class OrderDetailDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public string OrderType { get; set; } = ""; // Changed from int to string
    public string Status { get; set; } = ""; // Changed from int to string
    public string PaymentMethod { get; set; } = ""; // Changed from int to string
    public string PaymentStatus { get; set; } = ""; // Changed from int to string
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string? DeliveryAddressLine { get; set; }
    public string? CustomerNotes { get; set; }
    public DateTime? RequestedDeliveryTime { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? DeliveryName { get; set; }
    public BranchInfo? Branch { get; set; }
    public List<OrderItemInfo> Items { get; set; } = new();
    
    public string StatusText => Status; // Direct return since it's already a string
    public string OrderTypeText => OrderType; // Direct return since it's already a string
}

public class BranchInfo
{
    public int Id { get; set; }
    public string NameAr { get; set; } = "";
    public string NameEn { get; set; } = "";
    public string? Phone { get; set; }
}

public class OrderItemInfo
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemNameAr { get; set; } = "";
    public string MenuItemNameEn { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal AddOnsTotal { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
    public List<OrderAddOnInfo> AddOns { get; set; } = new();
}

public class OrderAddOnInfo
{
    public int Id { get; set; }
    public string NameAr { get; set; } = "";
    public string NameEn { get; set; } = "";
    public decimal Price { get; set; }
}
