using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Application.DTOs.Restaurant;
namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRestaurant()
    {
        var result = await _restaurantService.GetRestaurantAsync();
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRestaurant([FromBody] UpdateRestaurantDto dto)
    {
        var result = await _restaurantService.UpdateRestaurantAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}

[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;

    public BranchesController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBranches([FromQuery] decimal? latitude, [FromQuery] decimal? longitude)
    {
        bool isAdmin = User.IsInRole("Admin");
        var result = await _restaurantService.GetBranchesAsync(latitude, longitude, isAdmin);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBranch(int id)
    {
        var result = await _restaurantService.GetBranchAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpGet("nearest")]
    public async Task<IActionResult> GetNearestBranch([FromQuery] decimal latitude, [FromQuery] decimal longitude)
    {
        var result = await _restaurantService.GetNearestBranchAsync(latitude, longitude);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    // Admin endpoints for branch management
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchDto dto)
    {
        var result = await _restaurantService.CreateBranchAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetBranch), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateBranch(int id, [FromBody] UpdateBranchDto dto)
    {
        var result = await _restaurantService.UpdateBranchAsync(id, dto);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBranch(int id)
    {
        var result = await _restaurantService.DeleteBranchAsync(id);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}

