using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Application.DTOs.Loyalty;
using RestaurantApp.Application.Interfaces;
using System.Security.Claims;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoyaltyController : ControllerBase
{
    private readonly ILoyaltyService _loyaltyService;

    public LoyaltyController(ILoyaltyService loyaltyService)
    {
        _loyaltyService = loyaltyService;
    }

    /// <summary>
    /// Get current user's loyalty points
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPoints()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(customerId))
        {
            return Unauthorized();
        }

        var result = await _loyaltyService.GetPointsAsync(customerId);
        return Ok(result);
    }

    /// <summary>
    /// Get loyalty points for a specific user (for testing/demo)
    /// </summary>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> GetUserPoints(string userId)
    {
        var result = await _loyaltyService.GetPointsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Get transaction history
    /// </summary>
    [HttpGet("history")]
    [Authorize]
    public async Task<IActionResult> GetHistory([FromQuery] int? limit = 20)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(customerId))
        {
            return Unauthorized();
        }

        var result = await _loyaltyService.GetTransactionHistoryAsync(customerId, limit);
        return Ok(result);
    }

    /// <summary>
    /// Redeem points for a discount
    /// </summary>
    [HttpPost("redeem")]
    [Authorize]
    public async Task<IActionResult> RedeemPoints([FromBody] RedeemPointsDto dto)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(customerId))
        {
            return Unauthorized();
        }

        var result = await _loyaltyService.RedeemPointsAsync(customerId, dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get loyalty tier information
    /// </summary>
    [HttpGet("tiers")]
    public async Task<IActionResult> GetTiers()
    {
        var result = await _loyaltyService.GetTiersAsync();
        return Ok(result);
    }

    /// <summary>
    /// Calculate discount for a given number of points
    /// </summary>
    [HttpGet("calculate-discount")]
    public IActionResult CalculateDiscount([FromQuery] int points)
    {
        var discount = _loyaltyService.CalculateDiscount(points);
        return Ok(new { points, discountAmount = discount, message = $"{points} points = {discount:C} discount" });
    }

    // Admin endpoints

    /// <summary>
    /// Award points to a user (admin)
    /// </summary>
    [HttpPost("award")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AwardPoints([FromBody] AwardPointsRequest request)
    {
        var result = await _loyaltyService.AddBonusPointsAsync(
            request.CustomerId, 
            request.Points, 
            request.Description ?? "Admin bonus points"
        );

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Award points for an order (called after order completion)
    /// </summary>
    [HttpPost("award-order")]
    public async Task<IActionResult> AwardOrderPoints([FromBody] AwardOrderPointsRequest request)
    {
        var result = await _loyaltyService.AwardPointsAsync(
            request.CustomerId,
            request.OrderId,
            request.OrderTotal
        );

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get all customers with loyalty points (admin)
    /// </summary>
    [HttpGet("customers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? tier = null)
    {
        var result = await _loyaltyService.GetCustomersAsync(page, pageSize, search, tier);
        return Ok(result);
    }
}

public record AwardPointsRequest(
    string CustomerId,
    int Points,
    string? Description
);

public record AwardOrderPointsRequest(
    string CustomerId,
    int OrderId,
    decimal OrderTotal
);
