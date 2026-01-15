import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/theme/app_dimensions.dart';
import '../../../data/providers/restaurant_provider.dart';
import '../../../data/providers/locale_provider.dart';
import '../../../data/models/menu_model.dart';
import '../../widgets/common/custom_app_bar.dart';
import '../../widgets/category/category_chip.dart';
import '../../widgets/food/food_card.dart';
import '../../widgets/common/loading_indicator.dart';
import '../../widgets/common/empty_state.dart';
import '../menu/item_details_screen.dart';

class SearchScreen extends StatefulWidget {
  const SearchScreen({super.key});

  @override
  State<SearchScreen> createState() => _SearchScreenState();
}

class _SearchScreenState extends State<SearchScreen> {
  final TextEditingController _searchController = TextEditingController();
  int _selectedCategoryIndex = 0;
  String _selectedSortOption = 'popular';
  bool _isSearching = false;
  bool _showClearButton = false;

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _performSearch(String query, RestaurantProvider provider) {
    if (query.length < 2) {
      provider.clearSearch();
      if (mounted) {
        setState(() => _isSearching = false);
      }
      return;
    }

    if (mounted) {
      setState(() => _isSearching = true);
    }
    
    provider.searchItems(query).then((_) {
      if (mounted) {
        setState(() => _isSearching = false);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final restaurantProvider = context.watch<RestaurantProvider>();
    final isArabic = context.watch<LocaleProvider>().isArabic;

    return Scaffold(
      backgroundColor: AppColors.bgSecondary,
      appBar: const CustomAppBar(
        title: 'Search',
        showBackButton: false,
      ),
      body: Column(
        children: [
          // Search Bar
          _buildSearchBar(restaurantProvider),

          // Filters
          _buildFilters(restaurantProvider, isArabic),

          // Results
          Expanded(
            child: _buildSearchResults(restaurantProvider, isArabic),
          ),
        ],
      ),
    );
  }

  Widget _buildSearchBar(RestaurantProvider provider) {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMd),
      color: AppColors.bgPrimary,
      child: TextField(
        controller: _searchController,
        autofocus: true,
        style: AppTypography.bodyMedium,
        decoration: InputDecoration(
          hintText: 'Search for food...',
          hintStyle: AppTypography.bodyMedium.copyWith(
            color: AppColors.textLight,
          ),
          prefixIcon: const Icon(
            Icons.search,
            color: AppColors.primaryOrange,
          ),
          suffixIcon: _showClearButton
              ? IconButton(
                  icon: const Icon(
                    Icons.clear,
                    color: AppColors.textSecondary,
                  ),
                  onPressed: () {
                    _searchController.clear();
                    provider.clearSearch();
                    if (mounted) {
                      setState(() => _showClearButton = false);
                    }
                  },
                )
              : null,
          filled: true,
          fillColor: AppColors.bgSecondary,
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(AppDimensions.radiusFull),
            borderSide: const BorderSide(color: AppColors.borderColor),
          ),
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(AppDimensions.radiusFull),
            borderSide: const BorderSide(color: AppColors.borderColor),
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(AppDimensions.radiusFull),
            borderSide: const BorderSide(
              color: AppColors.primaryOrange,
              width: 2,
            ),
          ),
          contentPadding: const EdgeInsets.symmetric(
            horizontal: AppDimensions.paddingLg,
            vertical: AppDimensions.paddingMd,
          ),
        ),
        onChanged: (value) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            if (mounted) {
              setState(() {
                _showClearButton = value.isNotEmpty;
              });
              _performSearch(value, provider);
            }
          });
        },
      ),
    );
  }

  Widget _buildFilters(RestaurantProvider provider, bool isArabic) {
    final categories = [
      CategoryChipData(label: 'All', icon: Icons.restaurant),
      ...provider.categories.map((cat) => CategoryChipData(
            label: cat.getName(isArabic),
            value: cat.id.toString(),
          )),
    ];

    return Container(
      color: AppColors.bgPrimary,
      child: Column(
        children: [
          // Category Filter
          if (categories.isNotEmpty)
            CategoryChipList(
              categories: categories,
              selectedIndex: _selectedCategoryIndex,
              onCategorySelected: (index) {
                setState(() => _selectedCategoryIndex = index);
              },
            ),

          const SizedBox(height: AppDimensions.spaceSm),

          // Sort Options
          Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.screenPaddingHorizontal,
            ),
            child: Row(
              children: [
                Text(
                  'Sort by:',
                  style: AppTypography.bodySmall.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
                const SizedBox(width: AppDimensions.spaceSm),
                _buildSortChip('Popular', 'popular'),
                const SizedBox(width: AppDimensions.spaceXs),
                _buildSortChip('Price: Low to High', 'price_asc'),
                const SizedBox(width: AppDimensions.spaceXs),
                _buildSortChip('Rating', 'rating'),
              ],
            ),
          ),

          const SizedBox(height: AppDimensions.spaceSm),
        ],
      ),
    );
  }

  Widget _buildSortChip(String label, String value) {
    final isSelected = _selectedSortOption == value;
    return GestureDetector(
      onTap: () => setState(() => _selectedSortOption = value),
      child: Container(
        padding: const EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingSm,
          vertical: AppDimensions.paddingXs,
        ),
        decoration: BoxDecoration(
          color: isSelected ? AppColors.primaryOrange : AppColors.bgSecondary,
          borderRadius: BorderRadius.circular(AppDimensions.radiusFull),
          border: Border.all(
            color: isSelected ? AppColors.primaryOrange : AppColors.borderColor,
          ),
        ),
        child: Text(
          label,
          style: AppTypography.caption.copyWith(
            color: isSelected ? AppColors.textWhite : AppColors.textSecondary,
            fontWeight: isSelected
                ? AppTypography.fontSemiBold
                : AppTypography.fontRegular,
          ),
        ),
      ),
    );
  }

  Widget _buildSearchResults(RestaurantProvider provider, bool isArabic) {
    if (_searchController.text.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.search,
              size: 80,
              color: AppColors.textLight.withOpacity(0.5),
            ),
            const SizedBox(height: AppDimensions.spaceMd),
            Text(
              'Search for your favorite food',
              style: AppTypography.h4.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ],
        ),
      );
    }

    if (_isSearching) {
      return const Center(child: LoadingIndicator());
    }

    if (provider.searchResults.isEmpty) {
      return const EmptyState.search();
    }

    List<MenuItem> results = provider.searchResults;

    // Apply category filter
    if (_selectedCategoryIndex > 0) {
      final categoryId = provider.categories[_selectedCategoryIndex - 1].id;
      results = results.where((item) => item.categoryId == categoryId).toList();
    }

    // Apply sorting
    switch (_selectedSortOption) {
      case 'price_asc':
        results.sort((a, b) => a.effectivePrice.compareTo(b.effectivePrice));
        break;
      case 'rating':
        // TODO: Sort by rating when available
        break;
      case 'popular':
      default:
        results = results.where((item) => item.isPopular).toList() +
            results.where((item) => !item.isPopular).toList();
    }

    return ListView.builder(
      padding: const EdgeInsets.all(AppDimensions.paddingMd),
      itemCount: results.length,
      itemBuilder: (context, index) {
        final item = results[index];
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
    );
  }
}
