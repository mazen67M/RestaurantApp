using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Enums;

namespace RestaurantApp.Application.Services;

/// <summary>
/// Domain service for order pricing calculations
/// </summary>
public class OrderPricingService
{
    /// <summary>
    /// Calculate order pricing including items, add-ons, delivery, and discounts
    /// </summary>
    public OrderPricingResult CalculateOrderPricing(
        List<OrderItem> orderItems,
        Branch branch,
        OrderType orderType,
        decimal? discountAmount = null)
    {
        var subTotal = orderItems.Sum(item => item.TotalPrice);
        
        // Calculate delivery fee
        decimal deliveryFee = 0;
        if (orderType == OrderType.Delivery)
        {
            deliveryFee = subTotal >= branch.FreeDeliveryThreshold && branch.FreeDeliveryThreshold > 0
                ? 0
                : branch.DeliveryFee;
        }

        var discount = discountAmount ?? 0;
        var total = subTotal + deliveryFee - discount;

        return new OrderPricingResult(subTotal, deliveryFee, discount, total);
    }

    /// <summary>
    /// Calculate item price including add-ons
    /// </summary>
    public decimal CalculateItemPrice(decimal unitPrice, decimal addOnsTotal, int quantity)
    {
        return (unitPrice + addOnsTotal) * quantity;
    }

    /// <summary>
    /// Validate minimum order amount
    /// </summary>
    public bool MeetsMinimumOrder(decimal subTotal, decimal minimumOrderAmount)
    {
        return subTotal >= minimumOrderAmount;
    }
}

/// <summary>
/// Result of order pricing calculation
/// </summary>
public record OrderPricingResult(
    decimal SubTotal,
    decimal DeliveryFee,
    decimal Discount,
    decimal Total);
