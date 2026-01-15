import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/theme/app_dimensions.dart';
import '../../../core/localization/app_localizations.dart';
import '../../../data/providers/restaurant_provider.dart';
import '../../../data/providers/locale_provider.dart';
import '../../../data/models/menu_model.dart';
import '../../widgets/common/custom_app_bar.dart';
import '../../widgets/food/food_card.dart';
import '../../widgets/common/loading_indicator.dart';
import '../../widgets/common/empty_state.dart';
import 'item_details_screen.dart';

class CategoryItemsScreen extends StatefulWidget {
  final int categoryId;

  const CategoryItemsScreen({super.key, required this.categoryId});

  @override
  State<CategoryItemsScreen> createState() => _CategoryItemsScreenState();
}

class _CategoryItemsScreenState extends State<CategoryItemsScreen> {
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _loadItems();
    });
  }

  Future<void> _loadItems() async {
    setState(() => _isLoading = true);
    final provider = context.read<RestaurantProvider>();
    await provider.loadItemsByCategory(widget.categoryId);
    setState(() => _isLoading = false);
  }

  @override
  Widget build(BuildContext context) {
    final localeProvider = context.watch<LocaleProvider>();
    final isArabic = localeProvider.isArabic;
    final restaurantProvider = context.watch<RestaurantProvider>();
    
    final category = restaurantProvider.categories
        .where((c) => c.id == widget.categoryId)
        .firstOrNull;
    
    final items = restaurantProvider.getItemsByCategory(widget.categoryId);

    return Scaffold(
      backgroundColor: AppColors.bgSecondary,
      appBar: CustomAppBar(
        title: category?.getName(isArabic) ?? 'Menu',
      ),
      body: _isLoading
          ? const Center(child: LoadingIndicator())
          : items.isEmpty
              ? const EmptyState(
                  icon: Icons.restaurant_menu,
                  title: 'No items found',
                  message: 'This category has no items yet',
                )
              : RefreshIndicator(
                  onRefresh: _loadItems,
                  color: AppColors.primaryOrange,
                  child: ListView.builder(
                    padding: const EdgeInsets.all(AppDimensions.paddingMd),
                    itemCount: items.length,
                    itemBuilder: (context, index) {
                      final item = items[index];
                      return FoodCard(
                        imageUrl: item.imageUrl ?? 'https://via.placeholder.com/300x200',
                        name: item.getName(isArabic),
                        description: item.getDescription(isArabic),
                        price: item.effectivePrice,
                        rating: 4.5,
                        reviewCount: 120,
                        layout: FoodCardLayout.horizontal,
                        badge: item.hasDiscount
                            ? '${item.discountPercentage.toInt()}% OFF'
                            : null,
                        onTap: () {
                          Navigator.of(context).push(
                            MaterialPageRoute(
                              builder: (_) => ItemDetailsScreen(itemId: item.id),
                            ),
                          );
                        },
                        onFavoriteToggle: () {
                          ScaffoldMessenger.of(context).showSnackBar(
                            const SnackBar(
                              content: Text('Favorites feature coming soon'),
                              duration: Duration(seconds: 1),
                            ),
                          );
                        },
                        onAddToCart: () {
                          ScaffoldMessenger.of(context).showSnackBar(
                            SnackBar(
                              content: Text('${item.getName(isArabic)} added to cart'),
                              backgroundColor: AppColors.accentGreen,
                              duration: const Duration(seconds: 2),
                            ),
                          );
                        },
                      );
                    },
                  ),
                ),
    );
  }
}
