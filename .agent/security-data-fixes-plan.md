# Security & Data Access Fixes - Implementation Plan

**Review Date:** January 20, 2026  
**Sections:** 3️⃣ Security (TOP PRIORITY), 4️⃣ Data Access & Persistence  
**Status:** Ready for Implementation

---

## 🎯 Objectives

Fix critical security vulnerabilities and data access issues identified in the backend production review before moving to performance and monitoring sections.

---

## 3️⃣ Security (TOP PRIORITY)

### 🔴 Critical Security Issues

#### Issue 3.1: No Refresh Tokens ⚠️ CRITICAL
**Severity:** Critical (A07:2021 - Identification and Authentication Failures)  
**Current State:**
- JWT expires in 7 days with no rotation
- No refresh token mechanism
- Users must re-login after token expiry
- No way to revoke access without blacklist

**Action:** Implement refresh token mechanism

**Implementation Steps:**
1. Create `RefreshToken` entity in Domain
2. Add `RefreshTokens` DbSet to ApplicationDbContext
3. Create migration for RefreshTokens table
4. Update `IAuthService` interface with refresh token methods
5. Implement refresh token generation in `AuthService`
6. Create `RefreshTokenDto` and validation
7. Add `/api/auth/refresh` endpoint
8. Update `/api/auth/login` to return refresh token
9. Add refresh token rotation on use
10. Add refresh token revocation on logout

**Files to create/modify:**
- `Domain/Entities/RefreshToken.cs` (NEW)
- `Infrastructure/Data/ApplicationDbContext.cs` (MODIFY)
- `Application/Interfaces/IAuthService.cs` (MODIFY)
- `Infrastructure/Services/AuthService.cs` (MODIFY)
- `Application/DTOs/Auth/AuthDtos.cs` (MODIFY)
- `API/Controllers/AuthController.cs` (MODIFY)
- Migration file (NEW)

---

#### Issue 3.2: No File Size Limit on Uploads ⚠️ CRITICAL
**Severity:** Critical (A05:2021 - Security Misconfiguration)  
**Current State:**
- File uploads have no size validation
- Potential DoS via large file uploads
- Magic bytes validation exists but no size check

**Action:** Add file size limits with `[RequestSizeLimit]` attribute

**Implementation Steps:**
1. Find all file upload endpoints
2. Add `[RequestSizeLimit(5 * 1024 * 1024)]` attribute (5MB for images)
3. Add file size validation in services
4. Return proper error messages for oversized files

**Files to modify:**
- Controllers with file upload endpoints
- File upload services

---

#### Issue 3.3: Console.WriteLine in Production Code 🟠 HIGH
**Severity:** Medium (A09:2021 - Security Logging and Monitoring Failures)  
**Current State:**
- `Console.WriteLine` statements in AuthService
- Sensitive data potentially logged to console
- Should use ILogger instead

**Action:** Replace all Console.WriteLine with ILogger

**Implementation Steps:**
1. Search for all `Console.WriteLine` in codebase
2. Replace with appropriate `ILogger` calls
3. Ensure sensitive data is not logged

**Files to audit:**
- `Infrastructure/Services/AuthService.cs`
- Any other services with console logging

---

#### Issue 3.4: AllowedHosts: "*" 🟠 HIGH
**Severity:** Medium (A05:2021 - Security Misconfiguration)  
**Current State:**
- `appsettings.json` has `AllowedHosts: "*"`
- Accepts requests from any host
- Potential host header injection

**Action:** Configure specific allowed hosts

**Implementation Steps:**
1. Update `appsettings.json` with specific hosts
2. Update `appsettings.Production.json` with production hosts
3. Document host configuration

**Files to modify:**
- `API/appsettings.json`
- `API/appsettings.Production.json`

---

#### Issue 3.5: JWT Key in appsettings.Development.json 🟠 HIGH
**Severity:** High (A02:2021 - Cryptographic Failures)  
**Current State:**
- Development JWT key hardcoded in appsettings
- Should use user secrets for development

**Action:** Move to user secrets

**Implementation Steps:**
1. Remove JWT key from `appsettings.Development.json`
2. Add to user secrets
3. Document setup in README

**Files to modify:**
- `API/appsettings.Development.json`

---

### 🟡 Additional Security Enhancements

#### Issue 3.6: No Audit Logging 🟠 MEDIUM
**Severity:** Medium (Compliance risk)  
**Action:** Implement audit logging for sensitive operations

**Implementation Steps:**
1. Create `IAuditService` interface
2. Create `AuditLog` entity
3. Implement `AuditService`
4. Add audit logging to:
   - User registration/login
   - Password changes
   - Order creation/status changes
   - Admin actions (create/update/delete)
5. Create audit log query endpoints for admins

