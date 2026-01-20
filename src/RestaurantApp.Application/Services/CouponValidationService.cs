using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Enums;

namespace RestaurantApp.Application.Services;

/// <summary>
/// Domain service for coupon/offer validation and discount calculation
/// </summary>
public class CouponValidationService
{
    /// <summary>
    /// Validate coupon and calculate discount
    /// </summary>
    public CouponValidationResult ValidateAndCalculateDiscount(
        Offer offer,
        decimal subTotal,
        int branchId,
        DateTime currentTime)
    {
        // Validate offer
        if (!offer.IsActive)
        {
            return CouponValidationResult.Invalid("This coupon is no longer active");
        }

        if (currentTime < offer.StartDate)
        {
            return CouponValidationResult.Invalid("This coupon is not yet active");
        }

        if (currentTime > offer.EndDate)
        {
            return CouponValidationResult.Invalid("This coupon has expired");
        }

        if (offer.UsageLimit.HasValue && offer.UsageCount >= offer.UsageLimit.Value)
        {
            return CouponValidationResult.Invalid("This coupon has reached its usage limit");
        }

        if (offer.MinimumOrderAmount.HasValue && subTotal < offer.MinimumOrderAmount.Value)
        {
            return CouponValidationResult.Invalid(
                $"Minimum order amount for this coupon is {offer.MinimumOrderAmount.Value}");
        }

        if (offer.BranchId.HasValue && offer.BranchId != branchId)
        {
            return CouponValidationResult.Invalid("This coupon is not valid for the selected branch");
        }

        // Calculate discount
        decimal discount = CalculateDiscount(offer, subTotal);

        return CouponValidationResult.Valid(discount);
    }

    /// <summary>
    /// Calculate discount amount based on offer type
    /// </summary>
    private decimal CalculateDiscount(Offer offer, decimal subTotal)
    {
        decimal discount = 0;

        if (offer.Type == OfferType.Percentage)
        {
            discount = subTotal * (offer.Value / 100);
            if (offer.MaximumDiscount.HasValue && discount > offer.MaximumDiscount.Value)
            {
                discount = offer.MaximumDiscount.Value;
            }
        }
        else if (offer.Type == OfferType.FixedAmount)
        {
            discount = offer.Value;
        }

        return discount;
    }
}

/// <summary>
/// Result of coupon validation
/// </summary>
public class CouponValidationResult
{
    public bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }
    public decimal DiscountAmount { get; init; }

    private CouponValidationResult() { }

    public static CouponValidationResult Valid(decimal discountAmount) => new()
    {
        IsValid = true,
        DiscountAmount = discountAmount
    };

    public static CouponValidationResult Invalid(string errorMessage) => new()
    {
        IsValid = false,
        ErrorMessage = errorMessage
    };
}
