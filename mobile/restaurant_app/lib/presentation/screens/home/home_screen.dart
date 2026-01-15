import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/theme/app_dimensions.dart';
import '../../../core/localization/app_localizations.dart';
import '../../../data/providers/auth_provider.dart';
import '../../../data/providers/restaurant_provider.dart';
import '../../../data/providers/cart_provider.dart';
import '../../../data/providers/locale_provider.dart';
import '../../../data/models/menu_model.dart';
import '../../widgets/common/custom_bottom_nav.dart';
import '../../widgets/common/custom_app_bar.dart';
import '../../widgets/category/category_chip.dart';
import '../../widgets/food/food_card.dart';
import '../../widgets/common/loading_indicator.dart';
import '../../widgets/common/empty_state.dart';
import '../menu/category_items_screen.dart';
import '../menu/item_details_screen.dart';
import '../cart/cart_screen.dart';
import '../orders/orders_screen.dart';
import '../favorites/favorites_screen.dart';
import '../profile/profile_screen.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> with SingleTickerProviderStateMixin {
  int _currentNavIndex = 0;
  int _selectedCategoryIndex = 0;
  int _selectedTabIndex = 0; // 0: Recommended, 1: Popular
  late TabController _tabController;

  final List<Widget> _screens = [];

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _loadData();
    });
  }

  Future<void> _loadData() async {
    final restaurantProvider = context.read<RestaurantProvider>();
    await restaurantProvider.loadAll();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: IndexedStack(
        index: _currentNavIndex,
        children: [
          _buildHomeTab(),
          const Center(child: Text('Search Screen - Coming Soon')),
          const OrdersScreen(),
          const FavoritesScreen(),
          const ProfileScreen(),
        ],
      ),
      bottomNavigationBar: Consumer<CartProvider>(
        builder: (context, cart, _) {
          return CustomBottomNav(
            currentIndex: _currentNavIndex,
            onTap: (index) => setState(() => _currentNavIndex = index),
            cartItemCount: cart.itemCount,
          );
        },
      ),
    );
  }

  Widget _buildHomeTab() {
    final localeProvider = context.watch<LocaleProvider>();
    final isArabic = localeProvider.isArabic;
    final restaurantProvider = context.watch<RestaurantProvider>();
    final authProvider = context.watch<AuthProvider>();

    return RefreshIndicator(
      onRefresh: _loadData,
      color: AppColors.primaryOrange,
      child: CustomScrollView(
        slivers: [
          // Header Section
          SliverToBoxAdapter(
            child: _buildHeader(authProvider, isArabic),
          ),

          // Search Bar
          SliverToBoxAdapter(
            child: _buildSearchBar(),
          ),

          // Banner/Promo Section
          SliverToBoxAdapter(
            child: _buildPromoBanner(),
          ),

          // Categories Section
          SliverToBoxAdapter(
            child: _buildCategoriesSection(restaurantProvider, isArabic),
          ),

          // Recommended / Popular Tabs
          SliverToBoxAdapter(
            child: _buildTabSection(),
          ),

          // Food Items Grid
          if (restaurantProvider.isLoading)
            const SliverToBoxAdapter(
              child: Padding(
                padding: EdgeInsets.all(AppDimensions.paddingXl),
                child: LoadingIndicator(),
              ),
            )
          else
            _buildFoodGrid(restaurantProvider, isArabic),

          // Bottom Spacing
          const SliverToBoxAdapter(
            child: SizedBox(height: AppDimensions.space2xl),
          ),
        ],
      ),
    );
  }

  Widget _buildHeader(AuthProvider authProvider, bool isArabic) {
    return Container(
      padding: const EdgeInsets.fromLTRB(
        AppDimensions.screenPaddingHorizontal,
        AppDimensions.paddingXl + 32,
        AppDimensions.screenPaddingHorizontal,
        AppDimensions.paddingMd,
      ),
      decoration: BoxDecoration(
        color: AppColors.bgPrimary,
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          // Location & Welcome
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    const Icon(
                      Icons.location_on,
                      color: AppColors.primaryOrange,
                      size: AppDimensions.iconSm,
                    ),
                    const SizedBox(width: AppDimensions.spaceXs),
                    Text(
                      'Deliver to',
                      style: AppTypography.caption.copyWith(
                        color: AppColors.textSecondary,
                      ),
                    ),
                    const SizedBox(width: AppDimensions.spaceXs),
                    const Icon(
                      Icons.keyboard_arrow_down,
                      color: AppColors.textSecondary,
                      size: AppDimensions.iconSm,
                    ),
                  ],
                ),
                const SizedBox(height: AppDimensions.spaceXs),
                Text(
                  authProvider.user?.fullName ?? 'Guest User',
                  style: AppTypography.h4.copyWith(
                    color: AppColors.textPrimary,
                  ),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
              ],
            ),
          ),

          // Notification Icon
          Consumer<CartProvider>(
            builder: (context, cart, _) {
              return NotificationIconButton(
                notificationCount: 0,
                onPressed: () {
                  // TODO: Navigate to notifications
                },
              );
            },
          ),
        ],
      ),
    );
  }

  Widget _buildSearchBar() {
    return Padding(
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.screenPaddingHorizontal,
        vertical: AppDimensions.paddingSm,
      ),
      child: GestureDetector(
        onTap: () {
          // TODO: Navigate to search screen
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Search screen coming soon')),
          );
        },
        child: Container(
          height: 50,
          padding: const EdgeInsets.symmetric(
            horizontal: AppDimensions.paddingMd,
          ),
          decoration: BoxDecoration(
            color: AppColors.bgSecondary,
            borderRadius: BorderRadius.circular(AppDimensions.radiusMd),
            border: Border.all(color: AppColors.borderColor),
          ),
          child: Row(
            children: [
              const Icon(
                Icons.search,
                color: AppColors.textSecondary,
                size: AppDimensions.iconMd,
              ),
              const SizedBox(width: AppDimensions.spaceMd),
              Text(
                'Search for food...',
                style: AppTypography.bodyMedium.copyWith(
                  color: AppColors.textLight,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildPromoBanner() {
    return Container(
      margin: const EdgeInsets.symmetric(
        horizontal: AppDimensions.screenPaddingHorizontal,
        vertical: AppDimensions.paddingMd,
      ),
      height: 160,
      decoration: BoxDecoration(
        gradient: AppColors.primaryGradient,
        borderRadius: BorderRadius.circular(AppDimensions.radiusLg),
        boxShadow: [
          BoxShadow(
            color: AppColors.primaryOrange.withOpacity(0.3),
            blurRadius: 12,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Stack(
        children: [
          Positioned.fill(
            child: ClipRRect(
              borderRadius: BorderRadius.circular(AppDimensions.radiusLg),
              child: Container(
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    colors: [
                      AppColors.primaryOrange.withOpacity(0.8),
                      AppColors.primaryOrangeLight.withOpacity(0.6),
                    ],
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                  ),
                ),
              ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(AppDimensions.paddingLg),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppDimensions.paddingSm,
                    vertical: AppDimensions.paddingXs,
                  ),
                  decoration: BoxDecoration(
                    color: AppColors.accentYellow,
                    borderRadius: BorderRadius.circular(AppDimensions.radiusSm),
                  ),
                  child: Text(
                    '30% OFF',
                    style: AppTypography.caption.copyWith(
                      color: AppColors.textPrimary,
                      fontWeight: AppTypography.fontBold,
                    ),
                  ),
                ),
                const SizedBox(height: AppDimensions.spaceSm),
                Text(
                  'Special Deal',
                  style: AppTypography.h2.copyWith(
                    color: AppColors.textWhite,
                  ),
                ),
                const SizedBox(height: AppDimensions.spaceXs),
                Text(
                  'On your first order',
                  style: AppTypography.bodyMedium.copyWith(
                    color: AppColors.textWhite.withOpacity(0.9),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCategoriesSection(RestaurantProvider provider, bool isArabic) {
    if (provider.categories.isEmpty) {
      return const SizedBox.shrink();
    }

    final categories = [
      CategoryChipData(label: 'All', icon: Icons.restaurant),
      ...provider.categories.map((cat) => CategoryChipData(
            label: cat.getName(isArabic),
            value: cat.id.toString(),
          )),
    ];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(
            AppDimensions.screenPaddingHorizontal,
            AppDimensions.paddingLg,
            AppDimensions.screenPaddingHorizontal,
            AppDimensions.paddingSm,
          ),
          child: Text(
            'Categories',
            style: AppTypography.h3,
          ),
        ),
        CategoryChipList(
          categories: categories,
          selectedIndex: _selectedCategoryIndex,
          onCategorySelected: (index) {
            setState(() => _selectedCategoryIndex = index);
          },
        ),
      ],
    );
  }

  Widget _buildTabSection() {
    return Padding(
      padding: const EdgeInsets.fromLTRB(
        AppDimensions.screenPaddingHorizontal,
        AppDimensions.paddingLg,
        AppDimensions.screenPaddingHorizontal,
        AppDimensions.paddingMd,
      ),
      child: Row(
        children: [
          _buildTab('Recommended', 0),
          const SizedBox(width: AppDimensions.spaceMd),
          _buildTab('Popular', 1),
        ],
      ),
    );
  }

  Widget _buildTab(String label, int index) {
    final isSelected = _selectedTabIndex == index;
    return GestureDetector(
      onTap: () => setState(() => _selectedTabIndex = index),
      child: Column(
        children: [
          Text(
            label,
            style: AppTypography.h4.copyWith(
              color: isSelected ? AppColors.primaryOrange : AppColors.textSecondary,
              fontWeight: isSelected ? AppTypography.fontBold : AppTypography.fontMedium,
            ),
          ),
          const SizedBox(height: AppDimensions.spaceXs),
          if (isSelected)
            Container(
              width: 40,
              height: 3,
              decoration: BoxDecoration(
                color: AppColors.primaryOrange,
                borderRadius: BorderRadius.circular(AppDimensions.radiusFull),
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildFoodGrid(RestaurantProvider provider, bool isArabic) {
    List<MenuItem> items;
    
    if (_selectedTabIndex == 0) {
      // Recommended - show popular items or filtered by category
      if (_selectedCategoryIndex == 0) {
        items = provider.popularItems;
      } else {
        final categoryId = provider.categories[_selectedCategoryIndex - 1].id;
        items = provider.popularItems.where((item) => item.categoryId == categoryId).toList();
      }
    } else {
      // Popular items
      items = provider.popularItems;
    }

    if (items.isEmpty) {
      return const SliverToBoxAdapter(
        child: EmptyState(
          icon: Icons.restaurant_menu,
          title: 'No items found',
          message: 'Check back later for delicious food!',
        ),
      );
    }

    return SliverPadding(
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.screenPaddingHorizontal,
      ),
      sliver: SliverList(
        delegate: SliverChildBuilderDelegate(
          (context, index) {
            final item = items[index];
            return FoodCard(
              imageUrl: item.imageUrl ?? 'https://via.placeholder.com/300x200',
              name: item.getName(isArabic),
              description: item.getDescription(isArabic),
              price: item.effectivePrice,
              rating: 4.5, // TODO: Add rating to model
              reviewCount: 120, // TODO: Add review count to model
              layout: FoodCardLayout.horizontal,
              badge: item.hasDiscount ? '${item.discountPercentage.toInt()}% OFF' : null,
              onTap: () => _showItemDetails(item.id),
              onAddToCart: () => _addToCart(item),
              onFavoriteToggle: () {
                // TODO: Implement favorites
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(content: Text('Favorites feature coming soon')),
                );
              },
            );
          },
          childCount: items.length,
        ),
      ),
    );
  }

  void _showItemDetails(int itemId) {
    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (_) => ItemDetailsScreen(itemId: itemId),
      ),
    );
  }

  void _addToCart(MenuItem item) {
    final cartProvider = context.read<CartProvider>();
    // TODO: Implement proper cart item addition with add-ons selection
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('${item.getName(false)} added to cart'),
        backgroundColor: AppColors.accentGreen,
        duration: const Duration(seconds: 2),
        action: SnackBarAction(
          label: 'VIEW CART',
          textColor: AppColors.textWhite,
          onPressed: () {
            Navigator.of(context).push(
              MaterialPageRoute(builder: (_) => const CartScreen()),
            );
          },
        ),
      ),
    );
  }
}
