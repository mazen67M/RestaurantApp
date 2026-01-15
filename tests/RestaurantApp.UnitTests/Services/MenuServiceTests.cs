using Microsoft.EntityFrameworkCore;
using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Menu;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantApp.UnitTests.Services;

public class MenuServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public MenuServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetCategoriesAsync_ActiveOnly_ReturnsActiveCategories()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        context.MenuCategories.Add(new MenuCategory { NameAr = "C1", NameEn = "C1", IsActive = true, RestaurantId = 1 });
        context.MenuCategories.Add(new MenuCategory { NameAr = "C2", NameEn = "C2", IsActive = false, RestaurantId = 1 });
        await context.SaveChangesAsync();

        var service = new MenuService(context);

        // Act
        var result = await service.GetCategoriesAsync(includeInactive: false);

        // Assert
        Assert.Single(result.Data!);
        Assert.True(result.Data![0].IsActive);
    }

    [Fact]
    public async Task GetAllItemsAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var category = new MenuCategory { NameAr = "C", NameEn = "C", IsActive = true, RestaurantId = 1 };
        context.MenuCategories.Add(category);
        
        for (int i = 1; i <= 10; i++)
        {
            context.MenuItems.Add(new MenuItem 
            { 
                NameAr = $"Item {i}", 
                NameEn = $"Item {i}", 
                Price = i * 10,
                CategoryId = category.Id,
                IsAvailable = true
            });
        }
        await context.SaveChangesAsync();

        var service = new MenuService(context);

        // Act
        var result = await service.GetAllItemsAsync(page: 2, pageSize: 3);

        // Assert
        Assert.Equal(3, result.Data!.Items.Count);
        Assert.Equal(10, result.Data.TotalCount);
        Assert.Equal(2, result.Data.Page);
    }

    [Fact]
    public async Task ToggleItemAvailabilityAsync_ExistingItem_TogglesState()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var item = new MenuItem { NameAr = "I", NameEn = "I", IsAvailable = true, CategoryId = 1 };
        context.MenuItems.Add(item);
        await context.SaveChangesAsync();

        var service = new MenuService(context);

        // Act
        var result = await service.ToggleItemAvailabilityAsync(item.Id);

        // Assert
        var updatedItem = await context.MenuItems.FindAsync(item.Id);
        Assert.False(updatedItem!.IsAvailable);
        Assert.Contains("unavailable", result.Message);
    }
}
