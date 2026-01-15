using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HealthController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Health check endpoint - returns API status and database connectivity
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        var health = new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            database = await CheckDatabaseHealth()
        };

        return Ok(health);
    }

    private async Task<object> CheckDatabaseHealth()
    {
        try
        {
            // Simple query to check database connectivity
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (canConnect)
            {
                // Get some basic stats
                var restaurantCount = await _context.Restaurants.CountAsync();
                var branchCount = await _context.Branches.CountAsync();
                var menuItemCount = await _context.MenuItems.CountAsync();

                return new
                {
                    status = "connected",
                    restaurants = restaurantCount,
                    branches = branchCount,
                    menuItems = menuItemCount
                };
            }
            else
            {
                return new { status = "disconnected" };
            }
        }
        catch (Exception ex)
        {
            return new
            {
                status = "error",
                message = ex.Message
            };
        }
    }
}
