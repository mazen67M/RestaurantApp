/// Customer loyalty points model
class LoyaltyPoints {
  final int id;
  final int points;
  final int totalEarned;
  final int totalRedeemed;
  final String tier;
  final double bonusMultiplier;
  final int pointsToNextTier;
  final String nextTier;

  LoyaltyPoints({
    required this.id,
    required this.points,
    required this.totalEarned,
    required this.totalRedeemed,
    required this.tier,
    required this.bonusMultiplier,
    required this.pointsToNextTier,
    required this.nextTier,
  });

  factory LoyaltyPoints.fromJson(Map<String, dynamic> json) {
    return LoyaltyPoints(
      id: json['id'] ?? 0,
      points: json['points'] ?? 0,
      totalEarned: json['totalEarned'] ?? 0,
      totalRedeemed: json['totalRedeemed'] ?? 0,
      tier: json['tier'] ?? 'Bronze',
      bonusMultiplier: (json['bonusMultiplier'] ?? 1.0).toDouble(),
      pointsToNextTier: json['pointsToNextTier'] ?? 0,
      nextTier: json['nextTier'] ?? 'Silver',
    );
  }

  /// Get tier color
  int get tierColor {
    switch (tier.toLowerCase()) {
      case 'platinum': return 0xFFE5E4E2;
      case 'gold': return 0xFFFFD700;
      case 'silver': return 0xFFC0C0C0;
      default: return 0xFFCD7F32;
    }
  }

  /// Get tier icon
  String get tierIcon {
    switch (tier.toLowerCase()) {
      case 'platinum': return '💎';
      case 'gold': return '🥇';
      case 'silver': return '🥈';
      default: return '🥉';
    }
  }

  /// Calculate discount for points (100 points = 1 AED)
  double calculateDiscount(int pointsToRedeem) {
    return pointsToRedeem * 0.01;
  }
}

/// Loyalty transaction history item
class LoyaltyTransaction {
  final int id;
  final int points;
  final String transactionType;
  final String? description;
  final int? orderId;
  final DateTime createdAt;

  LoyaltyTransaction({
    required this.id,
    required this.points,
    required this.transactionType,
    this.description,
    this.orderId,
    required this.createdAt,
  });

  factory LoyaltyTransaction.fromJson(Map<String, dynamic> json) {
    return LoyaltyTransaction(
      id: json['id'] ?? 0,
      points: json['points'] ?? 0,
      transactionType: json['transactionType'] ?? 'Earned',
      description: json['description'],
      orderId: json['orderId'],
      createdAt: json['createdAt'] != null
          ? DateTime.parse(json['createdAt'])
          : DateTime.now(),
    );
  }

  /// Check if this was an earning transaction
  bool get isEarning => points > 0;

  /// Get formatted points string
  String get formattedPoints {
    return isEarning ? '+$points' : '$points';
  }

  /// Get icon for transaction type
  String get icon {
    switch (transactionType.toLowerCase()) {
      case 'earned': return '📈';
      case 'redeemed': return '🎁';
      case 'bonus': return '⭐';
      case 'expired': return '⏰';
      default: return '💰';
    }
  }
}

/// Loyalty tier information
class LoyaltyTier {
  final String name;
  final int minPoints;
  final int maxPoints;
  final double bonusMultiplier;
  final String benefits;

  LoyaltyTier({
    required this.name,
    required this.minPoints,
    required this.maxPoints,
    required this.bonusMultiplier,
    required this.benefits,
  });

  factory LoyaltyTier.fromJson(Map<String, dynamic> json) {
    return LoyaltyTier(
      name: json['name'] ?? '',
      minPoints: json['minPoints'] ?? 0,
      maxPoints: json['maxPoints'] ?? 0,
      bonusMultiplier: (json['bonusMultiplier'] ?? 1.0).toDouble(),
      benefits: json['benefits'] ?? '',
    );
  }
}

/// Request to redeem points
class RedeemPointsRequest {
  final int points;
  final int? orderId;

  RedeemPointsRequest({
    required this.points,
    this.orderId,
  });

  Map<String, dynamic> toJson() {
    return {
      'points': points,
      'orderId': orderId,
    };
  }
}

/// Redemption result
class RedeemResult {
  final bool success;
  final int pointsRedeemed;
  final double discountAmount;
  final int remainingPoints;
  final String message;

  RedeemResult({
    required this.success,
    required this.pointsRedeemed,
    required this.discountAmount,
    required this.remainingPoints,
    required this.message,
  });

  factory RedeemResult.fromJson(Map<String, dynamic> json) {
    return RedeemResult(
      success: json['success'] ?? false,
      pointsRedeemed: json['pointsRedeemed'] ?? 0,
      discountAmount: (json['discountAmount'] ?? 0).toDouble(),
      remainingPoints: json['remainingPoints'] ?? 0,
      message: json['message'] ?? '',
    );
  }
}
