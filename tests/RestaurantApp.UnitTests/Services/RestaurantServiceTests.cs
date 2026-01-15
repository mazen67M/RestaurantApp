using Microsoft.EntityFrameworkCore;
using RestaurantApp.Application.DTOs.Restaurant;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantApp.UnitTests.Services;

public class RestaurantServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public RestaurantServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetRestaurantAsync_ActiveRestaurantExists_ReturnsRestaurant()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        context.Restaurants.Add(new Restaurant 
        { 
            NameAr = "مطعم الاختبار", 
            NameEn = "Test Restaurant", 
            IsActive = true 
        });
        await context.SaveChangesAsync();

        var service = new RestaurantService(context);

        // Act
        var result = await service.GetRestaurantAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Test Restaurant", result.Data!.NameEn);
    }

    [Fact]
    public async Task GetNearestBranchAsync_WithinRadius_ReturnsBranch()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var restaurant = new Restaurant { NameAr = "R", NameEn = "R", IsActive = true };
        context.Restaurants.Add(restaurant);
        await context.SaveChangesAsync();

        var branch = new Branch
        {
            RestaurantId = restaurant.Id,
            NameAr = "ب1",
            NameEn = "B1",
            Latitude = 24.7136m, // Riyadh
            Longitude = 46.6753m,
            DeliveryRadiusKm = 10,
            IsActive = true,
            AcceptingOrders = true,
            OpeningTime = new TimeSpan(8, 0, 0),
            ClosingTime = new TimeSpan(23, 0, 0)
        };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var service = new RestaurantService(context);

        // Act
        // Current location very close to Riyadh center
        var result = await service.GetNearestBranchAsync(24.71m, 46.67m);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("B1", result.Data!.NameEn);
        Assert.True(result.Data.DistanceKm < 10);
    }

    [Fact]
    public async Task GetNearestBranchAsync_OutsideRadius_ReturnsError()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var restaurant = new Restaurant { NameAr = "R", NameEn = "R", IsActive = true };
        context.Restaurants.Add(restaurant);
        await context.SaveChangesAsync();

        var branch = new Branch
        {
            RestaurantId = restaurant.Id,
            NameAr = "B1",
            NameEn = "B1",
            Latitude = 24.7136m,
            Longitude = 46.6753m,
            DeliveryRadiusKm = 5, // Small radius
            IsActive = true,
            AcceptingOrders = true,
            OpeningTime = new TimeSpan(8, 0, 0),
            ClosingTime = new TimeSpan(23, 0, 0)
        };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var service = new RestaurantService(context);

        // Act
        // Location far away (Dubai coords roughly)
        var result = await service.GetNearestBranchAsync(25.2048m, 55.2708m);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("No branches deliver to your location", result.Message);
    }
}
