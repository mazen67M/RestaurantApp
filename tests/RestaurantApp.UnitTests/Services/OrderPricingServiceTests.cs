using Xunit;
using RestaurantApp.Application.Services;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Enums;

namespace RestaurantApp.UnitTests.Services;

public class OrderPricingServiceTests
{
    private readonly OrderPricingService _pricingService;

    public OrderPricingServiceTests()
    {
        _pricingService = new OrderPricingService();
    }

    [Fact]
    public void CalculateOrderPricing_DeliveryOrder_AppliesDeliveryFee()
    {
        // Arrange
        var branch = new Branch
        {
            DeliveryFee = 10,
            FreeDeliveryThreshold = 100
        };

        var orderItems = new List<OrderItem>
        {
            new OrderItem { TotalPrice = 50 }
        };

        // Act
        var result = _pricingService.CalculateOrderPricing(
            orderItems, branch, OrderType.Delivery);

        // Assert
        Assert.Equal(50, result.SubTotal);
        Assert.Equal(10, result.DeliveryFee);
        Assert.Equal(60, result.Total);
    }

    [Fact]
    public void CalculateOrderPricing_DeliveryOrderAboveThreshold_FreeDelivery()
    {
        // Arrange
        var branch = new Branch
        {
            DeliveryFee = 10,
            FreeDeliveryThreshold = 100
        };

        var orderItems = new List<OrderItem>
        {
            new OrderItem { TotalPrice = 150 }
        };

        // Act
        var result = _pricingService.CalculateOrderPricing(
            orderItems, branch, OrderType.Delivery);

        // Assert
        Assert.Equal(150, result.SubTotal);
        Assert.Equal(0, result.DeliveryFee);
        Assert.Equal(150, result.Total);
    }

    [Fact]
    public void CalculateOrderPricing_PickupOrder_NoDeliveryFee()
    {
        // Arrange
        var branch = new Branch
        {
            DeliveryFee = 10,
            FreeDeliveryThreshold = 100
        };

        var orderItems = new List<OrderItem>
        {
            new OrderItem { TotalPrice = 50 }
        };

        // Act
        var result = _pricingService.CalculateOrderPricing(
            orderItems, branch, OrderType.Pickup);

        // Assert
        Assert.Equal(50, result.SubTotal);
        Assert.Equal(0, result.DeliveryFee);
        Assert.Equal(50, result.Total);
    }

    [Fact]
    public void CalculateOrderPricing_WithDiscount_AppliesDiscount()
    {
        // Arrange
        var branch = new Branch
        {
            DeliveryFee = 10,
            FreeDeliveryThreshold = 100
        };

        var orderItems = new List<OrderItem>
        {
            new OrderItem { TotalPrice = 100 }
        };

        // Act
        var result = _pricingService.CalculateOrderPricing(
            orderItems, branch, OrderType.Delivery, discountAmount: 20);

        // Assert
        Assert.Equal(100, result.SubTotal);
        Assert.Equal(10, result.DeliveryFee);
        Assert.Equal(20, result.Discount);
        Assert.Equal(90, result.Total); // 100 + 10 - 20
    }

    [Fact]
    public void CalculateItemPrice_WithAddOns_CalculatesCorrectly()
    {
        // Arrange
        decimal unitPrice = 50;
        decimal addOnsTotal = 10;
        int quantity = 2;

        // Act
        var result = _pricingService.CalculateItemPrice(unitPrice, addOnsTotal, quantity);

        // Assert
        Assert.Equal(120, result); // (50 + 10) * 2
    }

    [Fact]
    public void MeetsMinimumOrder_BelowMinimum_ReturnsFalse()
    {
        // Arrange
        decimal subTotal = 40;
        decimal minimumOrderAmount = 50;

        // Act
        var result = _pricingService.MeetsMinimumOrder(subTotal, minimumOrderAmount);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void MeetsMinimumOrder_MeetsMinimum_ReturnsTrue()
    {
        // Arrange
        decimal subTotal = 60;
        decimal minimumOrderAmount = 50;

        // Act
        var result = _pricingService.MeetsMinimumOrder(subTotal, minimumOrderAmount);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void MeetsMinimumOrder_ExactlyMinimum_ReturnsTrue()
    {
        // Arrange
        decimal subTotal = 50;
        decimal minimumOrderAmount = 50;

        // Act
        var result = _pricingService.MeetsMinimumOrder(subTotal, minimumOrderAmount);

        // Assert
        Assert.True(result);
    }
}
