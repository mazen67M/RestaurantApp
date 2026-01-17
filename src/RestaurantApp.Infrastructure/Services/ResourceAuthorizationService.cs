using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Services;

public class ResourceAuthorizationService : IResourceAuthorizationService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ResourceAuthorizationService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> CanAccessUserAsync(int currentUserId, int targetUserId)
    {
        // User can access their own data
        if (currentUserId == targetUserId)
            return true;

        // SuperAdmin can access all users
        return await IsSuperAdminAsync(currentUserId);
    }

    public async Task<bool> CanAccessOrderAsync(int currentUserId, int orderId)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            return false;

        // User can access their own orders
        if (order.UserId == currentUserId)
            return true;

        // Admin or SuperAdmin can access all orders
        return await IsAdminAsync(currentUserId);
    }

    public async Task<bool> CanModifyOrderAsync(int currentUserId, int orderId)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            return false;

        // Only admins can modify orders (customers cannot modify after creation)
        return await IsAdminAsync(currentUserId);
    }

    public async Task<bool> CanAccessBranchAsync(int currentUserId, int branchId)
    {
        // SuperAdmin can access all branches
        if (await IsSuperAdminAsync(currentUserId))
            return true;

        // TODO: Implement branch assignment for admins
        // For now, regular admins can access all branches
        // In production, you should implement a BranchAdmin table
        // to map which admins can access which branches
        return await IsAdminAsync(currentUserId);
    }

    public async Task<bool> IsSuperAdminAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;

        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains("SuperAdmin");
    }

    public async Task<bool> IsAdminAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;

        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains("Admin") || roles.Contains("SuperAdmin");
    }
}
