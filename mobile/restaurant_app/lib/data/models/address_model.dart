class UserAddress {
  final int id;
  final String label;
  final String addressLine;
  final String? buildingName;
  final String? floor;
  final String? apartment;
  final String? landmark;
  final double latitude;
  final double longitude;
  final bool isDefault;

  UserAddress({
    required this.id,
    required this.label,
    required this.addressLine,
    this.buildingName,
    this.floor,
    this.apartment,
    this.landmark,
    required this.latitude,
    required this.longitude,
    required this.isDefault,
  });

  factory UserAddress.fromJson(Map<String, dynamic> json) {
    return UserAddress(
      id: json['id'],
      label: json['label'],
      addressLine: json['addressLine'],
      buildingName: json['buildingName'],
      floor: json['floor'],
      apartment: json['apartment'],
      landmark: json['landmark'],
      latitude: (json['latitude'] as num).toDouble(),
      longitude: (json['longitude'] as num).toDouble(),
      isDefault: json['isDefault'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'label': label,
      'addressLine': addressLine,
      'buildingName': buildingName,
      'floor': floor,
      'apartment': apartment,
      'landmark': landmark,
      'latitude': latitude,
      'longitude': longitude,
      'isDefault': isDefault,
    };
  }

  String get fullAddress {
    final parts = <String>[addressLine];
    if (buildingName != null && buildingName!.isNotEmpty) {
      parts.add(buildingName!);
    }
    if (floor != null && floor!.isNotEmpty) {
      parts.add('Floor $floor');
    }
    if (apartment != null && apartment!.isNotEmpty) {
      parts.add('Apt $apartment');
    }
    return parts.join(', ');
  }
}
