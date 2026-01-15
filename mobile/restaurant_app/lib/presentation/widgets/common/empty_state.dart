import 'package:flutter/material.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/theme/app_dimensions.dart';

class EmptyState extends StatelessWidget {
  final IconData icon;
  final String title;
  final String message;
  final String? actionText;
  final VoidCallback? onActionPressed;

  const EmptyState({
    super.key,
    required this.icon,
    required this.title,
    required this.message,
    this.actionText,
    this.onActionPressed,
  });

  const EmptyState.cart({
    super.key,
    this.actionText = 'Browse Menu',
    this.onActionPressed,
  })  : icon = Icons.shopping_cart_outlined,
        title = 'Your cart is empty',
        message = 'Add items to your cart to get started';

  const EmptyState.favorites({
    super.key,
    this.actionText = 'Explore Menu',
    this.onActionPressed,
  })  : icon = Icons.favorite_outline,
        title = 'No favorites yet',
        message = 'Save your favorite items for quick access';

  const EmptyState.orders({
    super.key,
    this.actionText = 'Order Now',
    this.onActionPressed,
  })  : icon = Icons.receipt_long_outlined,
        title = 'No orders yet',
        message = 'Place your first order and track it here';

  const EmptyState.search({
    super.key,
    this.actionText,
    this.onActionPressed,
  })  : icon = Icons.search_off_outlined,
        title = 'No results found',
        message = 'Try adjusting your search or filters';

  const EmptyState.addresses({
    super.key,
    this.actionText = 'Add Address',
    this.onActionPressed,
  })  : icon = Icons.location_on_outlined,
        title = 'No saved addresses',
        message = 'Add a delivery address to get started';

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(AppDimensions.paddingXl),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            // Icon
            Container(
              width: 120,
              height: 120,
              decoration: BoxDecoration(
                color: AppColors.bgSecondary,
                shape: BoxShape.circle,
              ),
              child: Icon(
                icon,
                size: 60,
                color: AppColors.textLight,
              ),
            ),
            
            const SizedBox(height: AppDimensions.spaceLg),
            
            // Title
            Text(
              title,
              style: AppTypography.h3.copyWith(
                color: AppColors.textPrimary,
              ),
              textAlign: TextAlign.center,
            ),
            
            const SizedBox(height: AppDimensions.spaceSm),
            
            // Message
            Text(
              message,
              style: AppTypography.bodyMedium.copyWith(
                color: AppColors.textSecondary,
              ),
              textAlign: TextAlign.center,
            ),
            
            // Action Button
            if (actionText != null && onActionPressed != null) ...[
              const SizedBox(height: AppDimensions.spaceLg),
              ElevatedButton(
                onPressed: onActionPressed,
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.primaryOrange,
                  foregroundColor: AppColors.textWhite,
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppDimensions.paddingXl,
                    vertical: AppDimensions.paddingMd,
                  ),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(AppDimensions.radiusMd),
                  ),
                ),
                child: Text(
                  actionText!,
                  style: AppTypography.button,
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
