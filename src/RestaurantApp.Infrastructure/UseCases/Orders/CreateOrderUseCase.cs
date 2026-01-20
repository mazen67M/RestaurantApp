using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Order;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Application.Services;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Enums;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.UseCases.Orders;

/// <summary>
/// Use case for creating a new order
/// Orchestrates the order creation process with proper separation of concerns
/// </summary>
public class CreateOrderUseCase
{
    private readonly ApplicationDbContext _context;
    private readonly OrderPricingService _pricingService;
    private readonly CouponValidationService _couponService;
    private readonly IEmailService _emailService;
    private readonly ILoyaltyService _loyaltyService;
    private readonly IOrderNotificationService _notificationService;
    private readonly ILogger<CreateOrderUseCase> _logger;

    public CreateOrderUseCase(
        ApplicationDbContext context,
        OrderPricingService pricingService,
        CouponValidationService couponService,
        IEmailService emailService,
        ILoyaltyService loyaltyService,
        IOrderNotificationService notificationService,
        ILogger<CreateOrderUseCase> logger)
    {
        _context = context;
        _pricingService = pricingService;
        _couponService = couponService;
        _emailService = emailService;
        _loyaltyService = loyaltyService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<ApiResponse<OrderCreatedDto>> ExecuteAsync(int userId, CreateOrderDto dto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Step 1: Validate branch
            var branch = await ValidateBranchAsync(dto.BranchId);
            if (branch == null)
            {
                return ApiResponse<OrderCreatedDto>.ErrorResponse("Branch not available");
            }

            // Step 2: Validate and get menu items
            var menuItems = await GetAndValidateMenuItemsAsync(dto.Items);
            if (menuItems == null)
            {
                return ApiResponse<OrderCreatedDto>.ErrorResponse("One or more items are not available");
            }

            // Step 3: Build order entity
            var order = BuildOrderEntity(userId, dto);

            // Step 4: Add order items
            var orderItems = BuildOrderItems(dto.Items, menuItems);
            foreach (var item in orderItems)
            {
                order.OrderItems.Add(item);
            }

            // Step 5: Calculate pricing
            var pricing = _pricingService.CalculateOrderPricing(
                orderItems, branch, dto.OrderType);
            
            order.SubTotal = pricing.SubTotal;
            order.DeliveryFee = pricing.DeliveryFee;

            // Step 6: Apply coupon if provided
            if (!string.IsNullOrWhiteSpace(dto.CouponCode))
            {
                var couponResult = await ApplyCouponAsync(dto.CouponCode, pricing.SubTotal, dto.BranchId);
                if (!couponResult.IsValid)
                {
                    return ApiResponse<OrderCreatedDto>.ErrorResponse(couponResult.ErrorMessage!);
                }

                order.Discount = couponResult.DiscountAmount;
                order.CouponCode = dto.CouponCode;
            }

            // Step 7: Validate minimum order
            if (!_pricingService.MeetsMinimumOrder(pricing.SubTotal, branch.MinOrderAmount))
            {
                return ApiResponse<OrderCreatedDto>.ErrorResponse(
                    $"Minimum order amount is {branch.MinOrderAmount}");
            }

            // Step 8: Calculate final total
            order.Total = order.SubTotal + order.DeliveryFee - order.Discount;
            order.EstimatedDeliveryTime = DateTime.UtcNow.AddMinutes(
                branch.DefaultPreparationTimeMinutes + 15);

            // Step 9: Save order
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Step 10: Send notifications (outside transaction)
            await SendNotificationsAsync(userId, order);

            return ApiResponse<OrderCreatedDto>.SuccessResponse(new OrderCreatedDto(
                order.Id,
                order.OrderNumber,
                order.Total,
                order.EstimatedDeliveryTime.Value
            ));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating order for user {UserId}", userId);
            return ApiResponse<OrderCreatedDto>.ErrorResponse(
                "Failed to create order. Please try again.");
        }
    }

    private async Task<Branch?> ValidateBranchAsync(int branchId)
    {
        var branch = await _context.Branches.FindAsync(branchId);
        return branch != null && branch.IsActive && branch.AcceptingOrders ? branch : null;
    }

