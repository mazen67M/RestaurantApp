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

// Admin Orders Controller
[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin,Cashier")]
public class AdminOrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IDeliveryService _deliveryService;
    private readonly IResourceAuthorizationService _authService;

    public AdminOrdersController(
        IOrderService orderService,
        IDeliveryService deliveryService,
        IResourceAuthorizationService authService)
    {
        _orderService = orderService;
        _deliveryService = deliveryService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int? branchId,
        [FromQuery] OrderStatus? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _orderService.GetOrdersAsync(branchId, status, fromDate, toDate, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var result = await _orderService.GetOrderDetailsAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var result = await _orderService.UpdateOrderStatusAsync(id, request.Status);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Assign a delivery driver to an order
    /// </summary>
    [HttpPost("{id}/assign-delivery")]
    public async Task<IActionResult> AssignDelivery(int id, [FromBody] AssignDeliveryRequest request)
    {
        var result = await _deliveryService.AssignDeliveryToOrderAsync(id, request.DeliveryId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}

public record UpdateOrderStatusRequest(OrderStatus Status);
public record AssignDeliveryRequest(int DeliveryId);

