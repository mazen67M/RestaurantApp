# Quick Fix for Const Issues

## Problem
The compilation is failing because we're using `const` with `AppDimensions` and `AppColors` in some places where it's not allowed.

## Solution
The values in `AppDimensions` and `AppColors` ARE const, but when used in certain contexts (like Size.fromHeight or inside non-const constructors), they can't be part of a const expression.

## Files to Fix

The issue is in these specific lines:

### custom_app_bar.dart
```dart
// Line 76 & 152 - Remove const
Size get preferredSize => Size.fromHeight(AppDimensions.appBarHeight);
```

### custom_button.dart  
Already fixed ✅

## Quick Fix Command

Run this to see all errors:
```bash
flutter analyze
```

Then fix each error by removing `const` where it's not allowed.

## Status
- ✅ custom_button.dart - Fixed
- ⏳ custom_app_bar.dart - Needs fix
- ⏳ Other files - Check with flutter analyze
