using Microsoft.EntityFrameworkCore;
using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Loyalty;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Services;

public class LoyaltyService : ILoyaltyService
{
    private readonly ApplicationDbContext _context;
    
    // 1 AED spent = 1 point earned
    private const decimal PointsPerAed = 1m;
    // 100 points = 1 AED discount
    private const decimal PointsToAedRate = 0.01m;

    public LoyaltyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<LoyaltyPointsDto>> GetPointsAsync(string customerId)
    {
        var loyalty = await GetOrCreateLoyaltyPointsAsync(customerId);
        
        // Calculate points needed for next tier
        var (pointsToNext, nextTier) = CalculateNextTier(loyalty);

        var dto = new LoyaltyPointsDto(
            loyalty.Id,
            loyalty.Points,
            loyalty.TotalEarned,
            loyalty.TotalRedeemed,
            loyalty.Tier,
            loyalty.GetBonusMultiplier(),
            pointsToNext,
            nextTier
        );

        return ApiResponse<LoyaltyPointsDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<List<LoyaltyTransactionDto>>> GetTransactionHistoryAsync(string customerId, int? limit = 20)
    {
        var loyalty = await _context.LoyaltyPoints
            .Include(l => l.Transactions.OrderByDescending(t => t.CreatedAt).Take(limit ?? 20))
            .FirstOrDefaultAsync(l => l.CustomerId == customerId);

        if (loyalty == null)
        {
            return ApiResponse<List<LoyaltyTransactionDto>>.SuccessResponse(new List<LoyaltyTransactionDto>());
        }

        var transactions = loyalty.Transactions
            .Select(t => new LoyaltyTransactionDto(
                t.Id,
                t.Points,
                t.TransactionType,
                t.Description,
                t.OrderId,
                t.CreatedAt
            ))
            .ToList();

        return ApiResponse<List<LoyaltyTransactionDto>>.SuccessResponse(transactions);
    }

    public async Task<ApiResponse<int>> AwardPointsAsync(string customerId, int orderId, decimal orderTotal)
    {
        var loyalty = await GetOrCreateLoyaltyPointsAsync(customerId);
        
        // Calculate base points (1 point per AED)
        var basePoints = (int)Math.Floor(orderTotal * PointsPerAed);
        
        // Apply tier bonus
        var bonusMultiplier = loyalty.GetBonusMultiplier();
        var totalPoints = (int)Math.Floor(basePoints * bonusMultiplier);

        // Create transaction
        var transaction = new LoyaltyTransaction
        {
            LoyaltyPointsId = loyalty.Id,
            OrderId = orderId,
            Points = totalPoints,
            TransactionType = "Earned",
            Description = $"Points earned from order #{orderId} ({bonusMultiplier}x bonus)"
        };

        loyalty.Points += totalPoints;
        loyalty.TotalEarned += totalPoints;
        loyalty.UpdateTier();
        loyalty.UpdatedAt = DateTime.UtcNow;

        _context.LoyaltyTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return ApiResponse<int>.SuccessResponse(totalPoints, $"Earned {totalPoints} points!");
    }

    public async Task<ApiResponse<RedeemResultDto>> RedeemPointsAsync(string customerId, RedeemPointsDto dto)
    {
        if (dto.Points <= 0)
        {
            return ApiResponse<RedeemResultDto>.ErrorResponse("Points must be greater than 0");
        }

        var loyalty = await GetOrCreateLoyaltyPointsAsync(customerId);

        if (loyalty.Points < dto.Points)
        {
            return ApiResponse<RedeemResultDto>.ErrorResponse(
                $"Insufficient points. You have {loyalty.Points} points available.");
        }

        // Minimum 100 points required to redeem
        if (dto.Points < 100)
        {
            return ApiResponse<RedeemResultDto>.ErrorResponse(
                "Minimum 100 points required to redeem");
        }

        var discountAmount = CalculateDiscount(dto.Points);

        // Create redemption transaction
        var transaction = new LoyaltyTransaction
        {
            LoyaltyPointsId = loyalty.Id,
            OrderId = dto.OrderId,
            Points = -dto.Points, // Negative for redemption
            TransactionType = "Redeemed",
            Description = $"Redeemed for {discountAmount:C} discount"
        };

        loyalty.Points -= dto.Points;
        loyalty.TotalRedeemed += dto.Points;
        loyalty.UpdatedAt = DateTime.UtcNow;

        _context.LoyaltyTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        var result = new RedeemResultDto(
            true,
            dto.Points,
            discountAmount,
            loyalty.Points,
            $"Successfully redeemed {dto.Points} points for {discountAmount:C} discount!"
        );

        return ApiResponse<RedeemResultDto>.SuccessResponse(result);
    }

    public async Task<ApiResponse<List<LoyaltyTierDto>>> GetTiersAsync()
    {
        var tiers = new List<LoyaltyTierDto>
        {
            new("Bronze", 0, 999, 1.0m, "Earn 1 point per AED spent"),
            new("Silver", 1000, 4999, 1.25m, "Earn 1.25x points, priority support"),
            new("Gold", 5000, 9999, 1.5m, "Earn 1.5x points, exclusive offers, free delivery"),
            new("Platinum", 10000, int.MaxValue, 2.0m, "Earn 2x points, VIP offers, personal concierge")
        };

        return await Task.FromResult(ApiResponse<List<LoyaltyTierDto>>.SuccessResponse(tiers));
    }

    public async Task<ApiResponse<int>> AddBonusPointsAsync(string customerId, int points, string description)
    {
        var loyalty = await GetOrCreateLoyaltyPointsAsync(customerId);

        var transaction = new LoyaltyTransaction
        {
            LoyaltyPointsId = loyalty.Id,
            Points = points,
            TransactionType = "Bonus",
            Description = description
        };

        loyalty.Points += points;
        loyalty.TotalEarned += points;
        loyalty.UpdateTier();
        loyalty.UpdatedAt = DateTime.UtcNow;

        _context.LoyaltyTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return ApiResponse<int>.SuccessResponse(points, $"Added {points} bonus points!");
    }

    public decimal CalculateDiscount(int points)
    {
        // 100 points = 1 AED
        return points * PointsToAedRate;
    }

    public async Task<ApiResponse<PagedResponse<LoyaltyCustomerDto>>> GetCustomersAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        string? tier = null)
    {
        var query = _context.LoyaltyPoints
            .Include(l => l.Customer)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(l => 
                l.Customer.Email != null && l.Customer.Email.ToLower().Contains(search) ||
                l.Customer.FullName.ToLower().Contains(search));
        }

