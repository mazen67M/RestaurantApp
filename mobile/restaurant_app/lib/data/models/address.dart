/// Address model for delivery addresses
class Address {
  final int id;
  final String label;
  final String street;
  final String building;
  final String floor;
  final String apartment;
  final String city;
  final String area;
  final double latitude;
  final double longitude;
  final bool isDefault;
  final String? additionalDirections;

  Address({
    required this.id,
    required this.label,
    required this.street,
    required this.building,
    required this.floor,
    required this.apartment,
    required this.city,
    required this.area,
    required this.latitude,
    required this.longitude,
    required this.isDefault,
    this.additionalDirections,
  });

  factory Address.fromJson(Map<String, dynamic> json) {
    return Address(
      id: json['id'] ?? 0,
      label: json['label'] ?? '',
      street: json['street'] ?? '',
      building: json['building'] ?? '',
      floor: json['floor'] ?? '',
      apartment: json['apartment'] ?? '',
      city: json['city'] ?? '',
      area: json['area'] ?? '',
      latitude: (json['latitude'] ?? 0.0).toDouble(),
      longitude: (json['longitude'] ?? 0.0).toDouble(),
      isDefault: json['isDefault'] ?? false,
      additionalDirections: json['additionalDirections'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'label': label,
      'street': street,
      'building': building,
      'floor': floor,
      'apartment': apartment,
      'city': city,
      'area': area,
      'latitude': latitude,
      'longitude': longitude,
      'isDefault': isDefault,
      'additionalDirections': additionalDirections,
    };
  }

  String get fullAddress {
    final parts = <String>[];
    if (apartment.isNotEmpty) parts.add('Apt $apartment');
    if (floor.isNotEmpty) parts.add('Floor $floor');
    parts.add(building);
    parts.add(street);
    parts.add(area);
    parts.add(city);
    return parts.where((p) => p.isNotEmpty).join(', ');
  }
}
