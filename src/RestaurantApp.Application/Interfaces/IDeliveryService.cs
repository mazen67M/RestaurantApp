using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Delivery;

namespace RestaurantApp.Application.Interfaces;

public interface IDeliveryService
{
    // CRUD operations
    Task<ApiResponse<List<DeliveryDto>>> GetAllDeliveriesAsync();
    Task<ApiResponse<List<DeliveryDto>>> GetAvailableDeliveriesAsync();
    Task<ApiResponse<DeliveryDto>> GetDeliveryByIdAsync(int id);
    Task<ApiResponse<DeliveryDto>> CreateDeliveryAsync(CreateDeliveryDto dto);
    Task<ApiResponse<DeliveryDto>> UpdateDeliveryAsync(int id, UpdateDeliveryDto dto);
    Task<ApiResponse<bool>> DeleteDeliveryAsync(int id);
    
    // Statistics
    Task<ApiResponse<DeliveryStatsDto>> GetDeliveryStatsAsync(int deliveryId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ApiResponse<List<DeliveryStatsDto>>> GetAllDeliveryStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
    
    // Order assignment
    Task<ApiResponse<bool>> AssignDeliveryToOrderAsync(int orderId, int deliveryId);
    Task<ApiResponse<bool>> SetDeliveryAvailabilityAsync(int deliveryId, bool isAvailable);
}
