# Phase 2: Operations & Real-Time - Implementation Plan

## Overview
Phase 2 focuses on building the admin dashboard using Blazor Server, implementing real-time features with SignalR, and adding operational capabilities like offers, push notifications, and delivery fee calculation.

---

## Phase 2 Components

### 1. 🖥️ Blazor Admin Dashboard
**Priority: HIGH**

#### Features:
- [ ] Admin authentication & authorization
- [ ] Dashboard home with key metrics
- [ ] Order management (view, update status, cancel)
- [ ] Menu management (CRUD categories, items, add-ons)
- [ ] Branch management
- [ ] User management
- [ ] Offers & coupons management
- [ ] Reports & analytics

#### Pages to Create:
```
Components/
├── Layout/
│   ├── AdminLayout.razor       # Admin dashboard layout with sidebar
│   └── AdminLayout.razor.css
├── Pages/
│   ├── Admin/
│   │   ├── Dashboard.razor     # Main dashboard with KPIs
│   │   ├── Orders/
│   │   │   ├── Index.razor     # Orders list
│   │   │   └── Details.razor   # Order details & status update
│   │   ├── Menu/
│   │   │   ├── Categories.razor
│   │   │   ├── Items.razor
│   │   │   └── AddOns.razor
│   │   ├── Branches.razor
│   │   ├── Users.razor
│   │   ├── Offers.razor
│   │   └── Reports.razor
```

---

### 2. 📡 SignalR Real-Time Updates
**Priority: HIGH**

#### Features:
- [ ] OrderHub for real-time order updates
- [ ] Kitchen display integration
- [ ] Customer order tracking
- [ ] Admin notifications

#### Implementation:
```csharp
// API/Hubs/OrderHub.cs
public class OrderHub : Hub
{
    Task JoinOrderGroup(string orderId);
    Task JoinKitchenGroup(string branchId);
    Task JoinAdminGroup();
    Task SendOrderStatusUpdate(string orderId, OrderStatus status);
    Task SendNewOrderNotification(OrderDto order);
}
```

---

### 3. 🎁 Offers & Coupons System
**Priority: MEDIUM**

#### New Entities:
```csharp
// Domain/Entities/Offer.cs
public class Offer : BaseEntity
{
    public string Code { get; set; }
    public string NameAr { get; set; }
    public string NameEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public OfferType Type { get; set; }  // Percentage, FixedAmount, FreeDelivery
    public decimal Value { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public decimal? MaximumDiscount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public bool IsActive { get; set; }
}

// Domain/Enums/OfferType.cs
public enum OfferType
{
    Percentage,
    FixedAmount,
    FreeDelivery,
    BuyOneGetOne
}
```

#### API Endpoints:
- `POST /api/offers` - Create offer
- `GET /api/offers` - List offers
- `GET /api/offers/validate/{code}` - Validate coupon
- `PUT /api/offers/{id}` - Update offer
- `DELETE /api/offers/{id}` - Delete offer

---

### 4. 📱 Push Notifications (FCM)
**Priority: MEDIUM**

#### Features:
- [ ] FCM integration for order updates
- [ ] Device token management
- [ ] Notification templates
- [ ] Order status notifications
- [ ] Promotional notifications

#### Implementation:
```csharp
// Infrastructure/Services/NotificationService.cs
public interface INotificationService
{
    Task SendOrderStatusUpdate(string userId, Order order);
    Task SendPromotionalNotification(List<string> userIds, string title, string body);
    Task RegisterDevice(string userId, string deviceToken);
    Task UnregisterDevice(string deviceToken);
}
```

---

### 5. 📍 Delivery Fee Calculation
**Priority: MEDIUM**

#### Features:
- [ ] Distance-based pricing
- [ ] Zone-based pricing
- [ ] Minimum order for free delivery
- [ ] Time-based surcharges (peak hours)

#### Implementation:
```csharp
// Domain/Entities/DeliveryZone.cs
public class DeliveryZone : BaseEntity
{
    public int BranchId { get; set; }
    public string NameAr { get; set; }
    public string NameEn { get; set; }
    public decimal MinDistance { get; set; }
    public decimal MaxDistance { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal? MinimumOrderForFreeDelivery { get; set; }
    public int EstimatedMinutes { get; set; }
    public bool IsActive { get; set; }
}
```

---

### 6. 📊 Reports & Analytics
**Priority: LOW**

#### Features:
- [ ] Daily/weekly/monthly sales reports
- [ ] Popular items report
- [ ] Order statistics
- [ ] Revenue by branch
- [ ] Customer analytics

---

## Implementation Order

### Sprint 1 (Week 1-2): Admin Dashboard Foundation
1. Create AdminLayout with sidebar navigation
2. Implement admin authentication
3. Build Dashboard home page with KPIs
4. Create Orders management pages

### Sprint 2 (Week 2-3): Real-Time & Menu Management
1. Implement SignalR OrderHub
2. Add real-time order updates to dashboard
3. Build Menu management pages
4. Build Branch management page

### Sprint 3 (Week 3-4): Offers & Delivery
1. Create Offer entity and migrations
2. Build Offers API endpoints
3. Implement coupon validation
4. Build delivery zone management

### Sprint 4 (Week 4-5): Notifications & Reports
1. Integrate FCM for push notifications
2. Build notification templates
3. Create Reports pages
4. Build analytics dashboard

---

## Technology Stack

| Component | Technology |
|-----------|------------|
| Admin Dashboard | Blazor Server (.NET 10) |
| Real-Time | SignalR |
| Push Notifications | Firebase Cloud Messaging |
| CSS Framework | Bootstrap 5 |
| Charts | Chart.js or ApexCharts |
| Icons | Bootstrap Icons |

---

## Getting Started

### Step 1: Set up Admin Layout
Create the admin dashboard layout with:
- Responsive sidebar
- Top navigation bar
- Main content area
- Dark/light mode toggle

### Step 2: Configure Authentication
Add admin role checking and route protection.

### Step 3: Build Core Pages
Start with Dashboard and Orders pages.

---

## Files to Create/Modify

### New Files:
1. `Components/Layout/AdminLayout.razor`
2. `Components/Layout/AdminLayout.razor.css`
3. `Components/Pages/Admin/Dashboard.razor`
4. `Components/Pages/Admin/Orders/Index.razor`
5. `Components/Pages/Admin/Orders/Details.razor`
6. `API/Hubs/OrderHub.cs`
7. `Domain/Entities/Offer.cs`
8. `Domain/Entities/DeliveryZone.cs`
9. `Infrastructure/Services/NotificationService.cs`

### Modified Files:
1. `API/Program.cs` - Add SignalR
2. `Web/Program.cs` - Add services and auth
3. `Infrastructure/Data/AppDbContext.cs` - Add new entities
