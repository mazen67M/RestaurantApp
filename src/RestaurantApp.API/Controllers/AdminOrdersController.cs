using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Enums;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IDeliveryService _deliveryService;
    private readonly IResourceAuthorizationService _authService;
    private readonly ILogger<AdminOrdersController> _logger;

    public AdminOrdersController(
        IOrderService orderService,
        IDeliveryService deliveryService,
        IResourceAuthorizationService authService,
        ILogger<AdminOrdersController> logger)
    {
        _orderService = orderService;
        _deliveryService = deliveryService;
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Get all orders for admin dashboard
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllOrders(
        [FromQuery] string? status = null,
        [FromQuery] int? branchId = null,
        [FromQuery] DateTime? date = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            OrderStatus? orderStatus = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var parsedStatus))
            {
                orderStatus = parsedStatus;
            }

            var result = await _orderService.GetOrdersAsync(
                branchId: branchId,
                status: orderStatus,
                fromDate: date,
                toDate: date?.AddDays(1),
                page: page,
                pageSize: pageSize);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all orders");
            return StatusCode(500, new { success = false, message = "Error fetching orders" });
        }
    }

    // Update order status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (!await _authService.CanModifyOrderAsync(userId, id))
            {
                return Forbid();
            }

            var result = await _orderService.UpdateOrderStatusAsync(id, request.Status);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status for order {OrderId}", id);
            return StatusCode(500, new { success = false, message = "Error updating order status" });
        }
    }

    // Get order details
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderDetails(int id)
    {
        try
        {
            var userId = GetUserId();
            if (!await _authService.CanAccessOrderAsync(userId, id))
            {
                return Forbid();
            }

            var result = await _orderService.GetOrderDetailsAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order details for order {OrderId}", id);
            return StatusCode(500, new { success = false, message = "Error fetching order details" });
        }
    }
    
    /// <summary>
    /// Assign a delivery driver to an order
    /// </summary>
    [HttpPost("{id}/assign-delivery")]
    public async Task<IActionResult> AssignDelivery(int id, [FromBody] AssignDeliveryRequest request)
    {
        try
        {
            var userId = GetUserId();
            if (!await _authService.CanModifyOrderAsync(userId, id))
            {
                return Forbid();
            }

            _logger.LogInformation("Assigning delivery {DeliveryId} to order {OrderId}", request.DeliveryId, id);
            
            var result = await _deliveryService.AssignDeliveryToOrderAsync(id, request.DeliveryId);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning delivery to order {OrderId}", id);
            return StatusCode(500, new { success = false, message = "Error assigning delivery" });
        }
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(userIdClaim) ? 0 : int.Parse(userIdClaim);
    }
}
