import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/theme/app_dimensions.dart';
import '../../../data/providers/locale_provider.dart';
import '../../../data/models/menu_model.dart';
import '../../widgets/common/custom_app_bar.dart';
import '../../widgets/food/food_card.dart';
import '../../widgets/common/empty_state.dart';
import '../menu/item_details_screen.dart';

class FavoritesScreen extends StatelessWidget {
  const FavoritesScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final isArabic = context.watch<LocaleProvider>().isArabic;
    
    // TODO: Replace with actual favorites provider
    final List<MenuItem> favorites = [];

    return Scaffold(
      backgroundColor: AppColors.bgSecondary,
      appBar: const CustomAppBar(
        title: 'Favorites',
        showBackButton: false,
      ),
      body: favorites.isEmpty
          ? const EmptyState.favorites(
              actionText: 'Explore Menu',
            )
          : _buildFavoritesGrid(context, favorites, isArabic),
    );
  }

  Widget _buildFavoritesGrid(
    BuildContext context,
    List<MenuItem> favorites,
    bool isArabic,
  ) {
    return GridView.builder(
      padding: const EdgeInsets.all(AppDimensions.paddingMd),
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 2,
        childAspectRatio: 0.75,
        crossAxisSpacing: AppDimensions.spaceSm,
        mainAxisSpacing: AppDimensions.spaceSm,
      ),
      itemCount: favorites.length,
      itemBuilder: (context, index) {
        final item = favorites[index];
        return FoodCard(
          imageUrl: item.imageUrl ?? 'https://via.placeholder.com/300x200',
          name: item.getName(isArabic),
          description: item.getDescription(isArabic),
          price: item.effectivePrice,
          rating: 4.5,
          reviewCount: 120,
          layout: FoodCardLayout.grid,
          isFavorite: true,
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
            // TODO: Remove from favorites
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                content: Text('Removed from favorites'),
                duration: Duration(seconds: 1),
              ),
            );
          },
          onAddToCart: () {
            // TODO: Add to cart
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
    );
  }
}
