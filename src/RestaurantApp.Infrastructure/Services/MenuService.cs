using Microsoft.EntityFrameworkCore;
using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Menu;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Services;

public class MenuService : IMenuService
{
    private readonly ApplicationDbContext _context;

    public MenuService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<MenuCategoryDto>>> GetCategoriesAsync(bool includeInactive = false)
    {
        var query = _context.MenuCategories.AsQueryable();
        
        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        var categories = await query
            .Include(c => c.MenuItems)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        var dtos = categories.Select(c => new MenuCategoryDto(
            c.Id,
            c.NameAr,
            c.NameEn,
            c.DescriptionAr,
            c.DescriptionEn,
            c.ImageUrl,
            c.DisplayOrder,
            c.IsActive,
            c.MenuItems.Count(i => i.IsAvailable)
        )).ToList();

        return ApiResponse<List<MenuCategoryDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<MenuCategoryDto>> GetCategoryAsync(int id)
    {
        var category = await _context.MenuCategories
            .Include(c => c.MenuItems)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return ApiResponse<MenuCategoryDto>.ErrorResponse("Category not found");
        }

        var dto = new MenuCategoryDto(
            category.Id,
            category.NameAr,
            category.NameEn,
            category.DescriptionAr,
            category.DescriptionEn,
            category.ImageUrl,
            category.DisplayOrder,
            category.IsActive,
            category.MenuItems.Count(i => i.IsAvailable)
        );

        return ApiResponse<MenuCategoryDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<List<MenuItemSummaryDto>>> GetItemsByCategoryAsync(int categoryId)
    {
        var items = await _context.MenuItems
            .Where(i => i.CategoryId == categoryId && i.IsAvailable)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync();

        var dtos = items.Select(i => new MenuItemSummaryDto(
            i.Id,
            i.NameAr,
            i.NameEn,
            i.Price,
            i.DiscountedPrice,
            i.ImageUrl,
            i.IsAvailable,
            i.IsPopular
        )).ToList();

        return ApiResponse<List<MenuItemSummaryDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<PagedResponse<MenuItemSummaryDto>>> GetAllItemsAsync(int page = 1, int pageSize = 50)
    {
        var query = _context.MenuItems
            .Include(i => i.Category)
            .OrderBy(i => i.CategoryId)
            .ThenBy(i => i.DisplayOrder)
            .AsNoTracking();

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = items.Select(i => new MenuItemSummaryDto(
            i.Id,
            i.NameAr,
            i.NameEn,
            i.Price,
            i.DiscountedPrice,
            i.ImageUrl,
            i.IsAvailable,
            i.IsPopular,
            i.CategoryId,
            i.Category?.NameEn,
            i.DescriptionAr,
            i.DescriptionEn
        )).ToList();

        var pagedResponse = new PagedResponse<MenuItemSummaryDto>
        {
            Items = dtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return ApiResponse<PagedResponse<MenuItemSummaryDto>>.SuccessResponse(pagedResponse);
    }



    public async Task<ApiResponse<MenuItemDto>> GetItemAsync(int id)
    {
        var item = await _context.MenuItems
            .Include(i => i.AddOns)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            return ApiResponse<MenuItemDto>.ErrorResponse("Item not found");
        }

        var dto = new MenuItemDto(
            item.Id,
            item.CategoryId,
            item.NameAr,
            item.NameEn,
            item.DescriptionAr,
            item.DescriptionEn,
            item.Price,
            item.DiscountedPrice,
            item.ImageUrl,
            item.PreparationTimeMinutes,
            item.IsAvailable,
            item.IsPopular,
            item.Calories,
            item.AddOns.Where(a => a.IsAvailable).Select(a => new MenuItemAddOnDto(
                a.Id,
                a.NameAr,
                a.NameEn,
                a.Price,
                a.IsAvailable
            )).ToList()
        );

        return ApiResponse<MenuItemDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<List<MenuItemSummaryDto>>> SearchItemsAsync(string query)
    {
        var items = await _context.MenuItems
            .Where(i => i.IsAvailable && 
                       (i.NameAr.Contains(query) || 
                        i.NameEn.Contains(query) ||
                        (i.DescriptionAr != null && i.DescriptionAr.Contains(query)) ||
                        (i.DescriptionEn != null && i.DescriptionEn.Contains(query))))
            .Take(20)
            .ToListAsync();

        var dtos = items.Select(i => new MenuItemSummaryDto(
            i.Id,
            i.NameAr,
            i.NameEn,
            i.Price,
            i.DiscountedPrice,
            i.ImageUrl,
            i.IsAvailable,
            i.IsPopular
        )).ToList();

        return ApiResponse<List<MenuItemSummaryDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<List<MenuItemSummaryDto>>> GetPopularItemsAsync(int count = 10)
    {
        var items = await _context.MenuItems
            .Where(i => i.IsAvailable && i.IsPopular)
            .Take(count)
            .ToListAsync();

        var dtos = items.Select(i => new MenuItemSummaryDto(
            i.Id,
            i.NameAr,
            i.NameEn,
            i.Price,
            i.DiscountedPrice,
            i.ImageUrl,
            i.IsAvailable,
            i.IsPopular
        )).ToList();

        return ApiResponse<List<MenuItemSummaryDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<PagedResponse<MenuItemSummaryDto>>> FilterItemsAsync(
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        double? minRating = null,
        bool? isAvailable = null,
        int page = 1,
        int pageSize = 50)
    {
        var query = _context.MenuItems
            .Include(i => i.Category)
            .AsQueryable();

        // Apply filters
        if (categoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(i => (i.DiscountedPrice ?? i.Price) >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(i => (i.DiscountedPrice ?? i.Price) <= maxPrice.Value);
        }

        if (isAvailable.HasValue)
        {
            query = query.Where(i => i.IsAvailable == isAvailable.Value);
        }

        // TODO: Add rating filter when reviews are integrated
        // For now, we'll skip the rating filter as it requires joining with Reviews table

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(i => i.DisplayOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = items.Select(i => new MenuItemSummaryDto(
            i.Id,
            i.NameAr,
            i.NameEn,
            i.Price,
            i.DiscountedPrice,
            i.ImageUrl,
            i.IsAvailable,
            i.IsPopular,
            i.CategoryId,
            i.Category?.NameEn,
            i.DescriptionAr,
            i.DescriptionEn
        )).ToList();

        var pagedResponse = new PagedResponse<MenuItemSummaryDto>
        {
            Items = dtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return ApiResponse<PagedResponse<MenuItemSummaryDto>>.SuccessResponse(pagedResponse);
    }

    // Admin operations
    public async Task<ApiResponse<MenuCategoryDto>> CreateCategoryAsync(CreateMenuCategoryDto dto)
    {
        var restaurant = await _context.Restaurants.FirstOrDefaultAsync();
        if (restaurant == null)
        {
            return ApiResponse<MenuCategoryDto>.ErrorResponse("Restaurant not configured");
        }

        var category = new MenuCategory
        {
            RestaurantId = restaurant.Id,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            DescriptionAr = dto.DescriptionAr,
            DescriptionEn = dto.DescriptionEn,
            ImageUrl = dto.ImageUrl,
            DisplayOrder = dto.DisplayOrder
        };

        _context.MenuCategories.Add(category);
        await _context.SaveChangesAsync();

        return await GetCategoryAsync(category.Id);
    }

    public async Task<ApiResponse<MenuCategoryDto>> UpdateCategoryAsync(int id, UpdateMenuCategoryDto dto)
    {
        var category = await _context.MenuCategories.FindAsync(id);
        if (category == null)
        {
            return ApiResponse<MenuCategoryDto>.ErrorResponse("Category not found");
        }

        category.NameAr = dto.NameAr;
        category.NameEn = dto.NameEn;
        category.DescriptionAr = dto.DescriptionAr;
        category.DescriptionEn = dto.DescriptionEn;
        category.ImageUrl = dto.ImageUrl;
        category.DisplayOrder = dto.DisplayOrder;
        category.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return await GetCategoryAsync(id);
    }

    public async Task<ApiResponse> DeleteCategoryAsync(int id)
    {
        var category = await _context.MenuCategories
            .Include(c => c.MenuItems)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return ApiResponse.ErrorResponse("Category not found");
        }

        if (category.MenuItems.Any())
        {
            return ApiResponse.ErrorResponse("Cannot delete category with items. Remove items first.");
        }

        _context.MenuCategories.Remove(category);
        await _context.SaveChangesAsync();

        return ApiResponse.SuccessResponse("Category deleted");
    }

    public async Task<ApiResponse<MenuItemDto>> CreateItemAsync(CreateMenuItemDto dto)
    {
        var category = await _context.MenuCategories.FindAsync(dto.CategoryId);
        if (category == null)
        {
            return ApiResponse<MenuItemDto>.ErrorResponse("Category not found");
        }

        var item = new MenuItem
        {
            CategoryId = dto.CategoryId,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            DescriptionAr = dto.DescriptionAr,
            DescriptionEn = dto.DescriptionEn,
            Price = dto.Price,
            DiscountedPrice = dto.DiscountedPrice,
            ImageUrl = dto.ImageUrl,
            PreparationTimeMinutes = dto.PreparationTimeMinutes,
            Calories = dto.Calories
        };

        if (dto.AddOns?.Any() == true)
        {
            foreach (var addOnDto in dto.AddOns)
            {
                item.AddOns.Add(new MenuItemAddOn
                {
                    NameAr = addOnDto.NameAr,
                    NameEn = addOnDto.NameEn,
                    Price = addOnDto.Price
                });
            }
        }

        _context.MenuItems.Add(item);
        await _context.SaveChangesAsync();

        return await GetItemAsync(item.Id);
    }

    public async Task<ApiResponse<MenuItemDto>> UpdateItemAsync(int id, UpdateMenuItemDto dto)
    {
        var item = await _context.MenuItems.FindAsync(id);
        if (item == null)
        {
            return ApiResponse<MenuItemDto>.ErrorResponse("Item not found");
        }

        item.NameAr = dto.NameAr;
        item.NameEn = dto.NameEn;
        item.DescriptionAr = dto.DescriptionAr;
        item.DescriptionEn = dto.DescriptionEn;
        item.Price = dto.Price;
        item.DiscountedPrice = dto.DiscountedPrice;
        item.ImageUrl = dto.ImageUrl;
        item.PreparationTimeMinutes = dto.PreparationTimeMinutes;
        item.IsAvailable = dto.IsAvailable;
        item.IsPopular = dto.IsPopular;
        item.Calories = dto.Calories;

        await _context.SaveChangesAsync();

        return await GetItemAsync(id);
    }

    public async Task<ApiResponse> DeleteItemAsync(int id)
    {
        var item = await _context.MenuItems.FindAsync(id);
        if (item == null)
        {
            return ApiResponse.ErrorResponse("Item not found");
        }

        _context.MenuItems.Remove(item);
        await _context.SaveChangesAsync();

        return ApiResponse.SuccessResponse("Item deleted");
    }

    public async Task<ApiResponse> ToggleItemAvailabilityAsync(int id)
    {
        var item = await _context.MenuItems.FindAsync(id);
        if (item == null)
        {
            return ApiResponse.ErrorResponse("Item not found");
        }

        item.IsAvailable = !item.IsAvailable;
        await _context.SaveChangesAsync();

        return ApiResponse.SuccessResponse($"Item is now {(item.IsAvailable ? "available" : "unavailable")}");
    }
}
