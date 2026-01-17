namespace RestaurantApp.Application.Interfaces;

/// <summary>
/// Service for resource-based authorization to prevent IDOR (Insecure Direct Object Reference) attacks
/// </summary>
public interface IResourceAuthorizationService
{
    /// <summary>
    /// Check if current user can access target user's data
    /// </summary>
    Task<bool> CanAccessUserAsync(int currentUserId, int targetUserId);

    /// <summary>
    /// Check if current user can access an order
    /// </summary>
    Task<bool> CanAccessOrderAsync(int currentUserId, int orderId);

    /// <summary>
    /// Check if current user can access a branch
    /// </summary>
    Task<bool> CanAccessBranchAsync(int currentUserId, int branchId);

    /// <summary>
    /// Check if current user can modify an order
    /// </summary>
    Task<bool> CanModifyOrderAsync(int currentUserId, int orderId);

    /// <summary>
    /// Check if current user is SuperAdmin
    /// </summary>
    Task<bool> IsSuperAdminAsync(int userId);

    /// <summary>
    /// Check if current user is Admin or SuperAdmin
    /// </summary>
    Task<bool> IsAdminAsync(int userId);
}
