namespace RestaurantApp.Domain.Entities;

/// <summary>
/// Represents a promotional offer or coupon
/// </summary>
public class Offer : BaseEntity
{
    /// <summary>
    /// Unique coupon code for the offer
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Arabic name of the offer
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// English name of the offer
    /// </summary>
    public string NameEn { get; set; } = string.Empty;

    /// <summary>
    /// Arabic description of the offer
    /// </summary>
    public string? DescriptionAr { get; set; }

    /// <summary>
    /// English description of the offer
    /// </summary>
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// Type of discount (Percentage, FixedAmount, FreeDelivery, etc.)
    /// </summary>
    public OfferType Type { get; set; }

    /// <summary>
    /// Discount value (percentage or fixed amount depending on Type)
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Minimum order amount required to use this offer
    /// </summary>
    public decimal? MinimumOrderAmount { get; set; }

    /// <summary>
    /// Maximum discount amount (for percentage discounts)
    /// </summary>
    public decimal? MaximumDiscount { get; set; }

    /// <summary>
    /// Start date when the offer becomes active
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date when the offer expires
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Maximum number of times this offer can be used (null = unlimited)
    /// </summary>
    public int? UsageLimit { get; set; }

    /// <summary>
    /// Number of times this offer has been used
    /// </summary>
    public int UsageCount { get; set; } = 0;

    /// <summary>
    /// Maximum times a single user can use this offer (null = unlimited)
    /// </summary>
    public int? PerUserLimit { get; set; }

    /// <summary>
    /// Whether the offer is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional: Specific branch ID this offer applies to (null = all branches)
    /// </summary>
    public int? BranchId { get; set; }

    /// <summary>
    /// Navigation property to branch
    /// </summary>
    public virtual Branch? Branch { get; set; }

    /// <summary>
    /// Optional: Specific menu item ID this offer applies to (null = all items)
    /// </summary>
    public int? MenuItemId { get; set; }

    /// <summary>
    /// Navigation property to menu item
    /// </summary>
    public virtual MenuItem? MenuItem { get; set; }

    /// <summary>
    /// Optional: Specific category ID this offer applies to (null = all categories)
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Navigation property to category
    /// </summary>
    public virtual MenuCategory? Category { get; set; }

    /// <summary>
    /// Check if the offer is valid for use
    /// </summary>
    public bool IsValid => IsActive && 
                           DateTime.UtcNow >= StartDate && 
                           DateTime.UtcNow <= EndDate &&
                           (UsageLimit == null || UsageCount < UsageLimit);

    /// <summary>
    /// Calculate discount amount for a given order total
    /// </summary>
    public decimal CalculateDiscount(decimal orderTotal)
    {
        if (!IsValid || orderTotal < (MinimumOrderAmount ?? 0))
            return 0;

        decimal discount = Type switch
        {
            OfferType.Percentage => orderTotal * (Value / 100),
            OfferType.FixedAmount => Value,
            OfferType.FreeDelivery => 0, // Handled separately
            OfferType.BuyOneGetOne => 0, // Handled separately
            _ => 0
        };

        // Apply maximum discount cap if set
        if (MaximumDiscount.HasValue && discount > MaximumDiscount.Value)
            discount = MaximumDiscount.Value;

        // Discount cannot exceed order total
        return Math.Min(discount, orderTotal);
    }
}

/// <summary>
/// Types of promotional offers
/// </summary>
public enum OfferType
{
    /// <summary>
    /// Percentage discount off the total
    /// </summary>
    Percentage = 0,

    /// <summary>
    /// Fixed amount discount
    /// </summary>
    FixedAmount = 1,

    /// <summary>
    /// Free delivery
    /// </summary>
    FreeDelivery = 2,

    /// <summary>
    /// Buy one get one free
    /// </summary>
    BuyOneGetOne = 3,

    /// <summary>
    /// Free item with order
    /// </summary>
    FreeItem = 4
}
