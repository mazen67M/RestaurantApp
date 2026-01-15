using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Review;

namespace RestaurantApp.Application.Interfaces;

public interface IReviewService
{
    /// <summary>
    /// Get all approved reviews for a menu item
    /// </summary>
    Task<ApiResponse<List<ReviewDto>>> GetItemReviewsAsync(int menuItemId);
    
    /// <summary>
    /// Get review summary (average rating, counts) for a menu item
    /// </summary>
    Task<ApiResponse<ReviewSummaryDto>> GetItemReviewSummaryAsync(int menuItemId);
    
    /// <summary>
    /// Get all reviews by a customer
    /// </summary>
    Task<ApiResponse<List<ReviewDto>>> GetMyReviewsAsync(string customerId);
    
    /// <summary>
    /// Create a new review
    /// </summary>
    Task<ApiResponse<ReviewDto>> CreateReviewAsync(string customerId, CreateReviewDto dto);
    
    /// <summary>
    /// Update an existing review
    /// </summary>
    Task<ApiResponse<ReviewDto>> UpdateReviewAsync(string customerId, int reviewId, UpdateReviewDto dto);
    
    /// <summary>
    /// Delete a review
    /// </summary>
    Task<ApiResponse> DeleteReviewAsync(string customerId, int reviewId);
    
    /// <summary>
    /// Check if customer can review an order item (has ordered and not yet reviewed)
    /// </summary>
    Task<ApiResponse<bool>> CanReviewItemAsync(string customerId, int orderId, int menuItemId);
    
    // Admin operations
    Task<ApiResponse<PagedResponse<ReviewModerationDto>>> GetAllReviewsAsync(int page = 1, int pageSize = 20, string? status = null);
    Task<ApiResponse<List<ReviewModerationDto>>> GetPendingReviewsAsync();
    Task<ApiResponse> ApproveReviewAsync(int reviewId);
    Task<ApiResponse> RejectReviewAsync(int reviewId);
    Task<ApiResponse> ToggleReviewVisibilityAsync(int reviewId);
}
