using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RestaurantApp.Application.DTOs.Delivery;
using RestaurantApp.Application.Interfaces;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;

    public DeliveriesController(IDeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    /// <summary>
    /// Get all deliveries (drivers)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _deliveryService.GetAllDeliveriesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get available deliveries (drivers)
    /// </summary>
    [HttpGet("available")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAvailable()
    {
        var result = await _deliveryService.GetAvailableDeliveriesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get delivery by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _deliveryService.GetDeliveryByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Create a new delivery (driver)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryDto dto)
    {
        var result = await _deliveryService.CreateDeliveryAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Update a delivery (driver)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDeliveryDto dto)
    {
        var result = await _deliveryService.UpdateDeliveryAsync(id, dto);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Delete a delivery (driver) - soft delete
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _deliveryService.DeleteDeliveryAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Get delivery statistics
    /// </summary>
    [HttpGet("{id}/stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStats(int id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var result = await _deliveryService.GetDeliveryStatsAsync(id, startDate, endDate);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Get all delivery statistics for reports
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var result = await _deliveryService.GetAllDeliveryStatsAsync(startDate, endDate);
        return Ok(result);
    }

    /// <summary>
    /// Set delivery availability
    /// </summary>
    [HttpPost("{id}/availability")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetAvailability(int id, [FromBody] bool isAvailable)
    {
        var result = await _deliveryService.SetDeliveryAvailabilityAsync(id, isAvailable);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
