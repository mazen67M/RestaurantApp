namespace RestaurantApp.Domain.Entities;

/// <summary>
/// Customer loyalty points balance and tier
/// </summary>
public class LoyaltyPoints
{
    public int Id { get; set; }
    
    /// <summary>
    /// The customer who owns these points
    /// </summary>
    public string CustomerId { get; set; } = null!;
    public ApplicationUser Customer { get; set; } = null!;
    
    /// <summary>
    /// Current available points balance
    /// </summary>
    public int Points { get; set; } = 0;
    
    /// <summary>
    /// Total points ever earned
    /// </summary>
    public int TotalEarned { get; set; } = 0;
    
    /// <summary>
    /// Total points ever redeemed
    /// </summary>
    public int TotalRedeemed { get; set; } = 0;
    
    /// <summary>
    /// Current loyalty tier: Bronze, Silver, Gold, Platinum
    /// </summary>
    public string Tier { get; set; } = "Bronze";
    
    /// <summary>
    /// Transaction history
    /// </summary>
    public ICollection<LoyaltyTransaction> Transactions { get; set; } = new List<LoyaltyTransaction>();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Calculate tier based on total earned points
    /// </summary>
    public void UpdateTier()
    {
        Tier = TotalEarned switch
        {
            >= 10000 => "Platinum",  // 10,000+ AED spent
            >= 5000 => "Gold",       // 5,000+ AED spent
            >= 1000 => "Silver",     // 1,000+ AED spent
            _ => "Bronze"
        };
    }
    
    /// <summary>
    /// Get bonus multiplier for tier
    /// </summary>
    public decimal GetBonusMultiplier()
    {
        return Tier switch
        {
            "Platinum" => 2.0m,  // 2x points
            "Gold" => 1.5m,      // 1.5x points
            "Silver" => 1.25m,   // 1.25x points
            _ => 1.0m            // Normal points
        };
    }
}

/// <summary>
/// Individual loyalty point transaction (earn or redeem)
/// </summary>
public class LoyaltyTransaction
{
    public int Id { get; set; }
    
    public int LoyaltyPointsId { get; set; }
    public LoyaltyPoints LoyaltyPoints { get; set; } = null!;
    
    /// <summary>
    /// Related order (if applicable)
    /// </summary>
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
    
    /// <summary>
    /// Amount of points (positive for earned, negative for redeemed)
    /// </summary>
    public int Points { get; set; }
    
    /// <summary>
    /// Type: Earned, Redeemed, Expired, Bonus, Adjustment
    /// </summary>
    public string TransactionType { get; set; } = "Earned";
    
    /// <summary>
    /// Description of the transaction
    /// </summary>
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
