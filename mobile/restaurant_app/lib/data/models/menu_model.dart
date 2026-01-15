class MenuCategory {
  final int id;
  final String nameAr;
  final String nameEn;
  final String? descriptionAr;
  final String? descriptionEn;
  final String? imageUrl;
  final int displayOrder;
  final bool isActive;
  final int itemCount;

  MenuCategory({
    required this.id,
    required this.nameAr,
    required this.nameEn,
    this.descriptionAr,
    this.descriptionEn,
    this.imageUrl,
    required this.displayOrder,
    required this.isActive,
    required this.itemCount,
  });

  factory MenuCategory.fromJson(Map<String, dynamic> json) {
    return MenuCategory(
      id: json['id'],
      nameAr: json['nameAr'],
      nameEn: json['nameEn'],
      descriptionAr: json['descriptionAr'],
      descriptionEn: json['descriptionEn'],
      imageUrl: json['imageUrl'],
      displayOrder: json['displayOrder'],
      isActive: json['isActive'],
      itemCount: json['itemCount'],
    );
  }

  String getName(bool isArabic) => isArabic ? nameAr : nameEn;
  String? getDescription(bool isArabic) => isArabic ? descriptionAr : descriptionEn;
}

class MenuItem {
  final int id;
  final int categoryId;
  final String nameAr;
  final String nameEn;
  final String? descriptionAr;
  final String? descriptionEn;
  final double price;
  final double? discountedPrice;
  final String? imageUrl;
  final int preparationTimeMinutes;
  final bool isAvailable;
  final bool isPopular;
  final int? calories;
  final List<MenuItemAddOn> addOns;

  MenuItem({
    required this.id,
    required this.categoryId,
    required this.nameAr,
    required this.nameEn,
    this.descriptionAr,
    this.descriptionEn,
    required this.price,
    this.discountedPrice,
    this.imageUrl,
    required this.preparationTimeMinutes,
    required this.isAvailable,
    required this.isPopular,
    this.calories,
    this.addOns = const [],
  });

  factory MenuItem.fromJson(Map<String, dynamic> json) {
    return MenuItem(
      id: json['id'],
      categoryId: json['categoryId'],
      nameAr: json['nameAr'],
      nameEn: json['nameEn'],
      descriptionAr: json['descriptionAr'],
      descriptionEn: json['descriptionEn'],
      price: (json['price'] as num).toDouble(),
      discountedPrice: json['discountedPrice']?.toDouble(),
      imageUrl: json['imageUrl'],
      preparationTimeMinutes: json['preparationTimeMinutes'] ?? 15,
      isAvailable: json['isAvailable'] ?? true,
      isPopular: json['isPopular'] ?? false,
      calories: json['calories'],
      addOns: json['addOns'] != null
          ? (json['addOns'] as List)
              .map((a) => MenuItemAddOn.fromJson(a))
              .toList()
          : [],
    );
  }

  String getName(bool isArabic) => isArabic ? nameAr : nameEn;
  String? getDescription(bool isArabic) => isArabic ? descriptionAr : descriptionEn;
  
  double get effectivePrice => discountedPrice ?? price;
  bool get hasDiscount => discountedPrice != null && discountedPrice! < price;
  double get discountPercentage => hasDiscount 
      ? ((price - discountedPrice!) / price * 100) 
      : 0;
}

class MenuItemAddOn {
  final int id;
  final String nameAr;
  final String nameEn;
  final double price;
  final bool isAvailable;

  MenuItemAddOn({
    required this.id,
    required this.nameAr,
    required this.nameEn,
    required this.price,
    required this.isAvailable,
  });

  factory MenuItemAddOn.fromJson(Map<String, dynamic> json) {
    return MenuItemAddOn(
      id: json['id'],
      nameAr: json['nameAr'],
      nameEn: json['nameEn'],
      price: (json['price'] as num).toDouble(),
      isAvailable: json['isAvailable'] ?? true,
    );
  }

  String getName(bool isArabic) => isArabic ? nameAr : nameEn;
}
