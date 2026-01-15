# ✅ All Systems Running - Connection Guide

> **Date:** January 10, 2026  
> **Status:** All components successfully connected

---

## 🚀 Running Services

| Service | URL | Status |
|---------|-----|--------|
| **API Server** | `http://localhost:5009` | ✅ Running |
| **Admin Dashboard** | `http://localhost:5119` | ✅ Running |
| **Flutter App** | Windows Desktop | ✅ Running |

---

## 🔐 Login Credentials

### Admin Dashboard
- **URL:** `http://localhost:5119`
- **Email:** `admin@restaurant.com`
- **Password:** `Admin@123`

### Flutter App (Customer)
- **Email:** `mazenmohsen11111@gmail.com`
- **Password:** `Password@123`

Or register a new account.

---

## 🔗 API Connection Configuration

### Admin Dashboard (Blazor)
**File:** `src/RestaurantApp.Web/Program.cs`
```csharp
builder.Services.AddHttpClient("RestaurantAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5009");
});
```
✅ **Connected to:** `http://localhost:5009`

### Flutter App
**File:** `mobile/restaurant_app/lib/core/constants/constants.dart`
```dart
static String get baseUrl {
  if (kIsWeb) return 'http://localhost:5009/api';
  try {
    if (Platform.isWindows) return 'http://localhost:5009/api';
    if (Platform.isAndroid) return 'http://10.0.2.2:5009/api';
  } catch (e) {}
  return 'http://localhost:5009/api';
}
```
✅ **Windows:** `http://localhost:5009/api`  
✅ **Android Emulator:** `http://10.0.2.2:5009/api`

---

## 🐛 Issue Fixed: JWT Configuration

### Problem
The API was crashing on startup with:
```
System.InvalidOperationException: JWT Secret Key is not configured
```

### Root Cause
The validation logic was too strict - it rejected the placeholder key even in Development mode.

### Solution
Updated `Program.cs` to only validate JWT key in Production:
```csharp
// Only validate in production - development can use appsettings key
if (builder.Environment.IsProduction() && 
    (string.IsNullOrEmpty(jwtKey) || jwtKey == "REPLACE_WITH_ENV_VARIABLE_IN_PRODUCTION"))
{
    throw new InvalidOperationException(
        "JWT Secret Key is not configured for production. Set JWT_SECRET_KEY environment variable.");
}
```

✅ **Development:** Uses key from `appsettings.Development.json`  
✅ **Production:** Requires `JWT_SECRET_KEY` environment variable

---

## 📋 How to Test

### 1. Admin Dashboard
1. Open browser: `http://localhost:5119`
2. Should redirect to `/admin/login`
3. Login with admin credentials
4. Should see dashboard with menu, orders, etc.

### 2. Flutter App (Windows)
1. App should show login screen
2. Login or register
3. Should load menu categories and items
4. Can browse menu, add to cart, place orders

### 3. API Endpoints
Test directly:
```powershell
# Get menu categories
curl http://localhost:5009/api/menu/categories

# Get all menu items
curl http://localhost:5009/api/menu/items
```

---

## 🔍 Troubleshooting

### If Admin Dashboard shows error:
1. Check API is running: `http://localhost:5009`
2. Check browser console for errors
3. Verify HttpClient configuration in `Program.cs`

### If Flutter App can't connect:
1. Check terminal for: `🚀 APP STARTING - API URL: http://localhost:5009/api`
2. Press `R` to hot restart
3. Verify API is accessible from browser

### If API won't start:
1. Check port 5009 is not in use
2. Verify `appsettings.Development.json` has JWT key
3. Check database connection string

---

## 📊 System Architecture

```
┌─────────────────┐
│  Flutter App    │
│  (Windows/      │
│   Android)      │
└────────┬────────┘
         │
         │ HTTP Requests
         │ localhost:5009/api
         ▼
┌─────────────────┐
│   API Server    │
│   .NET Core     │
│   Port: 5009    │
└────────┬────────┘
         │
         │ Entity Framework
         │
         ▼
┌─────────────────┐
│   SQL Server    │
│   LocalDB       │
└─────────────────┘
         ▲
         │
         │ Entity Framework
         │
┌────────┴────────┐
│ Admin Dashboard │
│   Blazor        │
│   Port: 5119    │
└─────────────────┘
```

---

## ✅ Verification Checklist

- [x] API Server running on port 5009
- [x] Admin Dashboard running on port 5119
- [x] Flutter App configured for localhost
- [x] JWT authentication working
- [x] Database seeded with test data
- [x] CORS configured for development
- [x] Exception handling middleware active
- [x] Request logging enabled

---

**All systems operational!** 🎉

You can now:
- Login to admin dashboard
- Use Flutter app to browse menu
- Place orders
- Manage restaurant from admin panel
