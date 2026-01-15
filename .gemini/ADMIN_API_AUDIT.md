# 🔍 Admin Dashboard API Audit & Fix Plan

## 🚨 **Issues Found:**

### **1. Dashboard Showing Fake Data**
- Reviews page: Using demo data
- Loyalty page: Using demo data  
- Reports page: Using demo data

### **2. Missing Admin CRUD Endpoints:**
- ❌ **Branches**: No Create, Update, Delete endpoints
- ❌ **Categories**: Endpoints exist but not connected to admin UI
- ❌ **Users**: No admin user management endpoints
- ❌ **Dashboard Stats**: No real-time statistics endpoint

---

## 📊 **Current API Endpoint Status:**

### ✅ **Complete (Has Full CRUD):**
| Entity | GET | POST | PUT | DELETE | Notes |
|--------|-----|------|-----|--------|-------|
| Menu Items | ✅ | ✅ | ✅ | ✅ | All endpoints exist |
| Categories | ✅ | ✅ | ✅ | ✅ | All endpoints exist |
| Orders | ✅ | ✅ | ✅ | ❌ | Can't delete orders (correct) |
| Addresses | ✅ | ✅ | ✅ | ✅ | All endpoints exist |
| Offers | ✅ | ✅ | ✅ | ✅ | All endpoints exist |
| Reviews | ✅ | ✅ | ✅ | ✅ | All endpoints exist |
| Favorites | ✅ | ✅ | ❌ | ✅ | No update needed |

### ⚠️ **Incomplete (Missing Admin CRUD):**
| Entity | GET | POST | PUT | DELETE | Missing |
|--------|-----|------|-----|--------|---------|
| **Branches** | ✅ | ❌ | ❌ | ❌ | Create, Update, Delete |
| **Users** | ❌ | ✅ | ❌ | ❌ | List, Update, Delete |
| **Restaurant** | ✅ | ❌ | ❌ | ❌ | Update restaurant info |

### ❌ **Missing Entirely:**
| Endpoint | Purpose | Priority |
|----------|---------|----------|
| `/api/admin/stats` | Dashboard statistics | HIGH |
| `/api/admin/users` | User management | MEDIUM |
| `/api/admin/reports` | Sales reports | MEDIUM |

---

## 🔧 **Fix Plan:**

### **Priority 1: Add Missing Branch Endpoints**
```csharp
POST   /api/branches              - Create branch
PUT    /api/branches/{id}         - Update branch
DELETE /api/branches/{id}         - Delete branch
```

### **Priority 2: Add Dashboard Stats Endpoint**
```csharp
GET /api/admin/dashboard/stats    - Real-time dashboard data
```

### **Priority 3: Add User Management**
```csharp
GET    /api/admin/users           - List all users
GET    /api/admin/users/{id}      - User details
PUT    /api/admin/users/{id}      - Update user
DELETE /api/admin/users/{id}      - Delete user
POST   /api/admin/users/{id}/toggle-status - Enable/disable user
```

### **Priority 4: Add Reports Endpoint**
```csharp
GET /api/admin/reports/sales      - Sales report
GET /api/admin/reports/items      - Top selling items
GET /api/admin/reports/branches   - Branch performance
```

### **Priority 5: Connect Admin UI to Real APIs**
- Update Dashboard page to use `/api/admin/dashboard/stats`
- Update Reviews page to remove demo data fallback
- Update Loyalty page to remove demo data fallback
- Update Reports page to use real endpoints

---

## 📝 **Implementation Steps:**

### **Step 1: Add Branch CRUD Endpoints**
File: `RestaurantController.cs`
- Add CreateBranch
- Add UpdateBranch
- Add DeleteBranch

### **Step 2: Create AdminController**
File: `AdminController.cs` (NEW)
- Add GetDashboardStats
- Add GetUsers
- Add UpdateUser
- Add DeleteUser
- Add GetSalesReport

### **Step 3: Update Admin Dashboard Pages**
- `Dashboard/Index.razor` - Connect to stats API
- `Reviews/Index.razor` - Remove demo fallback
- `Loyalty/Index.razor` - Remove demo fallback
- `Reports/Index.razor` - Connect to reports API

### **Step 4: Add Missing Services**
- `IAdminService` interface
- `AdminService` implementation
- Register in Program.cs

---

## 🎯 **Expected Outcome:**

After fixes:
- ✅ All admin pages show real data
- ✅ Branches can be created/edited/deleted
- ✅ Categories can be managed
- ✅ Dashboard shows real statistics
- ✅ Reports show actual data
- ✅ No more demo/fake data

---

## 📋 **Files to Create/Modify:**

### **New Files:**
1. `API/Controllers/AdminController.cs`
2. `Application/Interfaces/IAdminService.cs`
3. `Infrastructure/Services/AdminService.cs`
4. `Application/DTOs/Admin/AdminDtos.cs`

### **Files to Modify:**
5. `API/Controllers/RestaurantController.cs` - Add branch CRUD
6. `API/Program.cs` - Register AdminService
7. `Web/Components/Pages/Admin/Dashboard/Index.razor` - Use real API
8. `Web/Components/Pages/Admin/Reviews/Index.razor` - Remove demo fallback
9. `Web/Components/Pages/Admin/Loyalty/Index.razor` - Remove demo fallback
10. `Web/Components/Pages/Admin/Reports/Index.razor` - Use real API

---

**Total Estimated Time:** 4-5 hours
**Priority:** HIGH - Required for client presentation
