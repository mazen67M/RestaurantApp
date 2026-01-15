/// Review model for customer reviews
class Review {
  final int id;
  final int menuItemId;
  final String menuItemName;
  final int orderId;
  final String customerName;
  final int rating;
  final String? comment;
  final bool isApproved;
  final DateTime createdAt;

  Review({
    required this.id,
    required this.menuItemId,
    required this.menuItemName,
    required this.orderId,
    required this.customerName,
    required this.rating,
    this.comment,
    required this.isApproved,
    required this.createdAt,
  });

  factory Review.fromJson(Map<String, dynamic> json) {
    return Review(
      id: json['id'] ?? 0,
      menuItemId: json['menuItemId'] ?? 0,
      menuItemName: json['menuItemName'] ?? '',
      orderId: json['orderId'] ?? 0,
      customerName: json['customerName'] ?? 'Customer',
      rating: json['rating'] ?? 0,
      comment: json['comment'],
      isApproved: json['isApproved'] ?? false,
      createdAt: json['createdAt'] != null
          ? DateTime.parse(json['createdAt'])
          : DateTime.now(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'menuItemId': menuItemId,
      'menuItemName': menuItemName,
      'orderId': orderId,
      'customerName': customerName,
      'rating': rating,
      'comment': comment,
      'isApproved': isApproved,
      'createdAt': createdAt.toIso8601String(),
    };
  }
}

/// Review summary with rating statistics
class ReviewSummary {
  final int menuItemId;
  final double averageRating;
  final int totalReviews;
  final int fiveStarCount;
  final int fourStarCount;
  final int threeStarCount;
  final int twoStarCount;
  final int oneStarCount;

  ReviewSummary({
    required this.menuItemId,
    required this.averageRating,
    required this.totalReviews,
    required this.fiveStarCount,
    required this.fourStarCount,
    required this.threeStarCount,
    required this.twoStarCount,
    required this.oneStarCount,
  });

  factory ReviewSummary.fromJson(Map<String, dynamic> json) {
    return ReviewSummary(
      menuItemId: json['menuItemId'] ?? 0,
      averageRating: (json['averageRating'] ?? 0).toDouble(),
      totalReviews: json['totalReviews'] ?? 0,
      fiveStarCount: json['fiveStarCount'] ?? 0,
      fourStarCount: json['fourStarCount'] ?? 0,
      threeStarCount: json['threeStarCount'] ?? 0,
      twoStarCount: json['twoStarCount'] ?? 0,
      oneStarCount: json['oneStarCount'] ?? 0,
    );
  }

  /// Get the count for a specific star rating
  int getStarCount(int stars) {
    switch (stars) {
      case 5: return fiveStarCount;
      case 4: return fourStarCount;
      case 3: return threeStarCount;
      case 2: return twoStarCount;
      case 1: return oneStarCount;
      default: return 0;
    }
  }

  /// Get the percentage for a specific star rating
  double getStarPercentage(int stars) {
    if (totalReviews == 0) return 0;
    return getStarCount(stars) / totalReviews * 100;
  }
}

/// Create review request
class CreateReviewRequest {
  final int orderId;
  final int menuItemId;
  final int rating;
  final String? comment;

  CreateReviewRequest({
    required this.orderId,
    required this.menuItemId,
    required this.rating,
    this.comment,
  });

  Map<String, dynamic> toJson() {
    return {
      'orderId': orderId,
      'menuItemId': menuItemId,
      'rating': rating,
      'comment': comment,
    };
  }
}
