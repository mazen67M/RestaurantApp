using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Application.DTOs.Offer;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Application.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OffersController : ControllerBase
{
    private readonly IOfferService _offerService;

    public OffersController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    /// <summary>
    /// Get all offers (admin) with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PagedResponse<OfferDto>>>> GetOffers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _offerService.GetOffersAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get active offers for customers
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse<List<OfferDto>>>> GetActiveOffers()
    {
        var result = await _offerService.GetActiveOffersAsync();
        return Ok(result);
    }

    /// <summary>
    /// Validate a coupon code
    /// </summary>
    [HttpGet("validate/{code}")]
    public async Task<ActionResult<ApiResponse<OfferValidationResult>>> ValidateCoupon(
        string code, 
        [FromQuery] decimal orderTotal,
        [FromQuery] int? branchId = null,
        [FromQuery] int? categoryId = null)
    {
        var result = await _offerService.ValidateCouponAsync(code, orderTotal, branchId, categoryId);
        return Ok(result);
    }

    /// <summary>
    /// Create a new offer (admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<OfferDto>>> CreateOffer([FromBody] CreateOfferRequest request)
    {
        var result = await _offerService.CreateOfferAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetOffers), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Update an offer (admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOffer(int id, [FromBody] UpdateOfferRequest request)
    {
        var result = await _offerService.UpdateOfferAsync(id, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Delete an offer (admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOffer(int id)
    {
        var result = await _offerService.DeleteOfferAsync(id);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Toggle offer active status (admin)
    /// </summary>
    [HttpPatch("{id}/toggle")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleOffer(int id)
    {
        var result = await _offerService.ToggleOfferAsync(id);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
