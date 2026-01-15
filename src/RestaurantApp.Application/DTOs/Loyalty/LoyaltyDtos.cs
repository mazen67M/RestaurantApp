using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Application.DTOs.Loyalty;

/// <summary>
/// Customer loyalty points summary
/// </summary>
public record LoyaltyPointsDto(
    int Id,
    int Points,
    int TotalEarned,
    int TotalRedeemed,
    string Tier,
    decimal BonusMultiplier,
    int PointsToNextTier,
    string NextTier
);

/// <summary>
/// Loyalty transaction history item
/// </summary>
public record LoyaltyTransactionDto(
    int Id,
    int Points,
    string TransactionType,
    string? Description,
    int? OrderId,
    DateTime CreatedAt
);

/// <summary>
/// Redeem points request
/// </summary>
public record RedeemPointsDto(
    [property: Required(ErrorMessage = "Points amount is required")]
    [property: Range(1, 100000, ErrorMessage = "Points must be between 1 and 100000")]
    int Points,
    
    [property: Range(1, int.MaxValue, ErrorMessage = "Invalid order ID")]
    int? OrderId
);

/// <summary>
/// Redeem points result
/// </summary>
public record RedeemResultDto(
    bool Success,
    int PointsRedeemed,
    decimal DiscountAmount,
    int RemainingPoints,
    string Message
);

/// <summary>
/// Tier information
/// </summary>
public record LoyaltyTierDto(
    string Name,
    int MinPoints,
    int MaxPoints,
    decimal BonusMultiplier,
    string Benefits
);

/// <summary>
/// Customer with loyalty points (for admin)
/// </summary>
public record LoyaltyCustomerDto(
    string CustomerId,
    string CustomerName,
    string Email,
    int Points,
    int TotalEarned,
    int TotalRedeemed,
    string Tier,
    decimal BonusMultiplier
);