using Microsoft.EntityFrameworkCore;
using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Delivery;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Enums;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Services;

public class DeliveryService : IDeliveryService
{
    private readonly ApplicationDbContext _context;

    public DeliveryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<DeliveryDto>>> GetAllDeliveriesAsync()
    {
        var deliveries = await _context.Deliveries
            .Where(d => d.IsActive)
            .OrderBy(d => d.NameEn)
            .ToListAsync();

        var dtos = deliveries.Select(MapToDto).ToList();
        return ApiResponse<List<DeliveryDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<List<DeliveryDto>>> GetAvailableDeliveriesAsync()
    {
        var deliveries = await _context.Deliveries
            .Where(d => d.IsActive && d.IsAvailable)
            .OrderBy(d => d.NameEn)
            .ToListAsync();

        var dtos = deliveries.Select(MapToDto).ToList();
        return ApiResponse<List<DeliveryDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<DeliveryDto>> GetDeliveryByIdAsync(int id)
    {
        var delivery = await _context.Deliveries.FindAsync(id);
        if (delivery == null)
        {
            return ApiResponse<DeliveryDto>.ErrorResponse("Delivery not found");
        }

        return ApiResponse<DeliveryDto>.SuccessResponse(MapToDto(delivery));
    }

    public async Task<ApiResponse<DeliveryDto>> CreateDeliveryAsync(CreateDeliveryDto dto)
    {
        var delivery = new Delivery
        {
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Phone = dto.Phone,
            Email = dto.Email,
            IsActive = dto.IsActive,
            IsAvailable = dto.IsAvailable
        };

        _context.Deliveries.Add(delivery);
        await _context.SaveChangesAsync();

        return ApiResponse<DeliveryDto>.SuccessResponse(MapToDto(delivery));
    }

    public async Task<ApiResponse<DeliveryDto>> UpdateDeliveryAsync(int id, UpdateDeliveryDto dto)
    {
        var delivery = await _context.Deliveries.FindAsync(id);
        if (delivery == null)
        {
            return ApiResponse<DeliveryDto>.ErrorResponse("Delivery not found");
        }

        if (dto.NameAr != null) delivery.NameAr = dto.NameAr;
        if (dto.NameEn != null) delivery.NameEn = dto.NameEn;
        if (dto.Phone != null) delivery.Phone = dto.Phone;
        if (dto.Email != null) delivery.Email = dto.Email;
        if (dto.IsActive.HasValue) delivery.IsActive = dto.IsActive.Value;
        if (dto.IsAvailable.HasValue) delivery.IsAvailable = dto.IsAvailable.Value;

        await _context.SaveChangesAsync();

        return ApiResponse<DeliveryDto>.SuccessResponse(MapToDto(delivery));
    }

    public async Task<ApiResponse<bool>> DeleteDeliveryAsync(int id)
    {
        var delivery = await _context.Deliveries.FindAsync(id);
        if (delivery == null)
        {
            return ApiResponse<bool>.ErrorResponse("Delivery not found");
        }

        // Soft delete
        delivery.IsActive = false;
        delivery.IsAvailable = false;
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<DeliveryStatsDto>> GetDeliveryStatsAsync(int deliveryId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var delivery = await _context.Deliveries.FindAsync(deliveryId);
        if (delivery == null)
        {
            return ApiResponse<DeliveryStatsDto>.ErrorResponse("Delivery not found");
        }

        var today = DateTime.Today;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var ordersQuery = _context.Orders
            .Where(o => o.DeliveryId == deliveryId && o.Status == OrderStatus.Delivered);

        var allOrders = await ordersQuery.ToListAsync();
        var ordersToday = allOrders.Where(o => o.ActualDeliveryTime?.Date == today).ToList();
        var ordersThisWeek = allOrders.Where(o => o.ActualDeliveryTime >= weekStart).ToList();
        var ordersThisMonth = allOrders.Where(o => o.ActualDeliveryTime >= monthStart).ToList();

        var stats = new DeliveryStatsDto(
            deliveryId,
            delivery.NameEn,
            allOrders.Count,
            allOrders.Sum(o => o.Total),
            ordersToday.Count,
            ordersToday.Sum(o => o.Total),
            ordersThisWeek.Count,
            ordersThisWeek.Sum(o => o.Total),
            ordersThisMonth.Count,
            ordersThisMonth.Sum(o => o.Total)
        );

        return ApiResponse<DeliveryStatsDto>.SuccessResponse(stats);
    }

    public async Task<ApiResponse<List<DeliveryStatsDto>>> GetAllDeliveryStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var deliveries = await _context.Deliveries.Where(d => d.IsActive).ToListAsync();
        var statsList = new List<DeliveryStatsDto>();

        foreach (var delivery in deliveries)
        {
            var result = await GetDeliveryStatsAsync(delivery.Id, startDate, endDate);
            if (result.Success && result.Data != null)
            {
                statsList.Add(result.Data);
            }
        }

        return ApiResponse<List<DeliveryStatsDto>>.SuccessResponse(statsList);
    }

    public async Task<ApiResponse<bool>> AssignDeliveryToOrderAsync(int orderId, int deliveryId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return ApiResponse<bool>.ErrorResponse("Order not found");
        }

        var delivery = await _context.Deliveries.FindAsync(deliveryId);
        if (delivery == null)
        {
            return ApiResponse<bool>.ErrorResponse("Delivery not found");
        }

        if (!delivery.IsActive || !delivery.IsAvailable)
        {
            return ApiResponse<bool>.ErrorResponse("Delivery is not available");
        }

        order.DeliveryId = deliveryId;
        order.AssignedToDeliveryAt = DateTime.UtcNow;
        order.Status = OrderStatus.OutForDelivery;

        // Update delivery stats
        delivery.TotalOrders++;

        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<bool>> SetDeliveryAvailabilityAsync(int deliveryId, bool isAvailable)
    {
        var delivery = await _context.Deliveries.FindAsync(deliveryId);
        if (delivery == null)
        {
            return ApiResponse<bool>.ErrorResponse("Delivery not found");
        }

        delivery.IsAvailable = isAvailable;
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true);
    }

    private static DeliveryDto MapToDto(Delivery delivery)
    {
        return new DeliveryDto(
            delivery.Id,
            delivery.NameAr,
            delivery.NameEn,
            delivery.Phone,
            delivery.Email,
            delivery.IsActive,
            delivery.IsAvailable,
            delivery.TotalOrders,
            delivery.TotalRevenue,
            delivery.CreatedAt
        );
    }
}
