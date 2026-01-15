# Missing API Endpoints Analysis

## Summary
This document lists all API endpoints that are referenced in the web application or mobile application but are missing from the backend API implementation.

---

## 🔴 Critical Missing Endpoints

### 1. Users/Admin Users Controller (COMPLETELY MISSING)
**Location Needed**: `src/RestaurantApp.API/Controllers/UsersController.cs`

**Endpoints Required**:
- `GET /api/admin/users` - Get all users (with pagination, filters)
  - Query parameters: `page`, `pageSize`, `role`, `status`, `search`
  - Response: List of users with order count, total spent, etc.
  
- `GET /api/admin/users/{id}` - Get user details
  - Response: User profile with order history summary
  
- `PUT /api/admin/users/{id}` - Update user
  - Body: UpdateUserDto (role, status, etc.)
  
- `DELETE /api/admin/users/{id}` - Delete/deactivate user
  - Soft delete (set IsActive = false)

**Used By**: 
- `src/RestaurantApp.Web/Components/Pages/Admin/Users/Index.razor`

---

### 2. Reports/Analytics Controller (COMPLETELY MISSING)
**Location Needed**: `src/RestaurantApp.API/Controllers/ReportsController.cs`

**Endpoints Required**:
- `GET /api/admin/reports/summary` - Get overall business summary
  - Response: Total revenue, orders, customers, average order value
  
- `GET /api/admin/reports/revenue` - Get revenue analytics
  - Query parameters: `fromDate`, `toDate`, `groupBy` (day/week/month)
  - Response: Revenue breakdown by period
  
- `GET /api/admin/reports/orders` - Get order analytics
  - Query parameters: `fromDate`, `toDate`, `status`, `branchId`
  - Response: Order statistics, trends
  
- `GET /api/admin/reports/popular-items` - Get most popular menu items
  - Query parameters: `fromDate`, `toDate`, `limit`
  - Response: Top selling items with quantities
  
- `GET /api/admin/reports/branch-performance` - Get branch performance
  - Query parameters: `fromDate`, `toDate`, `branchId`
  - Response: Revenue and order stats per branch

**Used By**: 
- `src/RestaurantApp.Web/Components/Pages/Admin/Reports/Index.razor`

---

### 3. Admin Loyalty Endpoints (PARTIALLY MISSING)
**Location**: `src/RestaurantApp.API/Controllers/LoyaltyController.cs`

**Missing Endpoint**:
- `GET /api/admin/loyalty/customers` - Get all customers with loyalty points
  - Query parameters: `page`, `pageSize`, `search`, `tier`
  - Response: List of customers with points, tier, transaction summary
  - Currently: Comment in `LoyaltyApiService.cs` says "we need a new endpoint for listing all customers"

**Used By**: 
- `src/RestaurantApp.Web/Services/LoyaltyApiService.cs` (line 19-21)
- `src/RestaurantApp.Web/Components/Pages/Admin/Loyalty/Index.razor`

---

## 🟡 Potentially Missing Endpoints (To Verify)

### 4. Admin Orders - Assign Delivery Driver
**Location**: `src/RestaurantApp.API/Controllers/OrdersController.cs` (AdminOrdersController)

**Potentially Missing**:
- `PUT /api/admin/orders/{id}/assign-driver` - Assign delivery driver to order
  - Body: `{ deliveryId: int }`

**Current Status**: Need to verify if this exists in the service layer

---

### 5. Admin Reviews - Get All Reviews
**Location**: `src/RestaurantApp.API/Controllers/ReviewsController.cs`

**Potentially Missing**:
- `GET /api/admin/reviews` - Get all reviews (not just pending)
  - Query parameters: `page`, `pageSize`, `status`, `rating`, `itemId`
  - Response: Paginated list of all reviews

**Current Status**: Currently only `/api/reviews/pending` exists, but web app may need all reviews

---

## ✅ Endpoints That Exist (For Reference)

### Menu/Categories - All CRUD operations exist ✅
- POST /api/menu/categories
- PUT /api/menu/categories/{id}
- DELETE /api/menu/categories/{id}
- POST /api/menu/items
- PUT /api/menu/items/{id}
- DELETE /api/menu/items/{id}
- POST /api/menu/items/{id}/toggle-availability

### Branches - All CRUD operations exist ✅
- POST /api/branches
- PUT /api/branches/{id}
- DELETE /api/branches/{id}

### Admin Orders - Basic operations exist ✅
- GET /api/admin/orders
- GET /api/admin/orders/{id}
- PUT /api/admin/orders/{id}/status

### Reviews - Moderation endpoints exist ✅
- GET /api/reviews/pending
- PATCH /api/reviews/{id}/approve
- PATCH /api/reviews/{id}/reject
- PATCH /api/reviews/{id}/toggle-visibility

---

## 📋 Implementation Priority

### High Priority (Required for Web App)
1. **UsersController** - Web app has full UI, but no backend
2. **ReportsController** - Web app has full UI, but no backend
3. **Admin Loyalty Customers List** - Web app expects this endpoint

### Medium Priority (Nice to Have)
4. **Admin Reviews - Get All** - Could enhance the reviews page
5. **Assign Delivery Driver** - Useful for order management

---

## 📝 Notes

- Most missing endpoints are for **admin dashboard functionality**
- Mobile app endpoints appear to be complete
- Authorization attributes are commented out in many controllers (should be enabled for production)
- Consider adding Swagger/OpenAPI documentation once endpoints are implemented