        // Apply tier filter
        if (!string.IsNullOrWhiteSpace(tier))
        {
            query = query.Where(l => l.Tier == tier);
        }

        var totalCount = await query.CountAsync();

        var customers = await query
            .OrderByDescending(l => l.Points)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new LoyaltyCustomerDto(
                l.CustomerId,
                l.Customer.FullName,
                l.Customer.Email ?? "",
                l.Points,
                l.TotalEarned,
                l.TotalRedeemed,
                l.Tier,
                l.GetBonusMultiplier()
            ))
            .ToListAsync();

        var pagedResponse = new PagedResponse<LoyaltyCustomerDto>
        {
            Items = customers,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return ApiResponse<PagedResponse<LoyaltyCustomerDto>>.SuccessResponse(pagedResponse);
    }

    private async Task<LoyaltyPoints> GetOrCreateLoyaltyPointsAsync(string customerId)
    {
        var loyalty = await _context.LoyaltyPoints
            .FirstOrDefaultAsync(l => l.CustomerId == customerId);

        if (loyalty == null)
        {
            loyalty = new LoyaltyPoints
            {
                CustomerId = customerId,
                Points = 0,
                TotalEarned = 0,
                TotalRedeemed = 0,
                Tier = "Bronze"
            };

            _context.LoyaltyPoints.Add(loyalty);
            await _context.SaveChangesAsync();
        }

        return loyalty;
    }

    private (int pointsToNext, string nextTier) CalculateNextTier(LoyaltyPoints loyalty)
    {
        return loyalty.Tier switch
        {
            "Bronze" => (1000 - loyalty.TotalEarned, "Silver"),
            "Silver" => (5000 - loyalty.TotalEarned, "Gold"),
            "Gold" => (10000 - loyalty.TotalEarned, "Platinum"),
            _ => (0, "Platinum") // Already at max
        };
    }
}
