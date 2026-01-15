# 🔍 API Architectural Review & Implementation Roadmap

> **Review Date:** January 10, 2026  
> **Reviewer:** Senior Architect (15+ YOE)  
> **API Version:** v1.0  
> **Total Endpoints:** ~98 across 12 controllers

---

## 📊 Current API Inventory

| Controller | Endpoints | Auth | Quality | Status |
|------------|-----------|------|---------|--------|
| **AuthController** | 9 | Mixed | ⭐⭐⭐⭐ | ✅ Complete |
| **MenuController** | 12 | Admin CRUD | ⭐⭐⭐⭐ | ✅ Complete |
| **OrdersController** | 6 (User) + 5 (Admin) | ✅ | ⭐⭐⭐⭐ | ✅ Complete |
| **BranchesController** | 6 | Admin CRUD | ⭐⭐⭐⭐ | ✅ Complete |
| **AddressesController** | 6 | User | ⭐⭐⭐⭐ | ✅ Complete |
| **FavoritesController** | 7 | User | ⭐⭐⭐⭐ | ✅ Complete |
| **OffersController** | 9 | Admin CRUD | ⭐⭐⭐⭐ | ✅ Complete |
| **DeliveriesController** | 9 | Admin CRUD | ⭐⭐⭐⭐ | ✅ Complete |
| **ReviewsController** | 11 | Mixed | ⭐⭐⭐⭐ | ✅ Complete |
| **LoyaltyController** | 9 | Mixed | ⭐⭐⭐ | ⚠️ 90% |
| **ReportsController** | 5 | Admin | ⭐⭐⭐⭐ | ✅ Complete |
| **UsersController** | 4 | Admin | ⭐⭐⭐⭐ | ✅ Complete |

---

## ✅ Recently Implemented (Good Progress!)

| Item | File | Status |
|------|------|--------|
| Global Exception Handling | `Middleware/ExceptionHandlingMiddleware.cs` | ✅ Done |
| Request/Response Logging | `Middleware/RequestResponseLoggingMiddleware.cs` | ✅ Done |
| Admin User Management | `Controllers/UsersController.cs` | ✅ Done |
| Enhanced Swagger Docs | `Program.cs` | ✅ Done |
| Environment-specific JWT | `Program.cs` | ✅ Done |
| Production CORS Policy | `Program.cs` | ✅ Done |

---

## 🔴 Critical API Gaps

### 1. Order-Offer Integration Missing

> [!CAUTION]
> Promo code validation exists but is NOT connected to order creation!

**Current Flow:**
```
Mobile App → Validates Coupon ✅ → Creates Order ❌ (Discount not applied!)
```

**Missing in `OrderService.CreateOrderAsync`:**
- [ ] Accept `PromoCode` in `CreateOrderDto`
- [ ] Validate & apply discount at order creation
- [ ] Track coupon usage (increment `UsageCount`)
- [ ] Check per-user limit

**Files to Modify:**
- `Application/DTOs/Order/CreateOrderDto.cs`
- `Infrastructure/Services/OrderService.cs`

**Effort:** 4-6 hours

---

### 2. Order Status History Not Persisted

> [!WARNING]
> `GetOrderTrackingAsync` fakes status history with calculated timestamps

**Current (Fake):**
```csharp
statusHistory.Add(new(OrderStatus.Confirmed, order.CreatedAt.AddMinutes(2)));
```

**Required:**
- [ ] Create `OrderStatusHistory` entity
- [ ] Create database migration
- [ ] Store actual status changes with timestamp & actor
- [ ] Update `OrderService` to record history

**Files to Create/Modify:**
- `Domain/Entities/OrderStatusHistory.cs` (NEW)
- `Infrastructure/Data/ApplicationDbContext.cs`
- `Infrastructure/Services/OrderService.cs`

**Effort:** 4-6 hours

---

### 3. Loyalty Points Not Auto-Awarded

> [!IMPORTANT]
> Endpoint `/api/loyalty/award-order` exists but is never called automatically

**Missing in `OrderService.UpdateOrderStatusAsync`:**
```csharp
if (newStatus == OrderStatus.Delivered)
{
    await _loyaltyService.AwardPointsAsync(order.UserId.ToString(), order.Id, order.Total);
}
```

**File to Modify:**
- `Infrastructure/Services/OrderService.cs`

**Effort:** 2 hours

---

### 4. Admin Orders Authorization Disabled

> [!CAUTION]
> `AdminOrdersController` authorization is COMMENTED OUT!

**Location:** `Controllers/OrdersController.cs` line 88
```csharp
// [Authorize(Roles = "Admin,Cashier")]  // ← STILL COMMENTED!
```

