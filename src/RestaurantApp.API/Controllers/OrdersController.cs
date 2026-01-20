using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Application.DTOs.Order;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Enums;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IResourceAuthorizationService _authService;

    public OrdersController(
        IOrderService orderService,
        IResourceAuthorizationService authService)
    {
        _orderService = orderService;
        _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        var userId = GetUserId();
        var result = await _orderService.CreateOrderAsync(userId, dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetOrder), new { id = result.Data!.OrderId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = GetUserId();
        var result = await _orderService.GetUserOrdersAsync(userId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var userId = GetUserId();
        
        // IDOR Protection: Check if user can access this order
        if (!await _authService.CanAccessOrderAsync(userId, id))
        {
            return Forbid();
        }

        var result = await _orderService.GetOrderAsync(userId, id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpGet("{id}/track")]
    public async Task<IActionResult> TrackOrder(int id)
    {
        var userId = GetUserId();
        var result = await _orderService.GetOrderTrackingAsync(userId, id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderRequest request)
    {
        var userId = GetUserId();
        var result = await _orderService.CancelOrderAsync(userId, id, request.Reason);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Reorder a previous order (copy all items to new order)
    /// </summary>
    [HttpPost("{id}/reorder")]
    public async Task<IActionResult> ReorderOrder(int id)
    {
        var userId = GetUserId();
        var result = await _orderService.ReorderAsync(userId, id);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}

public record CancelOrderRequest(string Reason);
public record UpdateOrderStatusRequest(OrderStatus Status);
public record AssignDeliveryRequest(int DeliveryId);
