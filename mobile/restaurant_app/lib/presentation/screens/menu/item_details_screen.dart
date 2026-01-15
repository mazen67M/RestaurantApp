import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:cached_network_image/cached_network_image.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/theme/app_dimensions.dart';
import '../../../core/localization/app_localizations.dart';
import '../../../data/models/menu_model.dart';
import '../../../data/providers/restaurant_provider.dart';
import '../../../data/providers/cart_provider.dart';
import '../../../data/providers/locale_provider.dart';
import '../../widgets/common/custom_app_bar.dart';
import '../../widgets/common/rating_stars.dart';
import '../../widgets/common/quantity_selector.dart';
import '../../widgets/common/custom_button.dart';
import '../../widgets/common/loading_indicator.dart';

class ItemDetailsScreen extends StatefulWidget {
  final int itemId;

  const ItemDetailsScreen({super.key, required this.itemId});

  @override
  State<ItemDetailsScreen> createState() => _ItemDetailsScreenState();
}

class _ItemDetailsScreenState extends State<ItemDetailsScreen> {
  MenuItem? _item;
  bool _isLoading = true;
  bool _isFavorite = false;
  int _quantity = 1;
  final Set<int> _selectedAddOnIds = {};
  final TextEditingController _notesController = TextEditingController();
  final ScrollController _scrollController = ScrollController();
  bool _showTitle = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _loadItem();
    });
    _scrollController.addListener(_onScroll);
  }

  @override
  void dispose() {
    _notesController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (_scrollController.offset > 200 && !_showTitle) {
      setState(() => _showTitle = true);
    } else if (_scrollController.offset <= 200 && _showTitle) {
      setState(() => _showTitle = false);
    }
  }

  Future<void> _loadItem() async {
    final provider = context.read<RestaurantProvider>();
    final item = await provider.getItemDetails(widget.itemId);
    setState(() {
      _item = item;
      _isLoading = false;
    });
  }

  void _addToCart() {
    if (_item == null) return;

    final cartProvider = context.read<CartProvider>();
    final selectedAddOns = _item!.addOns
        .where((a) => _selectedAddOnIds.contains(a.id))
        .toList();

    cartProvider.addItem(
      _item!,
      quantity: _quantity,
      notes: _notesController.text.isNotEmpty ? _notesController.text : null,
      addOns: selectedAddOns,
    );

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(context.tr('added_to_cart')),
        backgroundColor: AppColors.accentGreen,
        duration: const Duration(seconds: 2),
        behavior: SnackBarBehavior.floating,
      ),
    );

    Navigator.of(context).pop();
  }

  double get _totalPrice {
    if (_item == null) return 0;
    
    double addOnsTotal = _item!.addOns
        .where((a) => _selectedAddOnIds.contains(a.id))
        .fold(0, (sum, a) => sum + a.price);
    
    return (_item!.effectivePrice + addOnsTotal) * _quantity;
  }

  @override
  Widget build(BuildContext context) {
    final isArabic = context.watch<LocaleProvider>().isArabic;

    if (_isLoading) {
      return const Scaffold(
        body: Center(child: LoadingIndicator()),
      );
    }

    if (_item == null) {
      return Scaffold(
        appBar: AppBar(),
        body: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(
                Icons.error_outline,
                size: 64,
                color: AppColors.textLight,
              ),
              const SizedBox(height: AppDimensions.spaceMd),
              Text(
                context.tr('error'),
                style: AppTypography.h3,
              ),
            ],
          ),
        ),
      );
    }

    return Scaffold(
      backgroundColor: AppColors.bgPrimary,
      body: Stack(
        children: [
          // Main Content
          CustomScrollView(
            controller: _scrollController,
            slivers: [
              // Large Image Header
              SliverAppBar(
                expandedHeight: 300,
                pinned: true,
                backgroundColor: _showTitle ? AppColors.bgPrimary : Colors.transparent,
                elevation: _showTitle ? 2 : 0,
                leading: IconButton(
                  icon: Container(
                    padding: const EdgeInsets.all(AppDimensions.paddingSm),
                    decoration: BoxDecoration(
                      color: _showTitle ? Colors.transparent : AppColors.bgPrimary,
                      shape: BoxShape.circle,
                    ),
                    child: const Icon(
                      Icons.arrow_back_ios,
                      color: AppColors.textPrimary,
                      size: 20,
                    ),
                  ),
                  onPressed: () => Navigator.of(context).pop(),
                ),
                actions: [
                  IconButton(
                    icon: Container(
                      padding: const EdgeInsets.all(AppDimensions.paddingSm),
                      decoration: BoxDecoration(
                        color: _showTitle ? Colors.transparent : AppColors.bgPrimary,
                        shape: BoxShape.circle,
                      ),
                      child: Icon(
                        _isFavorite ? Icons.favorite : Icons.favorite_border,
                        color: _isFavorite ? AppColors.accentRed : AppColors.textPrimary,
                        size: 24,
                      ),
                    ),
                    onPressed: () {
                      setState(() => _isFavorite = !_isFavorite);
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(
                          content: Text(_isFavorite 
                            ? 'Added to favorites' 
                            : 'Removed from favorites'),
                          duration: const Duration(seconds: 1),
                        ),
                      );
                    },
                  ),
                ],
                title: _showTitle
                    ? Text(
                        _item!.getName(isArabic),
                        style: AppTypography.h4,
                      )
                    : null,
                flexibleSpace: FlexibleSpaceBar(
                  background: Hero(
                    tag: 'item_${widget.itemId}',
                    child: _item!.imageUrl != null
                        ? CachedNetworkImage(
                            imageUrl: _item!.imageUrl!,
                            fit: BoxFit.cover,
                            placeholder: (_, __) => Container(
                              color: AppColors.bgSecondary,
                              child: const Center(
                                child: LoadingIndicator(),
                              ),
                            ),
                            errorWidget: (_, __, ___) => Container(
                              color: AppColors.bgSecondary,
                              child: const Icon(
                                Icons.fastfood,
                                size: 80,
                                color: AppColors.textLight,
                              ),
                            ),
                          )
                        : Container(
                            color: AppColors.bgSecondary,
                            child: const Icon(
                              Icons.fastfood,
                              size: 80,
                              color: AppColors.textLight,
                            ),
                          ),
                  ),
                ),
              ),

              // Product Info
              SliverToBoxAdapter(
                child: Container(
                  decoration: const BoxDecoration(
                    color: AppColors.bgPrimary,
                    borderRadius: BorderRadius.only(
                      topLeft: Radius.circular(AppDimensions.radiusXl),
                      topRight: Radius.circular(AppDimensions.radiusXl),
                    ),
                  ),
                  child: Padding(
                    padding: const EdgeInsets.all(AppDimensions.paddingLg),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        // Name and Popular Badge
                        Row(
                          children: [
                            Expanded(
                              child: Text(
                                _item!.getName(isArabic),
                                style: AppTypography.h2,
                              ),
                            ),
                            if (_item!.isPopular)
                              Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: AppDimensions.paddingSm,
                                  vertical: AppDimensions.paddingXs,
                                ),
                                decoration: BoxDecoration(
                                  color: AppColors.accentYellow.withOpacity(0.2),
                                  borderRadius: BorderRadius.circular(AppDimensions.radiusSm),
                                ),
                                child: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    const Icon(
                                      Icons.star,
                                      size: 16,
                                      color: AppColors.accentYellow,
                                    ),
                                    const SizedBox(width: 4),
                                    Text(
                                      'Popular',
                                      style: AppTypography.caption.copyWith(
                                        fontWeight: AppTypography.fontBold,
                                        color: AppColors.accentYellow,
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                          ],
                        ),

                        const SizedBox(height: AppDimensions.spaceSm),

                        // Rating
                        const RatingStars(
                          rating: 4.5,
                          reviewCount: 120,
                          size: 18,
                        ),

                        const SizedBox(height: AppDimensions.spaceMd),

                        // Price
                        Row(
                          children: [
                            if (_item!.hasDiscount) ...[
                              Text(
                                '\$${_item!.price.toStringAsFixed(2)}',
                                style: AppTypography.bodyLarge.copyWith(
                                  decoration: TextDecoration.lineThrough,
                                  color: AppColors.textLight,
                                ),
                              ),
                              const SizedBox(width: AppDimensions.spaceSm),
                            ],
                            Text(
                              '\$${_item!.effectivePrice.toStringAsFixed(2)}',
                              style: AppTypography.h2.copyWith(
                                color: _item!.hasDiscount 
                                    ? AppColors.accentRed 
                                    : AppColors.primaryOrange,
                              ),
                            ),
                          ],
                        ),

                        const SizedBox(height: AppDimensions.spaceLg),

                        // Description
                        if (_item!.getDescription(isArabic) != null) ...[
                          Text(
                            'Description',
                            style: AppTypography.h4,
                          ),
                          const SizedBox(height: AppDimensions.spaceSm),
                          Text(
                            _item!.getDescription(isArabic)!,
                            style: AppTypography.bodyMedium.copyWith(
                              color: AppColors.textSecondary,
                              height: 1.6,
                            ),
                          ),
                          const SizedBox(height: AppDimensions.spaceLg),
                        ],

                        // Preparation time and calories
                        Row(
                          children: [
                            _buildInfoChip(
                              Icons.timer_outlined,
                              '${_item!.preparationTimeMinutes} min',
                            ),
                            if (_item!.calories != null) ...[
                              const SizedBox(width: AppDimensions.spaceMd),
                              _buildInfoChip(
                                Icons.local_fire_department_outlined,
                                '${_item!.calories} cal',
                              ),
                            ],
                          ],
                        ),

                        // Add-ons
                        if (_item!.addOns.isNotEmpty) ...[
                          const SizedBox(height: AppDimensions.spaceLg),
                          Text(
                            'Add-ons',
                            style: AppTypography.h4,
                          ),
                          const SizedBox(height: AppDimensions.spaceMd),
                          ...(_item!.addOns.where((a) => a.isAvailable).map((addOn) {
                            final isSelected = _selectedAddOnIds.contains(addOn.id);
                            return Container(
                              margin: const EdgeInsets.only(bottom: AppDimensions.spaceSm),
                              decoration: BoxDecoration(
                                color: isSelected 
                                    ? AppColors.primaryOrange.withOpacity(0.1)
                                    : AppColors.bgSecondary,
                                borderRadius: BorderRadius.circular(AppDimensions.radiusMd),
                                border: Border.all(
                                  color: isSelected 
                                      ? AppColors.primaryOrange 
                                      : AppColors.borderColor,
                                  width: 1.5,
                                ),
                              ),
                              child: CheckboxListTile(
                                value: isSelected,
                                onChanged: (value) {
                                  setState(() {
                                    if (value == true) {
                                      _selectedAddOnIds.add(addOn.id);
                                    } else {
                                      _selectedAddOnIds.remove(addOn.id);
                                    }
                                  });
                                },
                                title: Text(
                                  addOn.getName(isArabic),
                                  style: AppTypography.bodyMedium.copyWith(
                                    fontWeight: AppTypography.fontMedium,
                                  ),
                                ),
                                subtitle: Text(
                                  '+\$${addOn.price.toStringAsFixed(2)}',
                                  style: AppTypography.bodySmall.copyWith(
                                    color: AppColors.primaryOrange,
                                    fontWeight: AppTypography.fontSemiBold,
                                  ),
                                ),
                                activeColor: AppColors.primaryOrange,
                                controlAffinity: ListTileControlAffinity.leading,
                                contentPadding: const EdgeInsets.symmetric(
                                  horizontal: AppDimensions.paddingMd,
                                  vertical: AppDimensions.paddingXs,
                                ),
                              ),
                            );
                          })),
                        ],

                        // Special Instructions
                        const SizedBox(height: AppDimensions.spaceLg),
                        Text(
                          'Special Instructions',
                          style: AppTypography.h4,
                        ),
                        const SizedBox(height: AppDimensions.spaceMd),
                        TextField(
                          controller: _notesController,
                          maxLines: 3,
                          style: AppTypography.bodyMedium,
                          decoration: InputDecoration(
                            hintText: 'Add any special requests...',
                            hintStyle: AppTypography.bodyMedium.copyWith(
                              color: AppColors.textLight,
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
                          ),
                        ),

                        const SizedBox(height: 120),
                      ],
                    ),
                  ),
                ),
              ),
            ],
          ),

          // Bottom Sheet - Quantity and Add to Cart
          Positioned(
            left: 0,
            right: 0,
            bottom: 0,
            child: Container(
              padding: const EdgeInsets.all(AppDimensions.paddingLg),
              decoration: BoxDecoration(
                color: AppColors.bgPrimary,
                boxShadow: [
                  BoxShadow(
                    color: AppColors.shadowMedium,
                    blurRadius: 16,
                    offset: const Offset(0, -4),
                  ),
                ],
              ),
              child: SafeArea(
                child: Row(
                  children: [
                    // Quantity Selector
                    QuantitySelector(
                      quantity: _quantity,
                      onQuantityChanged: (newQuantity) {
                        setState(() => _quantity = newQuantity);
                      },
                      minQuantity: 1,
                      maxQuantity: 99,
                      size: QuantitySelectorSize.large,
                    ),

                    const SizedBox(width: AppDimensions.spaceMd),

                    // Add to Cart Button
                    Expanded(
                      child: CustomButton.primary(
                        text: 'Add to Cart - \$${_totalPrice.toStringAsFixed(2)}',
                        onPressed: _addToCart,
                        size: CustomButtonSize.large,
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildInfoChip(IconData icon, String label) {
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingMd,
        vertical: AppDimensions.paddingSm,
      ),
      decoration: BoxDecoration(
        color: AppColors.bgSecondary,
        borderRadius: BorderRadius.circular(AppDimensions.radiusFull),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            icon,
            size: AppDimensions.iconSm,
            color: AppColors.textSecondary,
          ),
          const SizedBox(width: AppDimensions.spaceXs),
          Text(
            label,
            style: AppTypography.bodySmall.copyWith(
              color: AppColors.textSecondary,
              fontWeight: AppTypography.fontMedium,
            ),
          ),
        ],
      ),
    );
  }
}
