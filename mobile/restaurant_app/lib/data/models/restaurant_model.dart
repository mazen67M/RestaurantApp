class Restaurant {
  final int id;
  final String nameAr;
  final String nameEn;
  final String? descriptionAr;
  final String? descriptionEn;
  final String? logoUrl;
  final String? coverImageUrl;
  final String primaryColor;
  final String secondaryColor;
  final String? phone;
  final String? email;
  final bool isActive;

  Restaurant({
    required this.id,
    required this.nameAr,
    required this.nameEn,
    this.descriptionAr,
    this.descriptionEn,
    this.logoUrl,
    this.coverImageUrl,
    required this.primaryColor,
    required this.secondaryColor,
    this.phone,
    this.email,
    required this.isActive,
  });

  factory Restaurant.fromJson(Map<String, dynamic> json) {
    return Restaurant(
      id: json['id'],
      nameAr: json['nameAr'],
      nameEn: json['nameEn'],
      descriptionAr: json['descriptionAr'],
      descriptionEn: json['descriptionEn'],
      logoUrl: json['logoUrl'],
      coverImageUrl: json['coverImageUrl'],
      primaryColor: json['primaryColor'],
      secondaryColor: json['secondaryColor'],
      phone: json['phone'],
      email: json['email'],
      isActive: json['isActive'],
    );
  }

  String getName(bool isArabic) => isArabic ? nameAr : nameEn;
  String? getDescription(bool isArabic) => isArabic ? descriptionAr : descriptionEn;
}

class Branch {
  final int id;
  final int restaurantId;
  final String nameAr;
  final String nameEn;
  final String addressAr;
  final String addressEn;
  final double latitude;
  final double longitude;
  final String? phone;
  final double deliveryRadiusKm;
  final double minOrderAmount;
  final double deliveryFee;
  final double freeDeliveryThreshold;
  final int defaultPreparationTimeMinutes;
  final String openingTime;
  final String closingTime;
  final bool isActive;
  final bool acceptingOrders;
  final double? distanceKm;

  Branch({
    required this.id,
    required this.restaurantId,
    required this.nameAr,
    required this.nameEn,
    required this.addressAr,
    required this.addressEn,
    required this.latitude,
    required this.longitude,
    this.phone,
    required this.deliveryRadiusKm,
    required this.minOrderAmount,
    required this.deliveryFee,
    required this.freeDeliveryThreshold,
    required this.defaultPreparationTimeMinutes,
    required this.openingTime,
    required this.closingTime,
    required this.isActive,
    required this.acceptingOrders,
    this.distanceKm,
  });

  factory Branch.fromJson(Map<String, dynamic> json) {
    return Branch(
      id: json['id'],
      restaurantId: json['restaurantId'],
      nameAr: json['nameAr'],
      nameEn: json['nameEn'],
      addressAr: json['addressAr'],
      addressEn: json['addressEn'],
      latitude: (json['latitude'] as num).toDouble(),
      longitude: (json['longitude'] as num).toDouble(),
      phone: json['phone'],
      deliveryRadiusKm: (json['deliveryRadiusKm'] as num).toDouble(),
      minOrderAmount: (json['minOrderAmount'] as num).toDouble(),
      deliveryFee: (json['deliveryFee'] as num).toDouble(),
      freeDeliveryThreshold: (json['freeDeliveryThreshold'] as num).toDouble(),
      defaultPreparationTimeMinutes: json['defaultPreparationTimeMinutes'],
      openingTime: json['openingTime'],
      closingTime: json['closingTime'],
      isActive: json['isActive'],
      acceptingOrders: json['acceptingOrders'],
      distanceKm: json['distanceKm']?.toDouble(),
    );
  }

  String getName(bool isArabic) => isArabic ? nameAr : nameEn;
  String getAddress(bool isArabic) => isArabic ? addressAr : addressEn;
}
