using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Address;

namespace RestaurantApp.Application.Interfaces;

public interface IAddressService
{
    Task<ApiResponse<List<UserAddressDto>>> GetAddressesAsync(int userId);
    Task<ApiResponse<UserAddressDto>> GetAddressAsync(int userId, int addressId);
    Task<ApiResponse<UserAddressDto>> CreateAddressAsync(int userId, CreateAddressDto dto);
    Task<ApiResponse<UserAddressDto>> UpdateAddressAsync(int userId, int addressId, UpdateAddressDto dto);
    Task<ApiResponse> DeleteAddressAsync(int userId, int addressId);
    Task<ApiResponse> SetDefaultAddressAsync(int userId, int addressId);
}
