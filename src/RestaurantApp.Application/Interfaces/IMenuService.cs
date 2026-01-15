using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Menu;

namespace RestaurantApp.Application.Interfaces;

public interface IMenuService
{
    Task<ApiResponse<List<MenuCategoryDto>>> GetCategoriesAsync(bool includeInactive = false);
    Task<ApiResponse<MenuCategoryDto>> GetCategoryAsync(int id);
    Task<ApiResponse<List<MenuItemSummaryDto>>> GetItemsByCategoryAsync(int categoryId);
    Task<ApiResponse<PagedResponse<MenuItemSummaryDto>>> GetAllItemsAsync(int page = 1, int pageSize = 50);
    Task<ApiResponse<MenuItemDto>> GetItemAsync(int id);
    Task<ApiResponse<List<MenuItemSummaryDto>>> SearchItemsAsync(string query);
    Task<ApiResponse<List<MenuItemSummaryDto>>> GetPopularItemsAsync(int count = 10);
    Task<ApiResponse<PagedResponse<MenuItemSummaryDto>>> FilterItemsAsync(
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        double? minRating = null,
        bool? isAvailable = null,
        int page = 1,
        int pageSize = 50);
    
    // Admin operations
    Task<ApiResponse<MenuCategoryDto>> CreateCategoryAsync(CreateMenuCategoryDto dto);
    Task<ApiResponse<MenuCategoryDto>> UpdateCategoryAsync(int id, UpdateMenuCategoryDto dto);
    Task<ApiResponse> DeleteCategoryAsync(int id);
    Task<ApiResponse<MenuItemDto>> CreateItemAsync(CreateMenuItemDto dto);
    Task<ApiResponse<MenuItemDto>> UpdateItemAsync(int id, UpdateMenuItemDto dto);
    Task<ApiResponse> DeleteItemAsync(int id);
    Task<ApiResponse> ToggleItemAvailabilityAsync(int id);
}
