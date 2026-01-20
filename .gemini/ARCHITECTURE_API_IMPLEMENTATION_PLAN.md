# 🏗️ Architecture & API Design Implementation Plan

> **Created:** January 20, 2026  
> **Status:** PENDING REVIEW  
> **Phases:** 1️⃣ Architecture & System Design | 2️⃣ API Design & Contracts

---

## 📋 Overview

This document outlines the implementation plan for addressing **critical architectural and API design issues** identified in the production readiness review. The plan is divided into two phases that must be completed and reviewed before proceeding to Phase 3.

---

## 1️⃣ PHASE 1: Architecture & System Design

### 🎯 Objectives
- Fix architectural gaps in the domain layer
- Implement missing entities and relationships
- Ensure proper separation of concerns
- Complete the Clean Architecture implementation

### 📊 Tasks Breakdown

#### Task 1.1: Create OrderStatusHistory Entity ⏱️ 4-6 hours
**Priority:** 🔴 CRITICAL

**Current Issue:**
- Order tracking uses fake timestamps calculated from `CreatedAt`
- No audit trail of who changed order status and when
- Cannot track status change history

**Implementation:**

1. **Create Entity** (`Domain/Entities/OrderStatusHistory.cs`)
   ```csharp
   public class OrderStatusHistory : BaseEntity
   {
       public int OrderId { get; set; }
       public OrderStatus PreviousStatus { get; set; }
       public OrderStatus NewStatus { get; set; }
       public string? ChangedBy { get; set; }  // User ID or "System"
       public string? Notes { get; set; }
       public DateTime ChangedAt { get; set; }
       
       public virtual Order Order { get; set; } = null!;
   }
   ```

2. **Update DbContext** (`Infrastructure/Data/ApplicationDbContext.cs`)
   - Add `DbSet<OrderStatusHistory>`
   - Configure entity relationships

3. **Create Migration**
   ```bash
   dotnet ef migrations add AddOrderStatusHistory
   ```

4. **Update OrderService**
   - Record status changes in `UpdateOrderStatusAsync()`
   - Load real history in `GetOrderTrackingAsync()`

**Files to Create/Modify:**
- ✅ `Domain/Entities/OrderStatusHistory.cs` (NEW)
- ✅ `Infrastructure/Data/ApplicationDbContext.cs` (MODIFY)
- ✅ `Infrastructure/Services/OrderService.cs` (MODIFY)
- ✅ Migration file (NEW)

---

#### Task 1.2: Integrate Promo Codes with Order Creation ⏱️ 4-6 hours
**Priority:** 🔴 CRITICAL

**Current Issue:**
- Promo code validation exists but is NOT applied during order creation
- Discount is not calculated or stored
- Coupon usage is not tracked

**Implementation:**

1. **Update CreateOrderDto** (`Application/DTOs/Order/CreateOrderDto.cs`)
   ```csharp
   public record CreateOrderDto(
       // ... existing properties
       string? PromoCode = null  // NEW
   );
   ```

2. **Update CreateOrderUseCase** (`Infrastructure/UseCases/Orders/CreateOrderUseCase.cs`)
   - Validate promo code if provided
   - Calculate discount using `CouponValidationService`
   - Apply discount to order total
   - Increment offer usage count
   - Store promo code in order

3. **Update Order Entity** (if needed)
   - Add `PromoCode` property
   - Add `DiscountAmount` property (if not exists)

**Files to Modify:**
- ✅ `Application/DTOs/Order/CreateOrderDto.cs`
- ✅ `Infrastructure/UseCases/Orders/CreateOrderUseCase.cs`
- ✅ `Domain/Entities/Order.cs` (verify properties exist)

---

#### Task 1.3: Auto-Award Loyalty Points on Delivery ⏱️ 2 hours
**Priority:** 🔴 CRITICAL

**Current Issue:**
- Loyalty points endpoint exists but is never called automatically
- Points are not awarded when order is delivered

**Implementation:**

