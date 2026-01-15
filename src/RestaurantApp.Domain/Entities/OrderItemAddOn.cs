namespace RestaurantApp.Domain.Entities;

public class OrderItemAddOn : BaseEntity
{
    public int OrderItemId { get; set; }
    public int MenuItemAddOnId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // Navigation properties
    public virtual OrderItem OrderItem { get; set; } = null!;
    public virtual MenuItemAddOn MenuItemAddOn { get; set; } = null!;
}
