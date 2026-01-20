# Orders Dashboard Enhancement Plan

## Features to Implement

### 1. Actual Delivery Assignment ✓
**Backend:**
- Update `IDeliveryService` interface with `AssignToOrderAsync` method
- Implement in `DeliveryService` to update Order.DeliveryId
- Update `AdminOrdersController.AssignDelivery` to call service

**Database:**
- Order table already has `DeliveryId` field (nullable)

### 2. Order Filters (Date Range, Customer, Status) ✓
**Frontend:**
- Add date range picker (From/To dates)
- Add customer search input
- Status filter already exists
- Add "Apply Filters" and "Clear Filters" buttons

**Backend:**
- Update `GetAllOrders` to accept additional parameters
- Add filtering logic in `OrderService.GetOrdersAsync`

### 3. Export Orders to Excel/PDF ✓
**Backend:**
- Install EPPlus (Excel) and QuestPDF (PDF) packages
- Create `OrderExportService`
- Add endpoints: `/api/admin/orders/export/excel` and `/export/pdf`

**Frontend:**
- Add Export dropdown button with Excel/PDF options
- Download file on click

### 4. Order Search (by Order Number) ✓
**Frontend:**
- Search box already exists, enhance to search by order number
- Add real-time search (debounced)

**Backend:**
- Update query to include order number search

### 5. Kitchen Display Screen ✓
**New Page:**
- Create `/admin/kitchen` route
- Real-time SignalR updates
- Show only Confirmed/Preparing orders
- Large cards with order details
- "Mark as Ready" button
- Auto-refresh every 30 seconds
- Sound notification for new orders

## Implementation Order

1. ✅ Delivery Assignment (Backend + Frontend)
2. ✅ Enhanced Filters (Frontend + Backend)
3. ✅ Order Search Enhancement
4. ✅ Export to Excel/PDF
5. ✅ Kitchen Display Screen

## Files to Create/Modify

**Backend:**
- `IDeliveryService.cs` - Add AssignToOrderAsync
- `DeliveryService.cs` - Implement assignment
- `AdminOrdersController.cs` - Update AssignDelivery endpoint
- `OrderExportService.cs` - NEW
- `RestaurantApp.API.csproj` - Add packages

**Frontend:**
- `Orders/Index.razor` - Enhanced filters, search, export
- `Kitchen/Index.razor` - NEW kitchen display
- `AdminSidebar.razor` - Add Kitchen link
- `Routes.razor` - Add kitchen route