1. **Update UpdateOrderStatusUseCase** (`Infrastructure/UseCases/Orders/UpdateOrderStatusUseCase.cs`)
   ```csharp
   if (newStatus == OrderStatus.Delivered)
   {
       // Award loyalty points
       await _loyaltyService.AwardPointsForOrderAsync(
           order.UserId.ToString(), 
           order.Id, 
           order.Total
       );
   }
   ```

2. **Verify LoyaltyService** has the method
   - Check if `AwardPointsForOrderAsync` exists
   - Implement if missing

**Files to Modify:**
- ✅ `Infrastructure/UseCases/Orders/UpdateOrderStatusUseCase.cs`
- ✅ `Infrastructure/Services/LoyaltyService.cs` (verify/implement)

---

#### Task 1.4: Enable Admin Authorization ⏱️ 1 hour
**Priority:** 🔴 CRITICAL

**Current Issue:**
- Admin endpoints have commented-out authorization attributes
- Security vulnerability allowing unauthorized access

**Implementation:**

1. **Uncomment Authorization Attributes**
   - `Controllers/OrdersController.cs` (AdminOrdersController)
   - Verify all admin endpoints have proper `[Authorize]` attributes

2. **Verify Authorization Policies**
   - Check `Program.cs` for admin policies
   - Ensure role-based authorization is configured

**Files to Modify:**
- ✅ `Controllers/OrdersController.cs`
- ✅ Other admin controllers (verify)

---

#### Task 1.5: Add DTO Validation Attributes ⏱️ 3-4 hours
**Priority:** 🔴 HIGH

**Current Issue:**
- DTOs lack validation attributes
- Invalid data can reach the service layer

**Implementation:**

1. **Add Validation to All DTOs**
   - `CreateOrderDto` - Required fields, ranges
   - `UpdateOrderDto` - Validation rules
   - `CreateMenuItemDto` - Price > 0, required name
   - `CreateBranchDto` - Coordinates validation
   - `CreateOfferDto` - Date validation
   - All other DTOs

2. **Example:**
   ```csharp
   public record CreateMenuItemDto(
       [Required(ErrorMessage = "Name is required")]
       [StringLength(100, MinimumLength = 3)]
       string Name,
       
       [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
       decimal Price,
       
       [Required]
       int CategoryId
   );
   ```

**Files to Modify:**
- ✅ All DTO files in `Application/DTOs/`

---

### 📊 Phase 1 Summary

| Task | Priority | Effort | Status |
|------|----------|--------|--------|
| 1.1 OrderStatusHistory Entity | 🔴 Critical | 4-6 hours | ⏳ Pending |
| 1.2 Promo Code Integration | 🔴 Critical | 4-6 hours | ⏳ Pending |
| 1.3 Auto-Award Loyalty Points | 🔴 Critical | 2 hours | ⏳ Pending |
| 1.4 Enable Admin Authorization | 🔴 Critical | 1 hour | ⏳ Pending |
| 1.5 Add DTO Validation | 🔴 High | 3-4 hours | ⏳ Pending |
| **TOTAL** | | **14-19 hours** | |

---

## 2️⃣ PHASE 2: API Design & Contracts

### 🎯 Objectives
- Complete missing API endpoints
- Standardize API responses
- Improve API documentation
- Add health check endpoint

### 📊 Tasks Breakdown

#### Task 2.1: Add GET /api/admin/reviews Endpoint ⏱️ 2 hours
**Priority:** 🔴 HIGH

**Current Issue:**
- Only pending reviews endpoint exists
- Admins cannot view all reviews

**Implementation:**

1. **Add Method to ReviewService**
   ```csharp
   Task<PagedResponse<ReviewDto>> GetAllReviewsAsync(
       int page, 
       int pageSize, 
       string? status = null
   );
   ```

2. **Add Controller Endpoint**
   ```csharp
   [HttpGet]
   [Authorize(Roles = "Admin")]
   public async Task<IActionResult> GetAllReviews(
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 20,
       [FromQuery] string? status = null)
   {
       var result = await _reviewService.GetAllReviewsAsync(page, pageSize, status);
       return Ok(result);
   }
   ```

