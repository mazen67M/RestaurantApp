/// Favorite item model
class FavoriteItem {
  final int id;
  final int menuItemId;
  final String nameEn;
  final String nameAr;
  final double price;
  final double? discountedPrice;
  final String? imageUrl;
  final bool isAvailable;
  final DateTime addedAt;

  FavoriteItem({
    required this.id,
    required this.menuItemId,
    required this.nameEn,
    required this.nameAr,
    required this.price,
    this.discountedPrice,
    this.imageUrl,
    required this.isAvailable,
    required this.addedAt,
  });

  factory FavoriteItem.fromJson(Map<String, dynamic> json) {
    return FavoriteItem(
      id: json['id'] ?? 0,
      menuItemId: json['menuItemId'] ?? 0,
      nameEn: json['nameEn'] ?? '',
      nameAr: json['nameAr'] ?? '',
      price: (json['price'] ?? 0).toDouble(),
      discountedPrice: json['discountedPrice'] != null
          ? (json['discountedPrice']).toDouble()
          : null,
      imageUrl: json['imageUrl'],
      isAvailable: json['isAvailable'] ?? true,
      addedAt: json['addedAt'] != null
          ? DateTime.parse(json['addedAt'])
          : DateTime.now(),
    );
  }

  /// Get the effective price (discounted or regular)
  double get effectivePrice => discountedPrice ?? price;

  /// Check if item has a discount
  bool get hasDiscount => discountedPrice != null && discountedPrice! < price;

  /// Get discount percentage
  int get discountPercentage {
    if (!hasDiscount) return 0;
    return ((price - discountedPrice!) / price * 100).round();
  }
}
