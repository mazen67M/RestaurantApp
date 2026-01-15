using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Application.DTOs.Review;
using RestaurantApp.Application.Interfaces;
using System.Security.Claims;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Get all approved reviews for a menu item
    /// </summary>
    [HttpGet("item/{menuItemId}")]
    public async Task<IActionResult> GetItemReviews(int menuItemId)
    {
        var result = await _reviewService.GetItemReviewsAsync(menuItemId);
        return Ok(result);
    }

    /// <summary>
    /// Get review summary (average rating, counts) for a menu item
    /// </summary>
    [HttpGet("item/{menuItemId}/summary")]
    public async Task<IActionResult> GetItemReviewSummary(int menuItemId)
    {
        var result = await _reviewService.GetItemReviewSummaryAsync(menuItemId);
        return Ok(result);
    }

    /// <summary>
    /// Get current user's reviews
    /// </summary>
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyReviews()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(customerId))
        {
            return Unauthorized();
        }

        var result = await _reviewService.GetMyReviewsAsync(customerId);
        return Ok(result);
    }

    /// <summary>
    /// Create a new review
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(customerId))
        {
            return Unauthorized();
        }

        var result = await _reviewService.CreateReviewAsync(customerId, dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetItemReviews), new { menuItemId = dto.MenuItemId }, result);
    }

    /// <summary>
    /// Update an existing review
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDto dto)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(customerId))
        {
            return Unauthorized();
        }

        var result = await _reviewService.UpdateReviewAsync(customerId, id, dto);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete a review
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(customerId))
        {
            return Unauthorized();
        }

        var result = await _reviewService.DeleteReviewAsync(customerId, id);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Check if user can review a specific item from an order
    /// </summary>
    [HttpGet("can-review")]
    [Authorize]
    public async Task<IActionResult> CanReview([FromQuery] int orderId, [FromQuery] int menuItemId)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(customerId))
        {
            return Unauthorized();
        }

        var result = await _reviewService.CanReviewItemAsync(customerId, orderId, menuItemId);
        return Ok(result);
    }

    // Admin endpoints
    
    /// <summary>
    /// Get all reviews with pagination and filtering (admin)
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllReviews(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        var result = await _reviewService.GetAllReviewsAsync(page, pageSize, status);
        return Ok(result);
    }
    
    /// <summary>
    /// Get pending reviews for moderation
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingReviews()
    {
        var result = await _reviewService.GetPendingReviewsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Approve a review
    /// </summary>
    [HttpPatch("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveReview(int id)
    {
        var result = await _reviewService.ApproveReviewAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Reject a review
    /// </summary>
    [HttpPatch("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectReview(int id)
    {
        var result = await _reviewService.RejectReviewAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Toggle review visibility
    /// </summary>
    [HttpPatch("{id}/toggle-visibility")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleVisibility(int id)
    {
        var result = await _reviewService.ToggleReviewVisibilityAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
