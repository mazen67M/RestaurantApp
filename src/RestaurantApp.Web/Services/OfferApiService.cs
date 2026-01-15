using System.Net.Http.Json;

namespace RestaurantApp.Web.Services;

public class OfferApiService : BaseApiService
{
    public OfferApiService(HttpClient httpClient, Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authProvider) 
        : base(httpClient, authProvider)
    {
    }

    public async Task<List<OfferDto>> GetOffersAsync()
    {
        try
        {
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            bool isAdmin = authState.User.IsInRole("Admin");

            if (isAdmin)
            {
                await EnsureAuthHeaderAsync();
                var response = await HttpClient.GetAsync("/api/offers");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<OfferDto>>>();
                    return result?.Data?.Items ?? new List<OfferDto>();
                }
            }
            else
            {
                // Mobile app or non-admin user: only get active offers
                var response = await HttpClient.GetAsync("/api/offers/active");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<OfferDto>>>();
                    return result?.Data ?? new List<OfferDto>();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching offers: {ex.Message}");
        }
        return new List<OfferDto>();
    }

    public async Task<bool> CreateOfferAsync(OfferDto offer)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var createRequest = new
            {
                Code = offer.Code?.ToUpper() ?? "",
                NameEn = offer.NameEn,
                NameAr = offer.NameAr,
                DescriptionEn = string.IsNullOrEmpty(offer.DescriptionEn) ? $"{offer.Type} - {offer.Value} off" : offer.DescriptionEn,
                DescriptionAr = string.IsNullOrEmpty(offer.DescriptionAr) ? offer.NameAr : offer.DescriptionAr,
                Type = offer.Type,
                Value = offer.Value,
                MinimumOrderAmount = offer.MinimumOrderAmount,
                MaximumDiscount = (decimal?)null,
                StartDate = offer.StartDate,
                EndDate = offer.EndDate,
                UsageLimit = offer.UsageLimit,
                PerUserLimit = (int?)null,
                IsActive = offer.IsActive,
                BranchId = (int?)null,
                CategoryId = (int?)null,
                MenuItemId = (int?)null
            };
            
            var response = await HttpClient.PostAsJsonAsync("/api/offers", createRequest);
            Console.WriteLine($"Create offer response: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {error}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating offer: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateOfferAsync(int id, OfferDto offer)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var updateRequest = new
            {
                Code = offer.Code?.ToUpper() ?? "",
                NameEn = offer.NameEn,
                NameAr = offer.NameAr,
                DescriptionEn = string.IsNullOrEmpty(offer.DescriptionEn) ? $"{offer.Type} - {offer.Value} off" : offer.DescriptionEn,
                DescriptionAr = string.IsNullOrEmpty(offer.DescriptionAr) ? offer.NameAr : offer.DescriptionAr,
                Type = offer.Type,
                Value = offer.Value,
                MinimumOrderAmount = offer.MinimumOrderAmount,
                MaximumDiscount = (decimal?)null,
                StartDate = offer.StartDate,
                EndDate = offer.EndDate,
                UsageLimit = offer.UsageLimit,
                PerUserLimit = (int?)null,
                IsActive = offer.IsActive,
                BranchId = (int?)null,
                CategoryId = (int?)null,
                MenuItemId = (int?)null
            };
            
            var response = await HttpClient.PutAsJsonAsync($"/api/offers/{id}", updateRequest);
            Console.WriteLine($"Update offer response: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {error}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating offer: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ToggleOfferAsync(int id)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.PatchAsync($"/api/offers/{id}/toggle", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteOfferAsync(int id)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.DeleteAsync($"/api/offers/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

public class OfferDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string NameEn { get; set; } = "";
    public string NameAr { get; set; } = "";
    public string DescriptionEn { get; set; } = "";
    public string DescriptionAr { get; set; } = "";
    
    // UI Binding properties mapped to backend names
    public string Type { get; set; } = "Percentage";
    public string OfferType { get => Type; set => Type = value; }
    
    public decimal Value { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    
    public decimal? MinimumOrderAmount { get; set; }
    public decimal? MinOrderAmount { get => MinimumOrderAmount; set => MinimumOrderAmount = value; }
    
    public bool IsActive { get; set; } = true;
}
