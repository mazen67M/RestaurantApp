# 🔒 Understanding 401 Unauthorized Errors

## ✅ **This is NORMAL and CORRECT!**

The 401 errors mean your API security is working properly. Most endpoints require authentication to protect user data.

---

## 📋 **How Authentication Works:**

### **Step 1: User Must Login**
```
POST /api/auth/login
Body: { "email": "admin@restaurant.com", "password": "Admin@123" }
```

### **Step 2: API Returns Token**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "userId": 1,
    "email": "admin@restaurant.com",
    "role": "Admin"
  }
}
```

### **Step 3: Flutter App Stores Token**
The `ApiService` automatically:
- Stores token in secure storage
- Includes token in all future requests
- Handles token expiration

### **Step 4: Protected Endpoints Work**
Once logged in, all endpoints work because token is included.

---

## 🌐 **Public Endpoints (No Login Required):**

### **Authentication:**
- ✅ `POST /api/auth/register` - Create account
- ✅ `POST /api/auth/login` - Login
- ✅ `POST /api/auth/forgot-password` - Reset password

### **Menu (Browse Only):**
- ✅ `GET /api/menu/categories` - List categories
- ✅ `GET /api/menu/items` - List all items
- ✅ `GET /api/menu/categories/{id}/items` - Items by category
- ✅ `GET /api/menu/items/{id}` - Item details
- ✅ `GET /api/menu/search?q=pizza` - Search items
- ✅ `GET /api/menu/popular` - Popular items

### **Restaurant Info:**
- ✅ `GET /api/restaurant` - Restaurant details
- ✅ `GET /api/branches` - List branches
- ✅ `GET /api/branches/{id}` - Branch details
- ✅ `GET /api/branches/nearest` - Nearest branch

### **Offers:**
- ✅ `GET /api/offers/active` - Active offers

---

## 🔐 **Protected Endpoints (Login Required):**

### **User Profile:**
- 🔒 `GET /api/auth/profile` - My profile
- 🔒 `PUT /api/auth/profile` - Update profile

### **Addresses:**
- 🔒 `GET /api/addresses` - My addresses
- 🔒 `POST /api/addresses` - Add address
- 🔒 `PUT /api/addresses/{id}` - Update address
- 🔒 `DELETE /api/addresses/{id}` - Delete address

### **Orders:**
- 🔒 `GET /api/orders` - My orders
- 🔒 `POST /api/orders` - Create order
- 🔒 `GET /api/orders/{id}` - Order details
- 🔒 `POST /api/orders/{id}/cancel` - Cancel order

### **Reviews:**
- 🔒 `GET /api/reviews/my` - My reviews
- 🔒 `POST /api/reviews` - Submit review
- 🔒 `PUT /api/reviews/{id}` - Update review
- 🔒 `DELETE /api/reviews/{id}` - Delete review

### **Loyalty:**
- 🔒 `GET /api/loyalty` - My points
- 🔒 `GET /api/loyalty/history` - Points history
- 🔒 `POST /api/loyalty/redeem` - Redeem points

### **Favorites:**
- 🔒 `GET /api/favorites` - My favorites
- 🔒 `POST /api/favorites/{itemId}` - Add favorite
- 🔒 `DELETE /api/favorites/{itemId}` - Remove favorite

---

## 🎯 **In Your Flutter App:**

### **Current Flow (Correct):**

1. **User opens app** → Can browse menu (public endpoints)
2. **User tries to checkout** → Redirected to login
3. **User logs in** → Token saved automatically
4. **User can now:**
   - Place orders
   - Manage addresses
   - View order history
   - Submit reviews
   - Use loyalty points
   - Save favorites

### **Why You See 401 Errors:**

If you're testing endpoints directly (Postman, browser, etc.) **without logging in first**, you'll get 401. This is correct!

**In the Flutter app**, the flow works because:
- Login screen saves the token
- ApiService includes token automatically
- User doesn't see 401 errors

---

## 🧪 **Testing Guide:**

### **Test Public Endpoint (Works Without Login):**
```powershell
Invoke-RestMethod -Uri "http://localhost:5009/api/menu/categories"
```
**Result:** ✅ Returns categories

### **Test Protected Endpoint Without Token:**
```powershell
Invoke-RestMethod -Uri "http://localhost:5009/api/orders"
```
**Result:** ❌ 401 Unauthorized (Expected!)

### **Test Protected Endpoint With Token:**
```powershell
# 1. Login first
$login = Invoke-RestMethod -Uri "http://localhost:5009/api/auth/login" `
  -Method Post `
  -Body (@{email="admin@restaurant.com"; password="Admin@123"} | ConvertTo-Json) `
  -ContentType "application/json"

# 2. Get token
$token = $login.data.token

# 3. Use token for protected endpoint
$headers = @{Authorization = "Bearer $token"}
Invoke-RestMethod -Uri "http://localhost:5009/api/orders" -Headers $headers
```
**Result:** ✅ Returns orders

---

## ✅ **Your App is Working Correctly!**

### **What's Happening:**

1. ✅ **Public endpoints work** (menu, branches, offers)
2. ✅ **Login works** (tested successfully)
3. ✅ **Protected endpoints require auth** (security working!)
4. ✅ **Flutter app handles tokens automatically**

### **User Experience:**

```
User Flow:
1. Browse menu → Works (no login needed)
2. Add to cart → Works (no login needed)
3. Checkout → Login required
4. Login → Token saved
5. Place order → Works (token included)
6. View orders → Works (token included)
```

---

## 🎯 **Test Credentials:**

### **Admin Account:**
- Email: `admin@restaurant.com`
- Password: `Admin@123`
- Role: Admin (full access)

### **Regular User:**
- Email: `user@restaurant.com`
- Password: `User@123`
- Role: Customer

---

## 🔍 **Troubleshooting:**

### **If Login Fails in Flutter App:**

1. **Check API is running:**
   ```
   http://192.168.1.13:5009/api/menu/categories
   ```

2. **Check credentials:**
   - Email: `admin@restaurant.com`
   - Password: `Admin@123`

3. **Check network:**
   - Phone and PC on same WiFi
   - Firewall allows port 5009

### **If 401 After Login:**

1. **Token might be expired** (expires in 7 days)
   - Solution: Login again

2. **Token not saved properly**
   - Check FlutterSecureStorage permissions

---

## 🎉 **Summary:**

✅ **401 errors are GOOD** - They mean your API is secure  
✅ **Public endpoints work** - Anyone can browse menu  
✅ **Protected endpoints require login** - User data is safe  
✅ **Flutter app handles this automatically** - Users don't see 401s  
✅ **Login works perfectly** - Tested and confirmed  

**Your application security is working exactly as it should!** 🔒
