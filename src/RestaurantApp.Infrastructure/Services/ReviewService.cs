using Microsoft.EntityFrameworkCore;
using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Review;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Infrastructure.Data;

namespace RestaurantApp.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<ReviewDto>>> GetItemReviewsAsync(int menuItemId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.MenuItemId == menuItemId && r.IsApproved && r.IsVisible)
            .OrderByDescending(r => r.CreatedAt)
            .Take(50)
            .Select(r => new ReviewDto(
                r.Id,
                r.MenuItemId,
                r.MenuItem.NameEn,
                r.OrderId,
                r.CustomerName,
                r.Rating,
                r.Comment,
                r.IsApproved,
                r.CreatedAt
            ))
            .ToListAsync();

        return ApiResponse<List<ReviewDto>>.SuccessResponse(reviews);
    }

    public async Task<ApiResponse<ReviewSummaryDto>> GetItemReviewSummaryAsync(int menuItemId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.MenuItemId == menuItemId && r.IsApproved && r.IsVisible)
            .ToListAsync();

        if (!reviews.Any())
        {
            return ApiResponse<ReviewSummaryDto>.SuccessResponse(new ReviewSummaryDto(
                menuItemId, 0, 0, 0, 0, 0, 0, 0
            ));
        }

        var summary = new ReviewSummaryDto(
            menuItemId,
            reviews.Average(r => r.Rating),
            reviews.Count,
            reviews.Count(r => r.Rating == 5),
            reviews.Count(r => r.Rating == 4),
            reviews.Count(r => r.Rating == 3),
            reviews.Count(r => r.Rating == 2),
            reviews.Count(r => r.Rating == 1)
        );

        return ApiResponse<ReviewSummaryDto>.SuccessResponse(summary);
    }

    public async Task<ApiResponse<List<ReviewDto>>> GetMyReviewsAsync(string customerId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .Include(r => r.MenuItem)
            .Select(r => new ReviewDto(
                r.Id,
                r.MenuItemId,
                r.MenuItem.NameEn,
                r.OrderId,
                r.CustomerName,
                r.Rating,
                r.Comment,
                r.IsApproved,
                r.CreatedAt
            ))
            .ToListAsync();

        return ApiResponse<List<ReviewDto>>.SuccessResponse(reviews);
    }

    public async Task<ApiResponse<ReviewDto>> CreateReviewAsync(string customerId, CreateReviewDto dto)
    {
        // Validate rating
        if (dto.Rating < 1 || dto.Rating > 5)
        {
            return ApiResponse<ReviewDto>.ErrorResponse("Rating must be between 1 and 5");
        }

        // Check if order exists and belongs to customer
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == dto.OrderId && o.UserId.ToString() == customerId);

        if (order == null)
        {
            return ApiResponse<ReviewDto>.ErrorResponse("Order not found or does not belong to you");
        }

        // Check if order contains the menu item
        if (!order.OrderItems.Any(i => i.MenuItemId == dto.MenuItemId))
        {
            return ApiResponse<ReviewDto>.ErrorResponse("This item was not in your order");
        }

        // Check if already reviewed
        var existingReview = await _context.Reviews
            .FirstOrDefaultAsync(r => r.CustomerId == customerId && 
                                      r.OrderId == dto.OrderId && 
                                      r.MenuItemId == dto.MenuItemId);

        if (existingReview != null)
        {
            return ApiResponse<ReviewDto>.ErrorResponse("You have already reviewed this item from this order");
        }

        // Get customer name
        var customer = await _context.Users.FindAsync(customerId);
        var customerName = customer?.FullName ?? "Customer";

        var review = new Review
        {
            CustomerId = customerId,
            OrderId = dto.OrderId,
            MenuItemId = dto.MenuItemId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CustomerName = customerName,
            IsApproved = true, // Auto-approve for now
            IsVisible = true
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var menuItem = await _context.MenuItems.FindAsync(dto.MenuItemId);

        return ApiResponse<ReviewDto>.SuccessResponse(new ReviewDto(
            review.Id,
            review.MenuItemId,
            menuItem?.NameEn ?? "",
            review.OrderId,
            review.CustomerName,
            review.Rating,
            review.Comment,
            review.IsApproved,
            review.CreatedAt
        ));
    }

    public async Task<ApiResponse<ReviewDto>> UpdateReviewAsync(string customerId, int reviewId, UpdateReviewDto dto)
    {
        var review = await _context.Reviews
            .Include(r => r.MenuItem)
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.CustomerId == customerId);

        if (review == null)
        {
            return ApiResponse<ReviewDto>.ErrorResponse("Review not found");
        }

        if (dto.Rating < 1 || dto.Rating > 5)
        {
            return ApiResponse<ReviewDto>.ErrorResponse("Rating must be between 1 and 5");
        }

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ApiResponse<ReviewDto>.SuccessResponse(new ReviewDto(
            review.Id,
            review.MenuItemId,
            review.MenuItem?.NameEn ?? "",
            review.OrderId,
            review.CustomerName,
            review.Rating,
            review.Comment,
            review.IsApproved,
            review.CreatedAt
        ));
    }

    public async Task<ApiResponse> DeleteReviewAsync(string customerId, int reviewId)
    {
        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.CustomerId == customerId);

        if (review == null)
        {
            return ApiResponse.ErrorResponse("Review not found");
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return ApiResponse.SuccessResponse("Review deleted successfully");
    }

    public async Task<ApiResponse<bool>> CanReviewItemAsync(string customerId, int orderId, int menuItemId)
    {
        // Check if order exists, is delivered, and contains the item
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId && 
                                      o.UserId.ToString() == customerId &&
                                      o.Status == RestaurantApp.Domain.Enums.OrderStatus.Delivered);

        if (order == null)
        {
            return ApiResponse<bool>.SuccessResponse(false, "Order not found or not delivered");
        }

        if (!order.OrderItems.Any(i => i.MenuItemId == menuItemId))
        {
            return ApiResponse<bool>.SuccessResponse(false, "Item not in this order");
        }

        // Check if already reviewed
        var alreadyReviewed = await _context.Reviews
            .AnyAsync(r => r.CustomerId == customerId && 
                          r.OrderId == orderId && 
                          r.MenuItemId == menuItemId);

        return ApiResponse<bool>.SuccessResponse(!alreadyReviewed);
    }

    // Admin operations
    
    public async Task<ApiResponse<PagedResponse<ReviewModerationDto>>> GetAllReviewsAsync(
        int page = 1, 
        int pageSize = 20, 
        string? status = null)
    {
        var query = _context.Reviews
            .Include(r => r.MenuItem)
            .Include(r => r.Customer)
            .AsQueryable();

        // Filter by status if provided
        if (!string.IsNullOrWhiteSpace(status))
        {
            switch (status.ToLower())
            {
                case "approved":
                    query = query.Where(r => r.IsApproved);
                    break;
                case "rejected":
                    query = query.Where(r => !r.IsApproved && !r.IsVisible);
                    break;
                case "pending":
                    query = query.Where(r => !r.IsApproved && r.IsVisible);
                    break;
            }
        }

        var totalCount = await query.CountAsync();

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReviewModerationDto(
                r.Id,
                r.MenuItemId,
                r.MenuItem.NameEn,
                r.MenuItem.NameAr,
                r.OrderId,
                r.CustomerName,
                r.Customer.Email ?? "",
                r.Rating,
                r.Comment,
                r.IsApproved,
                r.IsVisible,
                r.CreatedAt
            ))
            .ToListAsync();

        return ApiResponse<PagedResponse<ReviewModerationDto>>.SuccessResponse(
            new PagedResponse<ReviewModerationDto>
            {
                Items = reviews,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            });
    }
    
    public async Task<ApiResponse<List<ReviewModerationDto>>> GetPendingReviewsAsync()
    {
        var reviews = await _context.Reviews
            .Where(r => !r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .Include(r => r.MenuItem)
            .Include(r => r.Customer)
            .Select(r => new ReviewModerationDto(
                r.Id,
                r.MenuItemId,
                r.MenuItem.NameEn,
                r.MenuItem.NameAr,
                r.OrderId,
                r.CustomerName,
                r.Customer.Email ?? "",
                r.Rating,
                r.Comment,
                r.IsApproved,
                r.IsVisible,
                r.CreatedAt
            ))
            .ToListAsync();

        return ApiResponse<List<ReviewModerationDto>>.SuccessResponse(reviews);
    }

    public async Task<ApiResponse> ApproveReviewAsync(int reviewId)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null)
        {
            return ApiResponse.ErrorResponse("Review not found");
        }

        review.IsApproved = true;
        review.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ApiResponse.SuccessResponse("Review approved");
    }

    public async Task<ApiResponse> RejectReviewAsync(int reviewId)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null)
        {
            return ApiResponse.ErrorResponse("Review not found");
        }

        review.IsApproved = false;
        review.IsVisible = false;
        review.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ApiResponse.SuccessResponse("Review rejected");
    }

    public async Task<ApiResponse> ToggleReviewVisibilityAsync(int reviewId)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null)
        {
            return ApiResponse.ErrorResponse("Review not found");
        }

        review.IsVisible = !review.IsVisible;
        review.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ApiResponse.SuccessResponse($"Review is now {(review.IsVisible ? "visible" : "hidden")}");
    }
}
