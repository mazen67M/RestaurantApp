using System.Net.Http.Json;
using System.Text.Json;

namespace RestaurantApp.Web.Services;

public class LoyaltyApiService
{
    private readonly HttpClient _httpClient;

    public LoyaltyApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<LoyaltyCustomerDto>> GetAllCustomersAsync(int page = 1, int pageSize = 20, string? search = null, string? tier = null)
    {
        try
        {
            var queryParams = new List<string> { $"page={page}", $"pageSize={pageSize}" };
            if (!string.IsNullOrEmpty(search)) queryParams.Add($"search={search}");
            if (!string.IsNullOrEmpty(tier)) queryParams.Add($"tier={tier}");

            var queryString = "?" + string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"/api/loyalty/customers{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (jsonResponse.ValueKind == JsonValueKind.Object &&
                    jsonResponse.TryGetProperty("data", out var dataProperty))
                {
                    if (dataProperty.TryGetProperty("items", out var itemsProperty))
                    {
                        var customers = System.Text.Json.JsonSerializer.Deserialize<List<LoyaltyCustomerDto>>(itemsProperty.GetRawText());
                        return customers ?? new List<LoyaltyCustomerDto>();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching loyalty customers: {ex.Message}");
            return new List<LoyaltyCustomerDto>();
        }

        return new List<LoyaltyCustomerDto>();
    }

    public async Task<LoyaltyPointsDto?> GetUserPointsAsync(string userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/loyalty/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoyaltyPointsDto>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching user points: {ex.Message}");
        }
        return null;
    }

    public async Task<List<LoyaltyTransactionDto>> GetTransactionHistoryAsync(string userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/loyalty/history?userId={userId}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<LoyaltyTransactionDto>>>();
                return result?.Data ?? new List<LoyaltyTransactionDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching transaction history: {ex.Message}");
        }
        return new List<LoyaltyTransactionDto>();
    }

    public async Task<bool> AwardPointsAsync(string userId, int points, string reason)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/loyalty/award", new
            {
                UserId = userId,
                Points = points,
                Description = reason
            });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error awarding points: {ex.Message}");
            return false;
        }
    }

    public async Task<List<LoyaltyTierDto>> GetTiersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/loyalty/tiers");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<LoyaltyTierDto>>>();
                return result?.Data ?? new List<LoyaltyTierDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching tiers: {ex.Message}");
        }
        return new List<LoyaltyTierDto>();
    }
}

public class LoyaltyCustomerDto
{
    public string CustomerId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public int Points { get; set; }
    public int TotalEarned { get; set; }
    public int TotalRedeemed { get; set; }
    public string Tier { get; set; } = "Bronze";
    public decimal BonusMultiplier { get; set; } = 1.0m;
    
    // For backward compatibility
    public string Id => CustomerId;
}

public class LoyaltyPointsDto
{
    public int Id { get; set; }
    public int Points { get; set; }
    public int TotalEarned { get; set; }
    public int TotalRedeemed { get; set; }
    public string Tier { get; set; } = "Bronze";
    public decimal BonusMultiplier { get; set; } = 1.0m;
    public int PointsToNextTier { get; set; }
    public string NextTier { get; set; } = "Silver";
}

public class LoyaltyTransactionDto
{
    public int Id { get; set; }
    public int Points { get; set; }
    public string TransactionType { get; set; } = "";
    public string? Description { get; set; }
    public int? OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LoyaltyTierDto
{
    public string Name { get; set; } = "";
    public int MinPoints { get; set; }
    public int MaxPoints { get; set; }
    public decimal BonusMultiplier { get; set; }
    public string Benefits { get; set; } = "";
}
