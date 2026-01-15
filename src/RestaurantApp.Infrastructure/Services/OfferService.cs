using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Offer;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApp.Infrastructure.Services;

public class OfferService : IOfferService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OfferService> _logger;

    public OfferService(ApplicationDbContext context, ILogger<OfferService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResponse<OfferDto>>> GetOffersAsync(int page = 1, int pageSize = 20)
    {
        var query = _context.Offers
            .Include(o => o.Branch)
            .Include(o => o.Category)
            .Include(o => o.MenuItem)
            .AsNoTracking();

        var totalCount = await query.CountAsync();

        var offers = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OfferDto
            {
                Id = o.Id,
                Code = o.Code,
                NameAr = o.NameAr,
                NameEn = o.NameEn,
                DescriptionAr = o.DescriptionAr,
                DescriptionEn = o.DescriptionEn,
                Type = o.Type.ToString(),
                Value = o.Value,
                MinimumOrderAmount = o.MinimumOrderAmount,
                MaximumDiscount = o.MaximumDiscount,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                UsageLimit = o.UsageLimit,
                UsageCount = o.UsageCount,
                IsActive = o.IsActive,
                BranchName = o.Branch != null ? o.Branch.NameEn : null,
                CategoryName = o.Category != null ? o.Category.NameEn : null,
                MenuItemName = o.MenuItem != null ? o.MenuItem.NameEn : null
            })
            .ToListAsync();

        var pagedResponse = new PagedResponse<OfferDto>
        {
            Items = offers,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return ApiResponse<PagedResponse<OfferDto>>.SuccessResponse(pagedResponse);
    }

    public async Task<ApiResponse<List<OfferDto>>> GetActiveOffersAsync()
    {
        var now = DateTime.UtcNow;
        var offers = await _context.Offers
            .Where(o => o.IsActive && o.StartDate <= now && o.EndDate >= now)
            .Where(o => o.UsageLimit == null || o.UsageCount < o.UsageLimit)
            .Select(o => new OfferDto
            {
                Id = o.Id,
                Code = o.Code,
                NameAr = o.NameAr,
                NameEn = o.NameEn,
                DescriptionAr = o.DescriptionAr,
                DescriptionEn = o.DescriptionEn,
                Type = o.Type.ToString(),
                Value = o.Value,
                MinimumOrderAmount = o.MinimumOrderAmount,
                MaximumDiscount = o.MaximumDiscount,
                StartDate = o.StartDate,
                EndDate = o.EndDate
            })
            .ToListAsync();

        return ApiResponse<List<OfferDto>>.SuccessResponse(offers);
    }

    public async Task<ApiResponse<OfferValidationResult>> ValidateCouponAsync(string code, decimal orderTotal, int? branchId = null, int? categoryId = null)
    {
        var now = DateTime.UtcNow;
        var offer = await _context.Offers
            .FirstOrDefaultAsync(o => o.Code.ToLower() == code.ToLower());

        if (offer == null)
        {
            return ApiResponse<OfferValidationResult>.SuccessResponse(new OfferValidationResult
            {
                IsValid = false,
                Message = "Invalid coupon code"
            });
        }

        if (!offer.IsActive)
        {
            return ApiResponse<OfferValidationResult>.SuccessResponse(new OfferValidationResult
            {
                IsValid = false,
                Message = "This coupon is no longer active"
            });
        }

        if (now < offer.StartDate)
        {
            return ApiResponse<OfferValidationResult>.SuccessResponse(new OfferValidationResult
            {
                IsValid = false,
                Message = "This coupon is not yet active"
            });
        }

        if (now > offer.EndDate)
        {
            return ApiResponse<OfferValidationResult>.SuccessResponse(new OfferValidationResult
            {
                IsValid = false,
                Message = "This coupon has expired"
            });
        }

        if (offer.UsageLimit.HasValue && offer.UsageCount >= offer.UsageLimit.Value)
        {
            return ApiResponse<OfferValidationResult>.SuccessResponse(new OfferValidationResult
            {
                IsValid = false,
                Message = "This coupon has reached its usage limit"
            });
        }

        if (offer.MinimumOrderAmount.HasValue && orderTotal < offer.MinimumOrderAmount.Value)
        {
            return ApiResponse<OfferValidationResult>.SuccessResponse(new OfferValidationResult
            {
                IsValid = false,
                Message = $"Minimum order amount is {offer.MinimumOrderAmount.Value:C}"
            });
        }

        if (offer.BranchId.HasValue && branchId.HasValue && offer.BranchId != branchId)
        {
            return ApiResponse<OfferValidationResult>.SuccessResponse(new OfferValidationResult
            {
                IsValid = false,
                Message = "This coupon is not valid for the selected branch"
            });
        }

        if (offer.CategoryId.HasValue && categoryId.HasValue && offer.CategoryId != categoryId)
        {
            return ApiResponse<OfferValidationResult>.SuccessResponse(new OfferValidationResult
            {
                IsValid = false,
                Message = "This coupon is not valid for items in your cart"
            });
        }

        var discount = offer.CalculateDiscount(orderTotal);

        return ApiResponse<OfferValidationResult>.SuccessResponse(new OfferValidationResult
        {
            IsValid = true,
            Message = "Coupon applied successfully!",
            OfferId = offer.Id,
            OfferCode = offer.Code,
            OfferType = offer.Type.ToString(),
            DiscountValue = offer.Value,
            DiscountAmount = discount,
            IsFreeDelivery = offer.Type == OfferType.FreeDelivery
        });
    }

    public async Task<ApiResponse<OfferDto>> CreateOfferAsync(CreateOfferRequest request)
    {
        if (await _context.Offers.AnyAsync(o => o.Code.ToLower() == request.Code.ToLower()))
        {
            return ApiResponse<OfferDto>.ErrorResponse("Coupon code already exists");
        }

        if (!Enum.TryParse<OfferType>(request.Type, out var offerType))
        {
            return ApiResponse<OfferDto>.ErrorResponse("Invalid offer type");
        }

        var offer = new Offer
        {
            Code = request.Code.ToUpper(),
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            Type = offerType,
            Value = request.Value,
            MinimumOrderAmount = request.MinimumOrderAmount,
            MaximumDiscount = request.MaximumDiscount,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            UsageLimit = request.UsageLimit,
            PerUserLimit = request.PerUserLimit,
            IsActive = request.IsActive,
            BranchId = request.BranchId,
            CategoryId = request.CategoryId,
            MenuItemId = request.MenuItemId
        };

        _context.Offers.Add(offer);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created offer: {Code}", offer.Code);

        var dto = new OfferDto
        {
            Id = offer.Id,
            Code = offer.Code,
            NameAr = offer.NameAr,
            NameEn = offer.NameEn,
            Type = offer.Type.ToString(),
            Value = offer.Value,
            IsActive = offer.IsActive
        };

        return ApiResponse<OfferDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponse> UpdateOfferAsync(int id, UpdateOfferRequest request)
    {
        var offer = await _context.Offers.FindAsync(id);
        if (offer == null)
            return ApiResponse.ErrorResponse("Offer not found");

        if (!Enum.TryParse<OfferType>(request.Type, out var offerType))
        {
            return ApiResponse.ErrorResponse("Invalid offer type");
        }

        offer.NameAr = request.NameAr;
        offer.NameEn = request.NameEn;
        offer.DescriptionAr = request.DescriptionAr;
        offer.DescriptionEn = request.DescriptionEn;
        offer.Type = offerType;
        offer.Value = request.Value;
        offer.MinimumOrderAmount = request.MinimumOrderAmount;
        offer.MaximumDiscount = request.MaximumDiscount;
        offer.StartDate = request.StartDate;
        offer.EndDate = request.EndDate;
        offer.UsageLimit = request.UsageLimit;
        offer.PerUserLimit = request.PerUserLimit;
        offer.IsActive = request.IsActive;
        offer.BranchId = request.BranchId;
        offer.CategoryId = request.CategoryId;
        offer.MenuItemId = request.MenuItemId;

        await _context.SaveChangesAsync();
        return ApiResponse.SuccessResponse("Offer updated successfully");
    }

    public async Task<ApiResponse> DeleteOfferAsync(int id)
    {
        var offer = await _context.Offers.FindAsync(id);
        if (offer == null)
            return ApiResponse.ErrorResponse("Offer not found");

        _context.Offers.Remove(offer);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted offer: {Code}", offer.Code);
        return ApiResponse.SuccessResponse("Offer deleted successfully");
    }

    public async Task<ApiResponse<bool>> ToggleOfferAsync(int id)
    {
        var offer = await _context.Offers.FindAsync(id);
        if (offer == null)
            return ApiResponse<bool>.ErrorResponse("Offer not found");

        offer.IsActive = !offer.IsActive;
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(offer.IsActive);
    }
}
