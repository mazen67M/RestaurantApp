using System.Net.Http.Json;
using System.Text.Json;

namespace RestaurantApp.Web.Services;

public class BranchApiService : BaseApiService
{
    public BranchApiService(HttpClient httpClient, Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authProvider) 
        : base(httpClient, authProvider)
    {
    }

    public async Task<List<BranchDto>> GetBranchesAsync()
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.GetAsync("/api/branches");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<BranchDto>>>();
                return result?.Data ?? new List<BranchDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching branches: {ex.Message}");
        }

        return new List<BranchDto>();
    }

    public async Task<BranchDto?> GetBranchByIdAsync(int id)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.GetAsync($"/api/branches/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<BranchDto>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching branch {id}: {ex.Message}");
        }
        
        return null;
    }

    public async Task<BranchDto?> CreateBranchAsync(CreateBranchRequest request)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.PostAsJsonAsync("/api/branches", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<BranchDto>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating branch: {ex.Message}");
        }
        
        return null;
    }

    public async Task<bool> UpdateBranchAsync(int id, UpdateBranchRequest request)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.PutAsJsonAsync($"/api/branches/{id}", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating branch {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteBranchAsync(int id)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.DeleteAsync($"/api/branches/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting branch {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<List<BranchDto>> GetActiveBranchesAsync()
    {
        var allBranches = await GetBranchesAsync();
        return allBranches.Where(b => b.IsActive && b.AcceptingOrders).ToList();
    }
}

public class BranchDto
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string NameAr { get; set; } = "";
    public string NameEn { get; set; } = "";
    public string AddressAr { get; set; } = "";
    public string AddressEn { get; set; } = "";
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Phone { get; set; }
    public decimal DeliveryRadiusKm { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal FreeDeliveryThreshold { get; set; }
    public int DefaultPreparationTimeMinutes { get; set; }
    public string OpeningTime { get; set; } = "";
    public string ClosingTime { get; set; } = "";
    public bool IsActive { get; set; }
    public bool AcceptingOrders { get; set; }
    public double? DistanceKm { get; set; }

    // Helper properties for display
    public string OpeningHours => $"{OpeningTime} - {ClosingTime}";
    public string Address => AddressEn; // Default to English
}

public class CreateBranchRequest
{
    public string NameAr { get; set; } = "";
    public string NameEn { get; set; } = "";
    public string AddressAr { get; set; } = "";
    public string AddressEn { get; set; } = "";
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Phone { get; set; }
    public decimal DeliveryRadiusKm { get; set; } = 10;
    public decimal MinOrderAmount { get; set; } = 0;
    public decimal DeliveryFee { get; set; } = 0;
    public decimal FreeDeliveryThreshold { get; set; } = 0;
    public int DefaultPreparationTimeMinutes { get; set; } = 30;
    public string OpeningTime { get; set; } = "09:00";
    public string ClosingTime { get; set; } = "23:00";
    public bool IsActive { get; set; } = true;
    public bool AcceptingOrders { get; set; } = true;
}

public class UpdateBranchRequest
{
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? AddressAr { get; set; }
    public string? AddressEn { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Phone { get; set; }
    public decimal? DeliveryRadiusKm { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal? DeliveryFee { get; set; }
    public decimal? FreeDeliveryThreshold { get; set; }
    public int? DefaultPreparationTimeMinutes { get; set; }
    public string? OpeningTime { get; set; }
    public string? ClosingTime { get; set; }
    public bool? IsActive { get; set; }
    public bool? AcceptingOrders { get; set; }
}

