using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Order;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Enums;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantApp.UnitTests.Services;

public class OrderServiceTests
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ILoyaltyService> _loyaltyServiceMock;
    private readonly Mock<IOrderNotificationService> _notificationServiceMock;
    private readonly Mock<ILogger<OrderService>> _loggerMock;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public OrderServiceTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _loyaltyServiceMock = new Mock<ILoyaltyService>();
        _notificationServiceMock = new Mock<IOrderNotificationService>();
        _loggerMock = new Mock<ILogger<OrderService>>();
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task CreateOrderAsync_ValidRequest_CreatesOrderSuccessfully()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        
        var branch = new Branch 
        { 
            Id = 1, 
            NameAr = "B", 
            NameEn = "B", 
            IsActive = true, 
            AcceptingOrders = true,
            MinOrderAmount = 50,
            DeliveryFee = 10,
            FreeDeliveryThreshold = 200,
            DefaultPreparationTimeMinutes = 30
        };
        context.Branches.Add(branch);

        var item = new MenuItem 
        { 
            Id = 1, 
            NameAr = "I", 
            NameEn = "I", 
            Price = 100, 
            IsAvailable = true,
            CategoryId = 1
        };
        context.MenuItems.Add(item);

        var user = new ApplicationUser { Id = 1, Email = "test@test.com", FullName = "Test User" };
        context.Users.Add(user);
        
        await context.SaveChangesAsync();

        var service = new OrderService(
            context, 
            _emailServiceMock.Object, 
            _loyaltyServiceMock.Object, 
            _notificationServiceMock.Object, 
            _loggerMock.Object);

        var dto = new CreateOrderDto(
            BranchId: 1,
            OrderType: OrderType.Delivery,
            AddressId: 1,
            DeliveryAddressLine: "Address",
            DeliveryLatitude: 0,
            DeliveryLongitude: 0,
            DeliveryNotes: null,
            RequestedDeliveryTime: null,
            CustomerNotes: "Notes",
            CouponCode: null,
            Items: new List<CreateOrderItemDto> { new CreateOrderItemDto(MenuItemId: 1, Quantity: 1, Notes: null, AddOnIds: null) }
        );

        // Act
        var result = await service.CreateOrderAsync(1, dto);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(110, result.Data.Total); // (100 * 1) + 10 delivery fee

        var order = await context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == result.Data.OrderId);
        Assert.NotNull(order);
        Assert.Single(order.OrderItems);
        Assert.Equal(1, order.OrderItems.First().Quantity);

        _emailServiceMock.Verify(e => e.SendOrderConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()), Times.Once);
        _notificationServiceMock.Verify(n => n.NotifyNewOrder(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<OrderType>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_BelowMinOrder_ReturnsError()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        context.Branches.Add(new Branch { Id = 1, MinOrderAmount = 100, IsActive = true, AcceptingOrders = true });
        context.MenuItems.Add(new MenuItem { Id = 1, Price = 50, IsAvailable = true });
        await context.SaveChangesAsync();

        var service = new OrderService(context, _emailServiceMock.Object, _loyaltyServiceMock.Object, _notificationServiceMock.Object, _loggerMock.Object);

        var dto = new CreateOrderDto(1, OrderType.Pickup, null, null, null, null, null, null, null, null, 
            new List<CreateOrderItemDto> { new CreateOrderItemDto(1, 1, null, null) });

        // Act
        var result = await service.CreateOrderAsync(1, dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Minimum order amount is 100", result.Message);
    }
}
