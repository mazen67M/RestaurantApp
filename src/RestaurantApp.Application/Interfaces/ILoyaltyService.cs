using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Loyalty;

namespace RestaurantApp.Application.Interfaces;

public interface ILoyaltyService
{
    /// <summary>
    /// Get customer loyalty points
    /// </summary>
    Task<ApiResponse<LoyaltyPointsDto>> GetPointsAsync(string customerId);
    
    /// <summary>
    /// Get transaction history
    /// </summary>
    Task<ApiResponse<List<LoyaltyTransactionDto>>> GetTransactionHistoryAsync(string customerId, int? limit = 20);
    
    /// <summary>
    /// Award points for an order
    /// </summary>
    Task<ApiResponse<int>> AwardPointsAsync(string customerId, int orderId, decimal orderTotal);
    
    /// <summary>
    /// Redeem points for a discount
    /// </summary>
    Task<ApiResponse<RedeemResultDto>> RedeemPointsAsync(string customerId, RedeemPointsDto dto);
    
    /// <summary>
    /// Get loyalty tier information
    /// </summary>
    Task<ApiResponse<List<LoyaltyTierDto>>> GetTiersAsync();
    
    /// <summary>
    /// Add bonus points (admin or promotion)
    /// </summary>
    Task<ApiResponse<int>> AddBonusPointsAsync(string customerId, int points, string description);
    
    /// <summary>
    /// Convert points to discount amount (100 points = 1 AED)
    /// </summary>
    decimal CalculateDiscount(int points);
    
    /// <summary>
    /// Get all customers with loyalty points (admin)
    /// </summary>
    Task<ApiResponse<PagedResponse<LoyaltyCustomerDto>>> GetCustomersAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        string? tier = null);
}
