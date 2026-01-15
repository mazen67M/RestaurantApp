using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Restaurant;

namespace RestaurantApp.Application.Interfaces;

public interface IRestaurantService
{
    Task<ApiResponse<RestaurantDto>> GetRestaurantAsync();
    Task<ApiResponse<RestaurantDto>> UpdateRestaurantAsync(UpdateRestaurantDto dto);

    Task<ApiResponse<List<BranchDto>>> GetBranchesAsync(decimal? latitude = null, decimal? longitude = null, bool includeInactive = false);
    Task<ApiResponse<BranchDto>> GetBranchAsync(int id);
    Task<ApiResponse<BranchDto>> GetNearestBranchAsync(decimal latitude, decimal longitude);
    
    // Admin branch management
    Task<ApiResponse<BranchDto>> CreateBranchAsync(CreateBranchDto dto);
    Task<ApiResponse<BranchDto>> UpdateBranchAsync(int id, UpdateBranchDto dto);
    Task<ApiResponse<bool>> DeleteBranchAsync(int id);
}

