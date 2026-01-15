/// Branch model representing a restaurant location
class Branch {
  final int id;
  final String nameEn;
  final String nameAr;
  final String address;
  final String phone;
  final double latitude;
  final double longitude;
  final String openingTime;
  final String closingTime;
  final bool isActive;
  final bool acceptingOrders;
  final double? distanceKm; // Calculated distance from user

  Branch({
    required this.id,
    required this.nameEn,
    required this.nameAr,
    required this.address,
    required this.phone,
    required this.latitude,
    required this.longitude,
    required this.openingTime,
    required this.closingTime,
    required this.isActive,
    required this.acceptingOrders,
    this.distanceKm,
  });

  factory Branch.fromJson(Map<String, dynamic> json) {
    return Branch(
      id: json['id'] ?? 0,
      nameEn: json['nameEn'] ?? '',
      nameAr: json['nameAr'] ?? '',
      address: json['address'] ?? '',
      phone: json['phone'] ?? '',
      latitude: (json['latitude'] ?? 0.0).toDouble(),
      longitude: (json['longitude'] ?? 0.0).toDouble(),
      openingTime: json['openingTime'] ?? '09:00',
      closingTime: json['closingTime'] ?? '23:00',
      isActive: json['isActive'] ?? true,
      acceptingOrders: json['acceptingOrders'] ?? true,
      distanceKm: json['distanceKm'] != null 
          ? (json['distanceKm']).toDouble() 
          : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'nameEn': nameEn,
      'nameAr': nameAr,
      'address': address,
      'phone': phone,
      'latitude': latitude,
      'longitude': longitude,
      'openingTime': openingTime,
      'closingTime': closingTime,
      'isActive': isActive,
      'acceptingOrders': acceptingOrders,
      'distanceKm': distanceKm,
    };
  }

  String getName(bool isArabic) => isArabic ? nameAr : nameEn;

  /// Check if branch is currently open
  bool isOpen() {
    final now = DateTime.now();
    final opening = _parseTime(openingTime);
    final closing = _parseTime(closingTime);
    
    final currentMinutes = now.hour * 60 + now.minute;
    final openingMinutes = opening.hour * 60 + opening.minute;
    final closingMinutes = closing.hour * 60 + closing.minute;

    return currentMinutes >= openingMinutes && currentMinutes <= closingMinutes;
  }

  DateTime _parseTime(String time) {
    final parts = time.split(':');
    return DateTime(0, 1, 1, int.parse(parts[0]), int.parse(parts[1]));
  }

  Branch copyWith({
    int? id,
    String? nameEn,
    String? nameAr,
    String? address,
    String? phone,
    double? latitude,
    double? longitude,
    String? openingTime,
    String? closingTime,
    bool? isActive,
    bool? acceptingOrders,
    double? distanceKm,
  }) {
    return Branch(
      id: id ?? this.id,
      nameEn: nameEn ?? this.nameEn,
      nameAr: nameAr ?? this.nameAr,
      address: address ?? this.address,
      phone: phone ?? this.phone,
      latitude: latitude ?? this.latitude,
      longitude: longitude ?? this.longitude,
      openingTime: openingTime ?? this.openingTime,
      closingTime: closingTime ?? this.closingTime,
      isActive: isActive ?? this.isActive,
      acceptingOrders: acceptingOrders ?? this.acceptingOrders,
      distanceKm: distanceKm ?? this.distanceKm,
    );
  }
}
