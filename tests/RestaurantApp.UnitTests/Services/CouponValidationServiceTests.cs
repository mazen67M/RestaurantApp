using Xunit;
using RestaurantApp.Application.Services;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Enums;

namespace RestaurantApp.UnitTests.Services;

public class CouponValidationServiceTests
{
    private readonly CouponValidationService _couponService;

    public CouponValidationServiceTests()
    {
        _couponService = new CouponValidationService();
    }

    [Fact]
    public void ValidateAndCalculateDiscount_InactiveOffer_ReturnsInvalid()
    {
        // Arrange
        var offer = new Offer
        {
            IsActive = false,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = _couponService.ValidateAndCalculateDiscount(
            offer, 100, 1, DateTime.UtcNow);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("This coupon is no longer active", result.ErrorMessage);
    }

    [Fact]
    public void ValidateAndCalculateDiscount_NotYetActive_ReturnsInvalid()
    {
        // Arrange
        var offer = new Offer
        {
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = _couponService.ValidateAndCalculateDiscount(
            offer, 100, 1, DateTime.UtcNow);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("This coupon is not yet active", result.ErrorMessage);
    }

    [Fact]
    public void ValidateAndCalculateDiscount_Expired_ReturnsInvalid()
    {
        // Arrange
        var offer = new Offer
        {
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = _couponService.ValidateAndCalculateDiscount(
            offer, 100, 1, DateTime.UtcNow);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("This coupon has expired", result.ErrorMessage);
    }

    [Fact]
    public void ValidateAndCalculateDiscount_UsageLimitReached_ReturnsInvalid()
    {
        // Arrange
        var offer = new Offer
        {
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10),
            UsageLimit = 10,
            UsageCount = 10
        };

        // Act
        var result = _couponService.ValidateAndCalculateDiscount(
            offer, 100, 1, DateTime.UtcNow);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("This coupon has reached its usage limit", result.ErrorMessage);
    }

    [Fact]
    public void ValidateAndCalculateDiscount_BelowMinimumOrder_ReturnsInvalid()
    {
        // Arrange
        var offer = new Offer
        {
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10),
            MinimumOrderAmount = 100
        };

        // Act
        var result = _couponService.ValidateAndCalculateDiscount(
            offer, 50, 1, DateTime.UtcNow);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Minimum order amount", result.ErrorMessage);
    }

    [Fact]
    public void ValidateAndCalculateDiscount_WrongBranch_ReturnsInvalid()
    {
        // Arrange
        var offer = new Offer
        {
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10),
            BranchId = 1
        };

        // Act
        var result = _couponService.ValidateAndCalculateDiscount(
            offer, 100, 2, DateTime.UtcNow);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("This coupon is not valid for the selected branch", result.ErrorMessage);
    }

    [Fact]
    public void ValidateAndCalculateDiscount_PercentageDiscount_CalculatesCorrectly()
    {
        // Arrange
        var offer = new Offer
        {
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10),
            Type = OfferType.Percentage,
            Value = 20 // 20%
        };

        // Act
        var result = _couponService.ValidateAndCalculateDiscount(
            offer, 100, 1, DateTime.UtcNow);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(20, result.DiscountAmount); // 20% of 100
    }

    [Fact]
    public void ValidateAndCalculateDiscount_PercentageWithMaxDiscount_CapsAtMaximum()
    {
        // Arrange
        var offer = new Offer
        {
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10),
            Type = OfferType.Percentage,
            Value = 20, // 20%
            MaximumDiscount = 15
        };

        // Act
        var result = _couponService.ValidateAndCalculateDiscount(
            offer, 100, 1, DateTime.UtcNow);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(15, result.DiscountAmount); // Capped at 15
    }

    [Fact]
    public void ValidateAndCalculateDiscount_FixedAmount_ReturnsFixedValue()
    {
        // Arrange
        var offer = new Offer
        {
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10),
            Type = OfferType.FixedAmount,
            Value = 25
        };

        // Act
        var result = _couponService.ValidateAndCalculateDiscount(
            offer, 100, 1, DateTime.UtcNow);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(25, result.DiscountAmount);
    }

    [Fact]
    public void ValidateAndCalculateDiscount_ValidOffer_ReturnsValid()
    {
        // Arrange
        var offer = new Offer
        {
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10),
            Type = OfferType.Percentage,
            Value = 10,
            MinimumOrderAmount = 50
        };

        // Act
        var result = _couponService.ValidateAndCalculateDiscount(
            offer, 100, 1, DateTime.UtcNow);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(10, result.DiscountAmount);
        Assert.Null(result.ErrorMessage);
    }
}
