using System.Net.Http.Json;
using System.Text.Json;

namespace RestaurantApp.Web.Services;

public class RestaurantApiService : BaseApiService
{
    public RestaurantApiService(HttpClient httpClient, Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authProvider) 
        : base(httpClient, authProvider)
    {
    }

    public async Task<RestaurantDto?> GetRestaurantAsync()
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.GetAsync("/api/restaurant");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (jsonResponse.TryGetProperty("data", out var data))
                {
                    return JsonSerializer.Deserialize<RestaurantDto>(data.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching restaurant: {ex.Message}");
        }
        return null;
    }

    public async Task<bool> UpdateRestaurantAsync(UpdateRestaurantRequest request)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.PutAsJsonAsync("/api/restaurant", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating restaurant: {ex.Message}");
            return false;
        }
    }
}

public class RestaurantDto
{
    public int Id { get; set; }
    public string NameAr { get; set; } = "";
    public string NameEn { get; set; } = "";
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string PrimaryColor { get; set; } = "#f57c00";
    public string SecondaryColor { get; set; } = "#1e293b";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateRestaurantRequest
{
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
}
