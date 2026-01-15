using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Offer;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantApp.UnitTests.Services;

public class OfferServiceTests
{
    private readonly Mock<ILogger<OfferService>> _loggerMock;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public OfferServiceTests()
    {
        _loggerMock = new Mock<ILogger<OfferService>>();
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task ValidateCouponAsync_ValidCoupon_ReturnsSuccess()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var offer = new Offer
        {
            Code = "WELCOME10",
            NameAr = "Welcome",
            NameEn = "Welcome",
            Type = OfferType.Percentage,
            Value = 10,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(7)
        };
        context.Offers.Add(offer);
        await context.SaveChangesAsync();

        var service = new OfferService(context, _loggerMock.Object);

        // Act
        var result = await service.ValidateCouponAsync("WELCOME10", 100);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.Data!.IsValid);
        Assert.Equal(10, result.Data.DiscountAmount);
    }

    [Fact]
    public async Task ValidateCouponAsync_InvalidCode_ReturnsNotValid()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var service = new OfferService(context, _loggerMock.Object);

        // Act
        var result = await service.ValidateCouponAsync("INVALID", 100);

        // Assert
        Assert.True(result.Success);
        Assert.False(result.Data!.IsValid);
        Assert.Equal("Invalid coupon code", result.Data.Message);
    }

    [Fact]
    public async Task ValidateCouponAsync_ExpiredCoupon_ReturnsExpiredMessage()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var offer = new Offer
        {
            Code = "EXPIRED",
            NameAr = "Expired",
            NameEn = "Expired",
            Type = OfferType.FixedAmount,
            Value = 5,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(-1)
        };
        context.Offers.Add(offer);
        await context.SaveChangesAsync();

        var service = new OfferService(context, _loggerMock.Object);

        // Act
        var result = await service.ValidateCouponAsync("EXPIRED", 100);

        // Assert
        Assert.False(result.Data!.IsValid);
        Assert.Equal("This coupon has expired", result.Data.Message);
    }

    [Fact]
    public async Task CreateOfferAsync_DuplicateCode_ReturnsError()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        context.Offers.Add(new Offer { Code = "EXISTING", NameAr = "Existing", NameEn = "Existing" });
        await context.SaveChangesAsync();

        var service = new OfferService(context, _loggerMock.Object);
        var request = new CreateOfferRequest 
        { 
            Code = "EXISTING", 
            Type = "Percentage",
            NameAr = "Test",
            NameEn = "Test"
        };

        // Act
        var result = await service.CreateOfferAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Coupon code already exists", result.Message);
    }
}
