namespace RestaurantApp.Domain.Entities;

/// <summary>
/// Customer's favorite menu items (wishlist)
/// </summary>
public class Favorite
{
    public int Id { get; set; }
    
    /// <summary>
    /// The customer who favorited the item
    /// </summary>
    public string CustomerId { get; set; } = null!;
    public ApplicationUser Customer { get; set; } = null!;
    
    /// <summary>
    /// The favorited menu item
    /// </summary>
    public int MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