    private async Task<Dictionary<int, MenuItem>?> GetAndValidateMenuItemsAsync(
        List<CreateOrderItemDto> items)
    {
        var menuItemIds = items.Select(i => i.MenuItemId).ToList();
        var menuItems = await _context.MenuItems
            .Include(i => i.AddOns)
            .Where(i => menuItemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);

        // Validate all items exist and are available
        foreach (var item in items)
        {
            if (!menuItems.TryGetValue(item.MenuItemId, out var menuItem) || !menuItem.IsAvailable)
            {
                return null;
            }
        }

        return menuItems;
    }

    private Order BuildOrderEntity(int userId, CreateOrderDto dto)
    {
        return new Order
        {
            UserId = userId,
            BranchId = dto.BranchId,
            OrderNumber = GenerateOrderNumber(),
            OrderType = dto.OrderType,
            Status = OrderStatus.Pending,
            PaymentMethod = PaymentMethod.CashOnDelivery,
            PaymentStatus = PaymentStatus.Pending,
            DeliveryAddressLine = dto.DeliveryAddressLine,
            DeliveryLatitude = dto.DeliveryLatitude,
            DeliveryLongitude = dto.DeliveryLongitude,
            DeliveryNotes = dto.DeliveryNotes,
            RequestedDeliveryTime = dto.RequestedDeliveryTime,
            CustomerNotes = dto.CustomerNotes
        };
    }

    private List<OrderItem> BuildOrderItems(
        List<CreateOrderItemDto> itemDtos,
        Dictionary<int, MenuItem> menuItems)
    {
        var orderItems = new List<OrderItem>();

        foreach (var itemDto in itemDtos)
        {
            var menuItem = menuItems[itemDto.MenuItemId];
            var unitPrice = menuItem.DiscountedPrice ?? menuItem.Price;

            var orderItem = new OrderItem
            {
                MenuItemId = itemDto.MenuItemId,
                MenuItemNameAr = menuItem.NameAr,
                MenuItemNameEn = menuItem.NameEn,
                UnitPrice = unitPrice,
                Quantity = itemDto.Quantity,
                Notes = itemDto.Notes
            };

            // Add add-ons
            decimal addOnsTotal = 0;
            if (itemDto.AddOnIds?.Any() == true)
            {
                foreach (var addOnId in itemDto.AddOnIds)
                {
                    var addOn = menuItem.AddOns.FirstOrDefault(a => a.Id == addOnId && a.IsAvailable);
                    if (addOn != null)
                    {
                        orderItem.OrderItemAddOns.Add(new OrderItemAddOn
                        {
                            MenuItemAddOnId = addOn.Id,
                            NameAr = addOn.NameAr,
                            NameEn = addOn.NameEn,
                            Price = addOn.Price
                        });
                        addOnsTotal += addOn.Price;
                    }
                }
            }

            orderItem.AddOnsTotal = addOnsTotal;
            orderItem.TotalPrice = _pricingService.CalculateItemPrice(
                unitPrice, addOnsTotal, itemDto.Quantity);

            orderItems.Add(orderItem);
        }

        return orderItems;
    }

    private async Task<CouponValidationResult> ApplyCouponAsync(
        string couponCode, decimal subTotal, int branchId)
    {
        var offer = await _context.Offers
            .FirstOrDefaultAsync(o => o.Code.ToLower() == couponCode.ToLower());

        if (offer == null)
        {
            return CouponValidationResult.Invalid("Invalid coupon code");
        }

        var result = _couponService.ValidateAndCalculateDiscount(
            offer, subTotal, branchId, DateTime.UtcNow);

        if (result.IsValid)
        {
            // Increment usage count
            offer.UsageCount++;
        }

        return result;
    }

    private async Task SendNotificationsAsync(int userId, Order order)
    {
        var user = await _context.Users.FindAsync(userId);

        // Send email (fire and forget)
        if (user?.Email != null)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendOrderConfirmationAsync(
                        user.Email, order.OrderNumber, order.Total);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to send order confirmation email for order {OrderId}", order.Id);
                }
            });
        }

        // Send real-time notification
        await _notificationService.NotifyNewOrder(
            order.Id,
            order.OrderNumber,
            user?.FullName ?? "Customer",
            order.OrderItems.Sum(i => i.Quantity),
            order.Total,
            order.OrderType);
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    }
}
