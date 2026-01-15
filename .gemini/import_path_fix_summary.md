# 🔧 Import Path Fix Summary

## Issue
All widget files in subdirectories (`common/`, `food/`, `category/`) are using incorrect import paths.

## Current (Wrong)
```dart
import '../../core/theme/app_colors.dart';
```

## Should Be
```dart
import '../../../core/theme/app_colors.dart';
```

## Files to Fix

### ✅ Already Fixed
- `custom_bottom_nav.dart`

### ⏳ Need to Fix
All files in these directories need `../../../core/theme/`:
- `lib/presentation/widgets/common/`
  - custom_button.dart
  - custom_app_bar.dart
  - rating_stars.dart
  - quantity_selector.dart
  - loading_indicator.dart
  - empty_state.dart

- `lib/presentation/widgets/food/`
  - food_card.dart

- `lib/presentation/widgets/category/`
  - category_chip.dart

### ✅ Correct (No Change Needed)
Files in `lib/presentation/widgets/` (root) use `../../core/theme/`:
- menu_item_card.dart
- category_card.dart

## Quick Fix Command
Run a find-replace in each subdirectory file to change:
- FROM: `import '../../core/theme/`
- TO: `import '../../../core/theme/`
