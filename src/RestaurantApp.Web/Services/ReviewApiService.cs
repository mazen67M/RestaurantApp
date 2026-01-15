using System.Net.Http.Json;

namespace RestaurantApp.Web.Services;

public class ReviewApiService
{
    private readonly HttpClient _httpClient;

    public ReviewApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ReviewModerationDto>> GetAllReviewsAsync()
    {
        try
        {
            // Get all reviews - combine pending and approved
            var pendingTask = GetPendingReviewsAsync();
            // For now, return pending reviews as we don't have a "get all" endpoint yet
            return await pendingTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching reviews: {ex.Message}");
            return new List<ReviewModerationDto>();
        }
    }

    public async Task<List<ReviewModerationDto>> GetPendingReviewsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/reviews/pending");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ReviewModerationDto>>>();
                return result?.Data ?? new List<ReviewModerationDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching pending reviews: {ex.Message}");
        }
        return new List<ReviewModerationDto>();
    }

    public async Task<bool> ApproveReviewAsync(int reviewId)
    {
        try
        {
            var response = await _httpClient.PatchAsync($"/api/reviews/{reviewId}/approve", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error approving review: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RejectReviewAsync(int reviewId)
    {
        try
        {
            var response = await _httpClient.PatchAsync($"/api/reviews/{reviewId}/reject", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rejecting review: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ToggleVisibilityAsync(int reviewId)
    {
        try
        {
            var response = await _httpClient.PatchAsync($"/api/reviews/{reviewId}/toggle-visibility", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error toggling visibility: {ex.Message}");
            return false;
        }
    }
}

public class ReviewModerationDto
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemNameEn { get; set; } = "";
    public string MenuItemNameAr { get; set; } = "";
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsApproved { get; set; }
    public bool IsVisible { get; set; }
    public DateTime CreatedAt { get; set; }
}