**Files to create:**
- `Application/Interfaces/IAuditService.cs` (NEW)
- `Domain/Entities/AuditLog.cs` (NEW)
- `Infrastructure/Services/AuditService.cs` (NEW)
- `API/Controllers/AuditController.cs` (NEW)

---

#### Issue 3.7: Password Reset Token in URL 🟡 MEDIUM
**Severity:** Medium  
**Current State:**
- Reset token sent as Base64 in URL
- Not encrypted, just encoded

**Action:** Document as acceptable risk (standard practice) or encrypt tokens

**Decision:** Keep current implementation (standard practice for email-based reset)

---

## 4️⃣ Data Access & Persistence

### Issue 4.1: N+1 Query Risk in GetOrders 🟠 HIGH
**Severity:** High (Performance degradation)  
**Current State:**
- Multiple includes with nested ThenInclude
- Potential N+1 queries at scale

**Action:** Enable split queries and optimize includes

**Implementation Steps:**
1. Analyze `GetOrdersAsync` query in OrderService
2. Enable `AsSplitQuery()` for complex includes
3. Profile queries to verify improvement
4. Add query optimization comments

**Files to modify:**
- `Infrastructure/Services/OrderService.cs`

---

### Issue 4.2: No Database Indexes Defined 🟠 HIGH
**Severity:** High (Performance degradation at scale)  
**Current State:**
- No explicit indexes on foreign keys
- No indexes on filter columns
- EF Core creates some indexes automatically

**Action:** Add database indexes for commonly queried columns

**Implementation Steps:**
1. Add indexes in `ApplicationDbContext.OnModelCreating`:
   - `Order.UserId`
   - `Order.BranchId`
   - `Order.Status`
   - `Order.CreatedAt`
   - `Review.MenuItemId`
   - `Review.IsApproved`
   - `MenuItem.CategoryId`
   - `MenuItem.IsAvailable`
   - `LoyaltyTransaction.UserId`
   - `OrderStatusHistory.OrderId`
2. Create migration for indexes
3. Document index strategy

**Files to modify:**
- `Infrastructure/Data/ApplicationDbContext.cs`
- New migration file

---

### Issue 4.3: No Connection Pooling Config 🟡 MEDIUM
**Severity:** Medium  
**Current State:**
- Using default connection pooling
- No retry logic configured

**Action:** Configure connection resiliency

**Implementation Steps:**
1. Add retry logic in `AddInfrastructure` method
2. Configure command timeout
3. Enable connection pooling explicitly

**Files to modify:**
- `Infrastructure/DependencyInjection.cs` or where DbContext is configured

---

### Issue 4.4: Migrations Warning Suppressed 🟡 LOW
**Severity:** Low  
**Current State:**
- `PendingModelChangesWarning` ignored in migrations

**Action:** Document and verify migrations are up to date

**Implementation Steps:**
1. Run `dotnet ef migrations list`
2. Verify no pending model changes
3. Document migration process

---

## 📋 Implementation Checklist

### Phase 1: Critical Security Fixes (Do First)
- [ ] Implement refresh token mechanism (Issue 3.1)
- [ ] Add file size limits on uploads (Issue 3.2)
- [ ] Remove Console.WriteLine statements (Issue 3.3)
- [ ] Configure specific AllowedHosts (Issue 3.4)
- [ ] Move JWT key to user secrets (Issue 3.5)

### Phase 2: Data Access Optimizations (Do Second)
- [ ] Add database indexes (Issue 4.2)
- [ ] Enable split queries for complex includes (Issue 4.1)
- [ ] Configure connection resiliency (Issue 4.3)

### Phase 3: Enhancements (Do Third)
- [ ] Implement audit logging (Issue 3.6)
- [ ] Verify migrations status (Issue 4.4)

---

## 🎯 Success Criteria

1. ✅ Refresh token mechanism fully implemented and tested
2. ✅ File upload size limits enforced (5MB for images)
3. ✅ No Console.WriteLine in production code
4. ✅ AllowedHosts configured for production
5. ✅ JWT secrets in user secrets (development)
6. ✅ Database indexes created for performance
7. ✅ Connection resiliency configured
8. ✅ Audit logging implemented for sensitive operations
9. ✅ Application builds successfully
10. ✅ All tests pass

---

## 📊 Expected Impact

### Security Improvements:
- **Critical vulnerabilities fixed:** 2 (refresh tokens, file size limits)
- **High-risk issues fixed:** 3 (console logging, allowed hosts, JWT secrets)
- **Medium-risk issues fixed:** 1 (audit logging)

### Performance Improvements:
- **Query optimization:** Split queries for complex includes
- **Database indexes:** 10+ indexes added
- **Connection resiliency:** Retry logic and pooling configured

### Production Readiness Score:
- **Current:** ~7.2 / 10
- **After implementation:** ~8.5 / 10 (+1.3 improvement)
- **Risk Level:** Medium → Low

---

**Ready to begin implementation!**
