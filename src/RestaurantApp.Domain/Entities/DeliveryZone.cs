namespace RestaurantApp.Domain.Entities;

/// <summary>
/// Represents a delivery zone with pricing for a branch
/// </summary>
public class DeliveryZone : BaseEntity
{
    /// <summary>
    /// Branch this delivery zone belongs to
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// Navigation property to branch
    /// </summary>
    public virtual Branch Branch { get; set; } = null!;

    /// <summary>
    /// Arabic name of the zone
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// English name of the zone
    /// </summary>
    public string NameEn { get; set; } = string.Empty;

    /// <summary>
    /// Minimum distance in kilometers for this zone
    /// </summary>
    public decimal MinDistanceKm { get; set; }

    /// <summary>
    /// Maximum distance in kilometers for this zone
    /// </summary>
    public decimal MaxDistanceKm { get; set; }

    /// <summary>
    /// Delivery fee for this zone
    /// </summary>
    public decimal DeliveryFee { get; set; }

    /// <summary>
    /// Minimum order amount for free delivery (null = no free delivery)
    /// </summary>
    public decimal? MinimumOrderForFreeDelivery { get; set; }

    /// <summary>
    /// Estimated delivery time in minutes
    /// </summary>
    public int EstimatedMinutes { get; set; }

    /// <summary>
    /// Whether this zone is currently active for deliveries
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Priority for zone matching (lower = higher priority)
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Calculate the delivery fee for an order
    /// </summary>
    public decimal CalculateDeliveryFee(decimal orderTotal)
    {
        if (!IsActive)
            return 0;

        // Free delivery if order exceeds minimum
        if (MinimumOrderForFreeDelivery.HasValue && orderTotal >= MinimumOrderForFreeDelivery.Value)
            return 0;

        return DeliveryFee;
    }
}

/// <summary>
/// Service for calculating delivery fees based on distance
/// </summary>
public static class DeliveryFeeCalculator
{
    /// <summary>
    /// Calculate the straight-line distance between two coordinates in kilometers
    /// Uses the Haversine formula
    /// </summary>
    public static double CalculateDistance(
        double lat1, double lon1,
        double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371;

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;

    /// <summary>
    /// Find the appropriate delivery zone based on distance
    /// </summary>
    public static DeliveryZone? FindZone(
        IEnumerable<DeliveryZone> zones,
        double distanceKm)
    {
        return zones
            .Where(z => z.IsActive)
            .Where(z => (decimal)distanceKm >= z.MinDistanceKm && (decimal)distanceKm <= z.MaxDistanceKm)
            .OrderBy(z => z.SortOrder)
            .FirstOrDefault();
    }
}
