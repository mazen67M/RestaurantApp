using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Application.DTOs.Menu;
using RestaurantApp.Application.Interfaces;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;
    private readonly RestaurantApp.API.Services.IAdminNotificationService _notificationService;

    public MenuController(
        IMenuService menuService,
        RestaurantApp.API.Services.IAdminNotificationService notificationService)
    {
        _menuService = menuService;
        _notificationService = notificationService;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _menuService.GetCategoriesAsync();
        return Ok(result);
    }

    [HttpGet("categories/{id}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var result = await _menuService.GetCategoryAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpGet("categories/{categoryId}/items")]
    public async Task<IActionResult> GetItemsByCategory(int categoryId)
    {
        var result = await _menuService.GetItemsByCategoryAsync(categoryId);
        return Ok(result);
    }

    /// <summary>
    /// Get all menu items (for admin dashboard)
    /// </summary>
    [HttpGet("items")]
    public async Task<IActionResult> GetAllItems(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _menuService.GetAllItemsAsync(page, pageSize);
        return Ok(result);
    }


    [HttpGet("items/{id}")]
    public async Task<IActionResult> GetItem(int id)
    {
        var result = await _menuService.GetItemAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchItems([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest("Search query required");
        }
        var result = await _menuService.SearchItemsAsync(q);
        return Ok(result);
    }

    /// <summary>
    /// Filter menu items by price, rating, category, etc.
    /// </summary>
    [HttpGet("items/filter")]
    public async Task<IActionResult> FilterItems(
        [FromQuery] int? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] double? minRating = null,
        [FromQuery] bool? isAvailable = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _menuService.FilterItemsAsync(
            categoryId, 
            minPrice, 
            maxPrice, 
            minRating, 
            isAvailable,
            page,
            pageSize);
        return Ok(result);
    }

    [HttpGet("popular")]
    public async Task<IActionResult> GetPopularItems([FromQuery] int count = 10)
    {
        var result = await _menuService.GetPopularItemsAsync(count);
        return Ok(result);
    }

    // Admin endpoints - Authorization temporarily disabled for development
    [Authorize(Roles = "Admin")]
    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateMenuCategoryDto dto)
    {
        var result = await _menuService.CreateCategoryAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        await _notificationService.NotifyCategoryUpdated(result.Data!.Id, "created");
        return CreatedAtAction(nameof(GetCategory), new { id = result.Data!.Id }, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateMenuCategoryDto dto)
    {
        var result = await _menuService.UpdateCategoryAsync(id, dto);
        if (!result.Success)
        {
            return NotFound(result);
        }
        await _notificationService.NotifyCategoryUpdated(id, "updated");
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _menuService.DeleteCategoryAsync(id);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("items")]
    public async Task<IActionResult> CreateItem([FromBody] CreateMenuItemDto dto)
    {
        var result = await _menuService.CreateItemAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        await _notificationService.NotifyMenuUpdated(result.Data!.Id, "created");
        return CreatedAtAction(nameof(GetItem), new { id = result.Data!.Id }, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateMenuItemDto dto)
    {
        var result = await _menuService.UpdateItemAsync(id, dto);
        if (!result.Success)
        {
            return NotFound(result);
        }
        await _notificationService.NotifyMenuUpdated(id, "updated");
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("items/{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var result = await _menuService.DeleteItemAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [Authorize(Roles = "Admin,Cashier")]
    [HttpPost("items/{id}/toggle-availability")]
    public async Task<IActionResult> ToggleItemAvailability(int id)
    {
        var result = await _menuService.ToggleItemAvailabilityAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