**Action:** Uncomment ALL admin authorization attributes

**Effort:** 30 minutes

---

## 🟡 Missing API Endpoints

### Phase 1: High Priority (Before Production)

| Endpoint | Description | Controller | Priority |
|----------|-------------|------------|----------|
| `POST /api/orders` + PromoCode | Apply promo code to order | OrdersController | 🔴 Critical |
| `GET /api/admin/reviews` | Get ALL reviews (not just pending) | ReviewsController | 🔴 High |
| `GET /api/orders/{id}/receipt` | Generate order receipt PDF | OrdersController | 🟡 Medium |
| `POST /api/orders/{id}/reorder` | Reorder previous order | OrdersController | 🟡 Medium |
| `GET /api/menu/items/filter` | Filter items by price, rating | MenuController | 🟡 Medium |

### Phase 2: Enhancement Endpoints

| Endpoint | Description | Controller |
|----------|-------------|------------|
| `GET /api/health` | Health check endpoint | HealthController (new) |
| `POST /api/auth/social-login` | Google/Apple sign-in | AuthController |
| `POST /api/auth/refresh-token` | Refresh JWT token | AuthController |
| `POST /api/device-tokens` | Register FCM token | NotificationsController (new) |
| `GET /api/notifications` | Get user notifications | NotificationsController (new) |
| `POST /api/orders/schedule` | Schedule future order | OrdersController |
| `GET /api/menu/recommendations` | AI-based menu recommendations | MenuController |

### Phase 3: Admin Enhancement Endpoints

| Endpoint | Description | Controller |
|----------|-------------|------------|
| `GET /api/admin/reports/export` | Export reports to Excel/PDF | ReportsController |
| `GET /api/admin/audit-log` | Admin action audit log | AuditController (new) |
| `GET /api/admin/dashboard-kpis` | Real-time dashboard KPIs | ReportsController |
| `POST /api/admin/broadcast` | Send notification to all users | NotificationsController |

---

## 📋 Implementation Phases

### Phase 1: Critical Fixes (Week 1)
**Estimated Time:** 15-20 hours

| # | Task | File | Effort | Priority |
|---|------|------|--------|----------|
| 1.1 | Enable admin authorization on all admin endpoints | Multiple controllers | 1 hour | 🔴 |
| 1.2 | Integrate promo code with order creation | `OrderService.cs` | 4-6 hours | 🔴 |
| 1.3 | Create OrderStatusHistory entity & migration | Domain + Infrastructure | 4-6 hours | 🔴 |
| 1.4 | Auto-award loyalty points on delivery | `OrderService.cs` | 2 hours | 🔴 |
| 1.5 | Add GET /api/admin/reviews endpoint | `ReviewsController.cs` | 2 hours | 🔴 |
| 1.6 | Add DTO validation attributes | All DTOs | 3-4 hours | 🔴 |

---

### Phase 2: API Completeness (Week 2)
**Estimated Time:** 20-25 hours

| # | Task | File | Effort | Priority |
|---|------|------|--------|----------|
| 2.1 | Add health check endpoint | `HealthController.cs` (new) | 1 hour | 🟡 |
| 2.2 | Add refresh token endpoint | `AuthController.cs` | 3-4 hours | 🟡 |
| 2.3 | Add reorder endpoint | `OrdersController.cs` | 3-4 hours | 🟡 |
| 2.4 | Add order receipt generation | `OrdersController.cs` | 4-6 hours | 🟡 |
| 2.5 | Create NotificationsController + FCM | `NotificationsController.cs` (new) | 6-8 hours | 🟡 |
| 2.6 | Add menu item filtering | `MenuController.cs` | 2-3 hours | 🟡 |

---

### Phase 3: Production Hardening (Week 3)
**Estimated Time:** 15-20 hours

| # | Task | File | Effort | Priority |
|---|------|------|--------|----------|
| 3.1 | Add rate limiting middleware | `RateLimitingMiddleware.cs` | 3-4 hours | 🟡 |
| 3.2 | Add API versioning | All controllers | 4-6 hours | 🟡 |
| 3.3 | Add Swagger XML documentation | All controllers | 2-3 hours | 🟡 |
| 3.4 | Implement caching for menu/branches | Services | 4-6 hours | 🟡 |
| 3.5 | Add database indexes migration | Migrations | 2 hours | 🟡 |

---

### Phase 4: Advanced Features (Week 4+)
**Estimated Time:** 25-35 hours

