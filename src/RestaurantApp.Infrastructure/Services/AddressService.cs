using Microsoft.EntityFrameworkCore;
using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Address;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Services;

public class AddressService : IAddressService
{
    private readonly ApplicationDbContext _context;

    public AddressService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<UserAddressDto>>> GetAddressesAsync(int userId)
    {
        var addresses = await _context.UserAddresses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();

        var dtos = addresses.Select(a => new UserAddressDto(
            a.Id,
            a.Label,
            a.AddressLine,
            a.BuildingName,
            a.Floor,
            a.Apartment,
            a.Landmark,
            a.Latitude,
            a.Longitude,
            a.IsDefault
        )).ToList();

        return ApiResponse<List<UserAddressDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<UserAddressDto>> GetAddressAsync(int userId, int addressId)
    {
        var address = await _context.UserAddresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        if (address == null)
        {
            return ApiResponse<UserAddressDto>.ErrorResponse("Address not found");
        }

        return ApiResponse<UserAddressDto>.SuccessResponse(MapToDto(address));
    }

    public async Task<ApiResponse<UserAddressDto>> CreateAddressAsync(int userId, CreateAddressDto dto)
    {
        // If this is the first address or marked as default, make it default
        var hasAddresses = await _context.UserAddresses.AnyAsync(a => a.UserId == userId);
        
        var address = new UserAddress
        {
            UserId = userId,
            Label = dto.Label,
            AddressLine = dto.AddressLine,
            BuildingName = dto.BuildingName,
            Floor = dto.Floor,
            Apartment = dto.Apartment,
            Landmark = dto.Landmark,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            IsDefault = dto.IsDefault || !hasAddresses
        };

        // If this is set as default, unset other defaults
        if (address.IsDefault)
        {
            await UnsetDefaultAddresses(userId);
        }

        _context.UserAddresses.Add(address);
        await _context.SaveChangesAsync();

        return ApiResponse<UserAddressDto>.SuccessResponse(MapToDto(address));
    }

    public async Task<ApiResponse<UserAddressDto>> UpdateAddressAsync(int userId, int addressId, UpdateAddressDto dto)
    {
        var address = await _context.UserAddresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        if (address == null)
        {
            return ApiResponse<UserAddressDto>.ErrorResponse("Address not found");
        }

        address.Label = dto.Label;
        address.AddressLine = dto.AddressLine;
        address.BuildingName = dto.BuildingName;
        address.Floor = dto.Floor;
        address.Apartment = dto.Apartment;
        address.Landmark = dto.Landmark;
        address.Latitude = dto.Latitude;
        address.Longitude = dto.Longitude;

        if (dto.IsDefault && !address.IsDefault)
        {
            await UnsetDefaultAddresses(userId);
            address.IsDefault = true;
        }

        await _context.SaveChangesAsync();

        return ApiResponse<UserAddressDto>.SuccessResponse(MapToDto(address));
    }

    public async Task<ApiResponse> DeleteAddressAsync(int userId, int addressId)
    {
        var address = await _context.UserAddresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        if (address == null)
        {
            return ApiResponse.ErrorResponse("Address not found");
        }

        _context.UserAddresses.Remove(address);
        await _context.SaveChangesAsync();

        // If deleted address was default, set another as default
        if (address.IsDefault)
        {
            var firstAddress = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.UserId == userId);
            if (firstAddress != null)
            {
                firstAddress.IsDefault = true;
                await _context.SaveChangesAsync();
            }
        }

        return ApiResponse.SuccessResponse("Address deleted");
    }

    public async Task<ApiResponse> SetDefaultAddressAsync(int userId, int addressId)
    {
        var address = await _context.UserAddresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        if (address == null)
        {
            return ApiResponse.ErrorResponse("Address not found");
        }

        await UnsetDefaultAddresses(userId);
        address.IsDefault = true;
        await _context.SaveChangesAsync();

        return ApiResponse.SuccessResponse("Default address updated");
    }

    private async Task UnsetDefaultAddresses(int userId)
    {
        var defaultAddresses = await _context.UserAddresses
            .Where(a => a.UserId == userId && a.IsDefault)
            .ToListAsync();

        foreach (var addr in defaultAddresses)
        {
            addr.IsDefault = false;
        }
    }

    private static UserAddressDto MapToDto(UserAddress address)
    {
        return new UserAddressDto(
            address.Id,
            address.Label,
            address.AddressLine,
            address.BuildingName,
            address.Floor,
            address.Apartment,
            address.Landmark,
            address.Latitude,
            address.Longitude,
            address.IsDefault
        );
    }
}
