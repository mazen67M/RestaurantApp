import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:cached_network_image/cached_network_image.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/theme/app_dimensions.dart';
import '../../../core/localization/app_localizations.dart';
import '../../../data/providers/cart_provider.dart';
import '../../../data/providers/locale_provider.dart';
import '../../widgets/common/custom_app_bar.dart';
import '../../widgets/common/custom_button.dart';
import '../../widgets/common/quantity_selector.dart';
import '../../widgets/common/empty_state.dart';
import '../checkout/checkout_screen.dart';

class CartScreen extends StatefulWidget {
  const CartScreen({super.key});

  @override
  State<CartScreen> createState() => _CartScreenState();
}

class _CartScreenState extends State<CartScreen> {
  final TextEditingController _promoController = TextEditingController();
  bool _isApplyingPromo = false;

  @override
  void dispose() {
    _promoController.dispose();
    super.dispose();
  }

  void _applyPromoCode(CartProvider cartProvider) {
    if (_promoController.text.isEmpty) return;

    setState(() => _isApplyingPromo = true);

    // Simulate API call
    Future.delayed(const Duration(seconds: 1), () {
      // TODO: Implement actual promo code validation
      setState(() => _isApplyingPromo = false);
      
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Promo code "${_promoController.text}" applied!'),
          backgroundColor: AppColors.accentGreen,
        ),
      );
    });
  }

  @override
  Widget build(BuildContext context) {
    final isArabic = context.watch<LocaleProvider>().isArabic;
    final cartProvider = context.watch<CartProvider>();

    return Scaffold(
      backgroundColor: AppColors.bgSecondary,
      appBar: CustomAppBar(
        title: 'My Cart',
        actions: [
          if (cartProvider.isNotEmpty)
            TextButton(
              onPressed: () => _showClearCartDialog(context, cartProvider),
              child: Text(
                'Clear',
                style: AppTypography.bodyMedium.copyWith(
                  color: AppColors.accentRed,
                  fontWeight: AppTypography.fontSemiBold,
                ),
              ),
            ),
        ],
      ),
      body: cartProvider.isEmpty
          ? const EmptyState.cart(
              actionText: 'Browse Menu',
              onActionPressed: null, // Will pop to previous screen
            )
          : _buildCartContent(context, cartProvider, isArabic),
    );
  }

  Widget _buildCartContent(
    BuildContext context,
    CartProvider cartProvider,
    bool isArabic,
  ) {
    return Column(
      children: [
        // Cart Items List
        Expanded(
          child: ListView.builder(
            padding: const EdgeInsets.all(AppDimensions.paddingMd),
            itemCount: cartProvider.items.length,
            itemBuilder: (context, index) {
              final item = cartProvider.items[index];
              return _buildCartItem(
                context,
                item,
                index,
                cartProvider,
                isArabic,
              );
            },
          ),
        ),

        // Promo Code Section
        _buildPromoSection(cartProvider),

        // Order Summary
        _buildOrderSummary(context, cartProvider),
      ],
    );
  }

  Widget _buildCartItem(
    BuildContext context,
    dynamic item,
    int index,
    CartProvider cartProvider,
    bool isArabic,
  ) {
    return Container(
      margin: const EdgeInsets.only(bottom: AppDimensions.spaceMd),
      decoration: BoxDecoration(
        color: AppColors.bgPrimary,
        borderRadius: BorderRadius.circular(AppDimensions.radiusMd),
        boxShadow: [
          BoxShadow(
            color: AppColors.shadowLight,
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Padding(
        padding: const EdgeInsets.all(AppDimensions.paddingMd),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Image
            ClipRRect(
              borderRadius: BorderRadius.circular(AppDimensions.radiusSm),
              child: item.menuItem.imageUrl != null
                  ? CachedNetworkImage(
                      imageUrl: item.menuItem.imageUrl!,
                      width: 80,
                      height: 80,
                      fit: BoxFit.cover,
                      placeholder: (_, __) => Container(
                        width: 80,
                        height: 80,
                        color: AppColors.bgSecondary,
                      ),
                      errorWidget: (_, __, ___) => Container(
                        width: 80,
                        height: 80,
                        color: AppColors.bgSecondary,
                        child: const Icon(
                          Icons.restaurant,
                          color: AppColors.textLight,
                        ),
                      ),
                    )
                  : Container(
                      width: 80,
                      height: 80,
                      color: AppColors.bgSecondary,
                      child: const Icon(
                        Icons.restaurant,
                        color: AppColors.textLight,
                      ),
                    ),
            ),

            const SizedBox(width: AppDimensions.spaceMd),

            // Details
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Expanded(
                        child: Text(
                          item.menuItem.getName(isArabic),
                          style: AppTypography.h4,
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                      IconButton(
                        icon: const Icon(
                          Icons.delete_outline,
                          color: AppColors.accentRed,
                        ),
                        onPressed: () => cartProvider.removeItem(index),
                        padding: EdgeInsets.zero,
                        constraints: const BoxConstraints(),
                      ),
                    ],
                  ),

                  if (item.selectedAddOns.isNotEmpty) ...[
                    const SizedBox(height: AppDimensions.spaceXs),
                    Text(
                      item.selectedAddOns
                          .map((a) => a.getName(isArabic))
                          .join(', '),
                      style: AppTypography.bodySmall.copyWith(
                        color: AppColors.textSecondary,
                      ),
                    ),
                  ],

                  if (item.notes != null && item.notes!.isNotEmpty) ...[
                    const SizedBox(height: AppDimensions.spaceXs),
                    Text(
                      item.notes!,
                      style: AppTypography.bodySmall.copyWith(
                        color: AppColors.textLight,
                        fontStyle: FontStyle.italic,
                      ),
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ],

                  const SizedBox(height: AppDimensions.spaceSm),

                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      QuantitySelector(
                        quantity: item.quantity,
                        onQuantityChanged: (newQuantity) {
                          cartProvider.updateItemQuantity(index, newQuantity);
                        },
                        minQuantity: 1,
                        size: QuantitySelectorSize.small,
                      ),
                      Text(
                        '\$${item.itemTotal.toStringAsFixed(2)}',
                        style: AppTypography.price.copyWith(
                          color: AppColors.primaryOrange,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPromoSection(CartProvider cartProvider) {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMd),
      color: AppColors.bgPrimary,
      child: Row(
        children: [
          Expanded(
            child: TextField(
              controller: _promoController,
              style: AppTypography.bodyMedium,
              decoration: InputDecoration(
                hintText: 'Enter promo code',
                hintStyle: AppTypography.bodyMedium.copyWith(
                  color: AppColors.textLight,
                ),
                prefixIcon: const Icon(
                  Icons.local_offer_outlined,
                  color: AppColors.primaryOrange,
                ),
                filled: true,
                fillColor: AppColors.bgSecondary,
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(AppDimensions.radiusMd),
                  borderSide: const BorderSide(color: AppColors.borderColor),
                ),
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(AppDimensions.radiusMd),
                  borderSide: const BorderSide(color: AppColors.borderColor),
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(AppDimensions.radiusMd),
                  borderSide: const BorderSide(
                    color: AppColors.primaryOrange,
                    width: 2,
                  ),
                ),
                contentPadding: const EdgeInsets.symmetric(
                  horizontal: AppDimensions.paddingMd,
                  vertical: AppDimensions.paddingMd,
                ),
              ),
            ),
          ),
          const SizedBox(width: AppDimensions.spaceSm),
          CustomButton.primary(
            text: 'Apply',
            onPressed: () => _applyPromoCode(cartProvider),
            isLoading: _isApplyingPromo,
            size: CustomButtonSize.medium,
          ),
        ],
      ),
    );
  }

  Widget _buildOrderSummary(BuildContext context, CartProvider cartProvider) {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingLg),
      decoration: BoxDecoration(
        color: AppColors.bgPrimary,
        borderRadius: const BorderRadius.only(
          topLeft: Radius.circular(AppDimensions.radiusXl),
          topRight: Radius.circular(AppDimensions.radiusXl),
        ),
        boxShadow: [
          BoxShadow(
            color: AppColors.shadowMedium,
            blurRadius: 16,
            offset: const Offset(0, -4),
          ),
        ],
      ),
      child: SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Subtotal
            _buildSummaryRow(
              'Subtotal',
              '\$${cartProvider.subTotal.toStringAsFixed(2)}',
              isSubtitle: true,
            ),

            const SizedBox(height: AppDimensions.spaceSm),

            // Delivery Fee
            _buildSummaryRow(
              'Delivery Fee',
              cartProvider.deliveryFee > 0
                  ? '\$${cartProvider.deliveryFee.toStringAsFixed(2)}'
                  : 'FREE',
              isSubtitle: true,
              valueColor: cartProvider.deliveryFee == 0
                  ? AppColors.accentGreen
                  : null,
            ),

            if (cartProvider.discount > 0) ...[
              const SizedBox(height: AppDimensions.spaceSm),
              _buildSummaryRow(
                'Discount',
                '-\$${cartProvider.discount.toStringAsFixed(2)}',
                isSubtitle: true,
                valueColor: AppColors.accentGreen,
              ),
            ],

            const Divider(height: AppDimensions.spaceLg),

            // Total
            _buildSummaryRow(
              'Total',
              '\$${cartProvider.total.toStringAsFixed(2)}',
              isTotal: true,
            ),

            const SizedBox(height: AppDimensions.spaceLg),

            // Checkout Button
            CustomButton.primary(
              text: 'Proceed to Checkout',
              onPressed: () {
                Navigator.of(context).push(
                  MaterialPageRoute(builder: (_) => const CheckoutScreen()),
                );
              },
              isFullWidth: true,
              size: CustomButtonSize.large,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSummaryRow(
    String label,
    String value, {
    bool isSubtitle = false,
    bool isTotal = false,
    Color? valueColor,
  }) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(
          label,
          style: isTotal
              ? AppTypography.h4
              : AppTypography.bodyMedium.copyWith(
                  color: isSubtitle ? AppColors.textSecondary : null,
                ),
        ),
        Text(
          value,
          style: isTotal
              ? AppTypography.h3.copyWith(color: AppColors.primaryOrange)
              : AppTypography.bodyMedium.copyWith(
                  fontWeight: AppTypography.fontSemiBold,
                  color: valueColor,
                ),
        ),
      ],
    );
  }

  void _showClearCartDialog(BuildContext context, CartProvider cartProvider) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.radiusLg),
        ),
        title: Text(
          'Clear Cart?',
          style: AppTypography.h3,
        ),
        content: Text(
          'Are you sure you want to remove all items from your cart?',
          style: AppTypography.bodyMedium.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: Text(
              'Cancel',
              style: AppTypography.button.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ),
          CustomButton.primary(
            text: 'Clear Cart',
            onPressed: () {
              cartProvider.clearCart();
              Navigator.of(ctx).pop();
            },
          ),
        ],
      ),
    );
  }
}