| # | Task | File | Effort | Priority |
|---|------|------|--------|----------|
| 4.1 | Report export (Excel/PDF) | `ReportsController.cs` | 6-8 hours | 🟢 |
| 4.2 | Order scheduling | `OrdersController.cs` | 4-6 hours | 🟢 |
| 4.3 | Social login (Google/Apple) | `AuthController.cs` | 6-8 hours | 🟢 |
| 4.4 | Payment gateway integration | `PaymentsController.cs` (new) | 12-16 hours | 🟢 |
| 4.5 | Audit logging | `AuditController.cs` (new) | 4-6 hours | 🟢 |

---

## 🔧 Quick Fixes (Ready to Implement)

### Fix 1: Uncomment Admin Authorization

**File:** `Controllers/OrdersController.cs` line 88

```diff
- // [Authorize(Roles = "Admin,Cashier")]
+ [Authorize(Roles = "Admin,Cashier")]
public class AdminOrdersController : ControllerBase
```

---

### Fix 2: Add PromoCode to CreateOrderDto

**File:** `Application/DTOs/Order/CreateOrderDto.cs`

```csharp
public record CreateOrderDto(
    [Required(ErrorMessage = "Branch ID is required")]
    int BranchId,
    
    [Required, MinLength(1, ErrorMessage = "Order must have at least one item")]
    List<OrderItemDto> Items,
    
    OrderType OrderType = OrderType.Delivery,
    string? DeliveryAddressLine = null,
    decimal? DeliveryLatitude = null,
    decimal? DeliveryLongitude = null,
    string? DeliveryNotes = null,
    DateTime? RequestedDeliveryTime = null,
    string? CustomerNotes = null,
    
    // NEW: Promo code support
    string? PromoCode = null
);
```

---

### Fix 3: Add GetAllReviews Endpoint

**File:** `Controllers/ReviewsController.cs`

```csharp
/// <summary>
/// Get all reviews with pagination (admin)
/// </summary>
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

---

### Fix 4: Create OrderStatusHistory Entity

**File:** `Domain/Entities/OrderStatusHistory.cs` (NEW)

```csharp
using RestaurantApp.Domain.Enums;

namespace RestaurantApp.Domain.Entities;

public class OrderStatusHistory : BaseEntity
{
    public int OrderId { get; set; }
    public OrderStatus PreviousStatus { get; set; }
    public OrderStatus NewStatus { get; set; }
    public string? ChangedBy { get; set; }
    public string? Notes { get; set; }
    
    public virtual Order Order { get; set; } = null!;
}
```

---

## 📊 API Quality Scorecard

| Aspect | Current | Target | Gap |
|--------|---------|--------|-----|
| **Endpoint Coverage** | 90% | 100% | +10 endpoints |
| **Authorization** | 75% | 100% | Enable admin auth |
| **Validation** | 30% | 100% | Add DTO attributes |
| **Documentation** | 60% | 90% | Add XML docs |
| **Error Handling** | 90% | 95% | ✅ Done |
| **Logging** | 80% | 90% | ✅ Done |
| **Caching** | 0% | 50% | Add Redis/MemCache |
| **Rate Limiting** | 0% | 100% | Add middleware |

---

## 🎯 Priority Task Checklist

### Immediate (Do Today)
- [ ] Uncomment `[Authorize(Roles = "Admin")]` on AdminOrdersController

### This Week (Critical)
- [ ] Add promo code to order creation flow
- [ ] Create OrderStatusHistory table
- [ ] Auto-award loyalty points on delivery
- [ ] Add DTO validation attributes
- [ ] Add GET /api/admin/reviews endpoint

### Next Week (Important)
- [ ] Add refresh token endpoint
- [ ] Add health check endpoint  
- [ ] Add reorder functionality
- [ ] Create NotificationsController for FCM
- [ ] Add rate limiting

### Future (Nice to Have)
- [ ] Payment gateway integration
- [ ] Report export (Excel/PDF)
- [ ] Social login (Google/Apple)
- [ ] Order scheduling
- [ ] Audit logging

---

## 📈 Effort Summary

| Phase | Effort | Items |
|-------|--------|-------|
| **Phase 1: Critical Fixes** | 15-20 hours | 6 items |
| **Phase 2: API Completeness** | 20-25 hours | 6 items |
| **Phase 3: Production Hardening** | 15-20 hours | 5 items |
| **Phase 4: Advanced Features** | 25-35 hours | 5 items |
| **TOTAL** | **75-100 hours** | **22 items** |

**Timeline:** 3-4 weeks (1 developer) or 1.5-2 weeks (2 developers)

---

**Last Updated:** January 10, 2026  
**Status:** Pre-Production Review
