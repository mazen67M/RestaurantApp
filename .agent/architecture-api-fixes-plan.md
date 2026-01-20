# Architecture & API Design Fixes - Implementation Plan

**Review Date:** January 20, 2026  
**Sections:** 1️⃣ Architecture & System Design, 2️⃣ API Design & Contracts  
**Status:** Ready for Implementation

---

## 🎯 Objectives

Fix critical architecture and API design issues identified in the backend production review before moving to security enhancements.

---

## 1️⃣ Architecture & System Design Fixes

### Issue 1.1: OrderService God Class (639 lines) ✅ ALREADY ADDRESSED
**Status:** Partially refactored in Phase 4
- `CreateOrderUseCase` already exists
- `UpdateOrderStatusUseCase` already exists
- **Action:** Verify use cases are being used, document completion

### Issue 1.2: Direct DbContext Usage
**Severity:** Low  
**Action:** Audit services for direct DbContext usage
- Review all services in `Infrastructure/Services`
- Identify services bypassing repository pattern
- Document findings (may defer fix to later phase)

### Issue 1.3: Missing Domain Events
**Severity:** Medium  
**Action:** Design domain events architecture
- Create `IDomainEvent` interface
- Create `DomainEventDispatcher` service
- Identify key events: `OrderCreated`, `OrderStatusChanged`, `PaymentProcessed`
- **Defer to Phase 5** (nice-to-have improvement)

---

## 2️⃣ API Design & Contracts Fixes

### Issue 2.1: No Request Body Size Limits ⚠️ CRITICAL
**Severity:** High (DoS vulnerability)  
**Action:** Add request size limits to Program.cs

```csharp
// Add to Program.cs
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB global limit
});

// Add attribute for file uploads
[RequestSizeLimit(5 * 1024 * 1024)] // 5MB for images
```

**Files to modify:**
- `src/RestaurantApp.API/Program.cs`
- Controllers with file uploads (if any)

---

### Issue 2.2: Mixed Response Formats ⚠️ HIGH PRIORITY
**Severity:** Medium  
**Current State:**
- `ExceptionHandlingMiddleware` uses custom `ErrorResponse` class
- RFC 7807 `ProblemDetails` is configured but not used in middleware
- Services use `ApiResponse<T>` wrapper

**Action:** Unify all error responses to RFC 7807 ProblemDetails

**Files to modify:**
1. `src/RestaurantApp.API/Middleware/ExceptionHandlingMiddleware.cs`
   - Replace `ErrorResponse` with `ProblemDetails`
   - Use `ProblemDetailsFactory` from Application layer

2. Create `ProblemDetailsFactory` helper (if not exists)
   - `CreateValidationProblem()` - already exists ✅
   - `CreateExceptionProblem()` - needs creation
   - `CreateNotFoundProblem()`
   - `CreateUnauthorizedProblem()`

**Implementation Steps:**
1. Update `ExceptionHandlingMiddleware` to return RFC 7807 format
2. Ensure all exception types map to correct ProblemDetails
3. Test error responses match RFC 7807 spec

---

### Issue 2.3: DTO Validation Not Comprehensive ⚠️ HIGH PRIORITY
**Severity:** Medium  
**Current State:**
- Only 1 validator found: `CreateOrderDtoValidator.cs`
- Many DTOs lack FluentValidation validators

**Action:** Create validators for all input DTOs

**DTOs needing validators:**
1. **Auth DTOs:**
   - `LoginDto`
   - `RegisterDto`
   - `ResetPasswordDto`
   - `ChangePasswordDto`

2. **Menu DTOs:**
   - `CreateMenuCategoryDto`
   - `UpdateMenuCategoryDto`
   - `CreateMenuItemDto`
   - `UpdateMenuItemDto`

3. **Restaurant DTOs:**
   - `CreateRestaurantDto`
   - `UpdateRestaurantDto`
   - `CreateBranchDto`
   - `UpdateBranchDto`

4. **Offer DTOs:**
   - `CreateOfferDto`
   - `UpdateOfferDto`

5. **Review DTOs:**
   - `CreateReviewDto`
   - `UpdateReviewDto`

6. **User DTOs:**
   - `UpdateUserDto`

**Implementation Steps:**
1. Create validator files in `Application/Validators/[Domain]/`
2. Follow pattern from `CreateOrderDtoValidator`
3. Add validation rules for:
   - Required fields
   - String length limits
   - Email format
   - Phone format
   - Numeric ranges
   - Date validations
   - Business rules

---

### Issue 2.4: Inconsistent HTTP Status Codes
**Severity:** Medium  
**Action:** Audit controller error handling

**Common Issues:**
- Validation errors returning 500 instead of 400
- Business logic errors returning 500 instead of 422
- Not found errors inconsistent

**Files to review:**
- All controllers in `src/RestaurantApp.API/Controllers/`
- Ensure proper status codes:
  - 200 OK - Success
  - 201 Created - Resource created
  - 204 No Content - Success with no body
  - 400 Bad Request - Validation errors
  - 401 Unauthorized - Authentication required
  - 403 Forbidden - Insufficient permissions
  - 404 Not Found - Resource not found
  - 422 Unprocessable Entity - Business rule violation
  - 500 Internal Server Error - Unexpected errors only

---

## 📋 Implementation Checklist

### Phase 1: Critical Fixes (Do First)
- [ ] Add request body size limits (Issue 2.1)
- [ ] Unify error responses to RFC 7807 (Issue 2.2)
- [ ] Create Auth DTOs validators (Issue 2.3)
- [ ] Create Menu DTOs validators (Issue 2.3)

### Phase 2: High Priority (Do Second)
- [ ] Create Restaurant DTOs validators (Issue 2.3)
- [ ] Create Offer DTOs validators (Issue 2.3)
- [ ] Create Review DTOs validators (Issue 2.3)
- [ ] Audit HTTP status codes (Issue 2.4)

### Phase 3: Documentation & Verification
- [ ] Verify OrderService refactoring status (Issue 1.1)
- [ ] Document direct DbContext usage (Issue 1.2)
- [ ] Test all error responses
- [ ] Update API documentation
- [ ] Build and verify no errors

---

## 🎯 Success Criteria

1. ✅ Request size limits enforced (10MB global, 5MB for files)
2. ✅ All error responses follow RFC 7807 ProblemDetails format
3. ✅ All input DTOs have FluentValidation validators
4. ✅ HTTP status codes are consistent and correct
5. ✅ Application builds successfully
6. ✅ No breaking changes to existing functionality

---

## 📝 Notes

- Domain Events (Issue 1.3) deferred to Phase 5 as "nice-to-have"
- Direct DbContext audit (Issue 1.2) for documentation only
- Focus on API contract consistency and security
- All changes must maintain backward compatibility

---

**Ready for user review before implementation**
