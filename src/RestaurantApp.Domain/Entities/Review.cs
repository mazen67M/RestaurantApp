namespace RestaurantApp.Domain.Entities;

/// <summary>
/// Customer review for a menu item (associated with an order)
/// </summary>
public class Review
{
    public int Id { get; set; }
    
    /// <summary>
    /// The customer who wrote the review
    /// </summary>
    public string CustomerId { get; set; } = null!;
    public ApplicationUser Customer { get; set; } = null!;
    
    /// <summary>
    /// The order this review is for (to verify purchase)
    /// </summary>
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    
    /// <summary>
    /// The specific menu item being reviewed
    /// </summary>
    public int MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; } = null!;
    
    /// <summary>
    /// Rating from 1-5 stars
    /// </summary>
    public int Rating { get; set; }
    
    /// <summary>
    /// Optional review comment
    /// </summary>
    public string? Comment { get; set; }
    
    /// <summary>
    /// Customer name (cached for display)
    /// </summary>
    public string CustomerName { get; set; } = "";
    
    /// <summary>
    /// Admin approval status
    /// </summary>
    public bool IsApproved { get; set; } = false;
    
    /// <summary>
    /// Whether review is visible to customers
    /// </summary>
    public bool IsVisible { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
