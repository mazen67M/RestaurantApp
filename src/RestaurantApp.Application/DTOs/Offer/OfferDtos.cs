using System;

namespace RestaurantApp.Application.DTOs.Offer;

public class OfferDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public decimal? MaximumDiscount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public bool IsActive { get; set; }
    public string? BranchName { get; set; }
    public string? CategoryName { get; set; }
    public string? MenuItemName { get; set; }
}

public class OfferValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? OfferId { get; set; }
    public string? OfferCode { get; set; }
    public string? OfferType { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal? DiscountAmount { get; set; }
    public bool IsFreeDelivery { get; set; }
}

public class CreateOfferRequest
{
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public decimal? MaximumDiscount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? UsageLimit { get; set; }
    public int? PerUserLimit { get; set; }
    public bool IsActive { get; set; } = true;
    public int? BranchId { get; set; }
    public int? CategoryId { get; set; }
    public int? MenuItemId { get; set; }
}

public class UpdateOfferRequest : CreateOfferRequest
{
}