**Files to Modify:**
- ✅ `Infrastructure/Services/ReviewService.cs`
- ✅ `Controllers/ReviewsController.cs`

---

#### Task 2.2: Add Health Check Endpoint ⏱️ 1 hour
**Priority:** 🟡 MEDIUM

**Implementation:**

1. **Create HealthController**
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class HealthController : ControllerBase
   {
       [HttpGet]
       public IActionResult Get()
       {
           return Ok(new { 
               status = "healthy", 
               timestamp = DateTime.UtcNow,
               version = "1.0.0"
           });
       }
       
       [HttpGet("db")]
       [Authorize(Roles = "Admin")]
       public async Task<IActionResult> CheckDatabase()
       {
           // Check database connectivity
       }
   }
   ```

2. **Configure Health Checks in Program.cs**
   ```csharp
   builder.Services.AddHealthChecks()
       .AddDbContextCheck<ApplicationDbContext>();
   ```

**Files to Create/Modify:**
- ✅ `Controllers/HealthController.cs` (NEW)
- ✅ `Program.cs` (MODIFY)

---

#### Task 2.3: Add Reorder Endpoint ⏱️ 3-4 hours
**Priority:** 🟡 MEDIUM

**Implementation:**

1. **Add Method to OrderService**
   ```csharp
   Task<OrderCreatedDto> ReorderAsync(int orderId, string userId);
   ```

2. **Add Controller Endpoint**
   ```csharp
   [HttpPost("{id}/reorder")]
   [Authorize]
   public async Task<IActionResult> Reorder(int id)
   {
       var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
       var result = await _orderService.ReorderAsync(id, userId);
       return Ok(result);
   }
   ```

3. **Implementation Logic**
   - Fetch original order
   - Verify user owns the order
   - Check if items are still available
   - Create new order with same items
   - Handle price changes

**Files to Modify:**
- ✅ `Infrastructure/Services/OrderService.cs`
- ✅ `Controllers/OrdersController.cs`

---

#### Task 2.4: Add Menu Item Filtering ⏱️ 2-3 hours
**Priority:** 🟡 MEDIUM

**Implementation:**

1. **Update MenuService**
   ```csharp
   Task<PagedResponse<MenuItemDto>> GetMenuItemsAsync(
       int? categoryId = null,
       decimal? minPrice = null,
       decimal? maxPrice = null,
       decimal? minRating = null,
       bool? isFeatured = null,
       int page = 1,
       int pageSize = 20
   );
   ```

2. **Update Controller**
   ```csharp
   [HttpGet("items/filter")]
   public async Task<IActionResult> FilterMenuItems(
       [FromQuery] int? categoryId,
       [FromQuery] decimal? minPrice,
       [FromQuery] decimal? maxPrice,
       [FromQuery] decimal? minRating,
       [FromQuery] bool? isFeatured,
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 20)
   {
       var result = await _menuService.GetMenuItemsAsync(
           categoryId, minPrice, maxPrice, minRating, isFeatured, page, pageSize);
       return Ok(result);
   }
   ```

**Files to Modify:**
- ✅ `Infrastructure/Services/MenuService.cs`
- ✅ `Controllers/MenuController.cs`

---

#### Task 2.5: Enhance Swagger Documentation ⏱️ 2-3 hours
**Priority:** 🟡 MEDIUM

**Implementation:**

1. **Add XML Comments to Controllers**
   - Document all endpoints
   - Add parameter descriptions
   - Add response examples

2. **Example:**
   ```csharp
   /// <summary>
   /// Creates a new order
   /// </summary>
   /// <param name="dto">Order details including items and delivery info</param>
   /// <returns>Created order with ID and estimated delivery time</returns>
   /// <response code="201">Order created successfully</response>
   /// <response code="400">Invalid order data</response>
   /// <response code="401">User not authenticated</response>
   [HttpPost]
   [ProducesResponseType(typeof(OrderCreatedDto), 201)]
   [ProducesResponseType(typeof(ProblemDetails), 400)]
   [ProducesResponseType(401)]
   public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
   ```

3. **Enable XML Documentation**
   - Already enabled in Phase 4
   - Add comments to all endpoints

**Files to Modify:**
- ✅ All controllers in `Controllers/`

---

#### Task 2.6: Add Refresh Token Endpoint ⏱️ 3-4 hours
**Priority:** 🟡 MEDIUM

**Implementation:**

1. **Update User Entity**
   - Add `RefreshToken` property
   - Add `RefreshTokenExpiry` property

2. **Create RefreshTokenDto**
   ```csharp
   public record RefreshTokenRequest(string RefreshToken);
   public record RefreshTokenResponse(string AccessToken, string RefreshToken);
   ```

3. **Add to AuthController**
   ```csharp
   [HttpPost("refresh-token")]
   public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
   {
       // Validate refresh token
       // Generate new access token
       // Generate new refresh token
       // Return both tokens
   }
   ```

**Files to Create/Modify:**
- ✅ `Domain/Entities/User.cs` (MODIFY)
- ✅ `Application/DTOs/Auth/RefreshTokenDto.cs` (NEW)
- ✅ `Controllers/AuthController.cs` (MODIFY)
- ✅ `Infrastructure/Services/AuthService.cs` (MODIFY)

---

### 📊 Phase 2 Summary

| Task | Priority | Effort | Status |
|------|----------|--------|--------|
| 2.1 GET /api/admin/reviews | 🔴 High | 2 hours | ⏳ Pending |
| 2.2 Health Check Endpoint | 🟡 Medium | 1 hour | ⏳ Pending |
| 2.3 Reorder Endpoint | 🟡 Medium | 3-4 hours | ⏳ Pending |
| 2.4 Menu Item Filtering | 🟡 Medium | 2-3 hours | ⏳ Pending |
| 2.5 Swagger Documentation | 🟡 Medium | 2-3 hours | ⏳ Pending |
| 2.6 Refresh Token Endpoint | 🟡 Medium | 3-4 hours | ⏳ Pending |
| **TOTAL** | | **13-17 hours** | |

---

## 📊 Overall Summary

### Effort Breakdown
| Phase | Tasks | Effort | Priority |
|-------|-------|--------|----------|
| **Phase 1: Architecture** | 5 | 14-19 hours | 🔴 Critical |
| **Phase 2: API Design** | 6 | 13-17 hours | 🔴 High |
| **TOTAL** | **11** | **27-36 hours** | |

### Priority Distribution
- 🔴 **Critical:** 4 tasks (OrderStatusHistory, PromoCode, Loyalty, Authorization)
- 🔴 **High:** 2 tasks (DTO Validation, Admin Reviews)
- 🟡 **Medium:** 5 tasks (Health Check, Reorder, Filtering, Swagger, Refresh Token)

### Timeline Estimate
- **1 Developer:** 4-5 days
- **2 Developers:** 2-3 days

---

## ✅ Review Checklist

Before proceeding to implementation, please review:

- [ ] **Task Priorities:** Are the priorities correct?
- [ ] **Task Scope:** Is anything missing or should be added?
- [ ] **Implementation Approach:** Do the proposed solutions make sense?
- [ ] **Timeline:** Is the effort estimate reasonable?
- [ ] **Dependencies:** Are there any blocking dependencies?
- [ ] **Testing:** Should we add specific test requirements?

---

## 🎯 Next Steps

**After your review and approval:**

1. ✅ Start with Phase 1 Critical Tasks (1.1 - 1.4)
2. ✅ Run tests and build verification
3. ✅ Commit Phase 1 changes
4. ⏸️ **PAUSE FOR YOUR REVIEW**
5. ✅ Proceed to Phase 2 upon approval
6. ✅ Complete Phase 2 tasks
7. ✅ Final testing and documentation
8. ⏸️ **PAUSE FOR YOUR REVIEW**
9. ✅ Proceed to Phase 3 (Security & Testing)

---

**Status:** 📋 **AWAITING YOUR REVIEW**  
**Created:** January 20, 2026  
**Last Updated:** January 20, 2026
