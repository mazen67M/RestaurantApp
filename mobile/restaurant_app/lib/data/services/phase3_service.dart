import 'dart:convert';
import 'package:http/http.dart' as http;
import '../../core/constants/constants.dart';
import '../models/review.dart';
import '../models/loyalty.dart';

/// Service for Reviews and Loyalty API calls
/// Uses centralized ApiConstants for base URL
class Phase3Service {
  String? authToken;

  Phase3Service({this.authToken});

  void setAuthToken(String token) {
    authToken = token;
  }

  Map<String, String> get _headers {
    final headers = {'Content-Type': 'application/json'};
    if (authToken != null) {
      headers['Authorization'] = 'Bearer $authToken';
    }
    return headers;
  }

  // ==================== REVIEWS ====================

  /// Get reviews for a menu item
  Future<List<Review>> getItemReviews(int menuItemId) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.reviewsItem}/$menuItemId'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return (json['data'] as List)
              .map((item) => Review.fromJson(item))
              .toList();
        }
      }
      return [];
    } catch (e) {
      print('Error fetching reviews: $e');
      return [];
    }
  }

  /// Get review summary for a menu item
  Future<ReviewSummary> getItemReviewSummary(int menuItemId) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.reviewsItem}/$menuItemId/summary'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return ReviewSummary.fromJson(json['data']);
        }
      }
      return ReviewSummary(
        menuItemId: menuItemId,
        averageRating: 0,
        totalReviews: 0,
        fiveStarCount: 0,
        fourStarCount: 0,
        threeStarCount: 0,
        twoStarCount: 0,
        oneStarCount: 0,
      );
    } catch (e) {
      print('Error fetching review summary: $e');
      return ReviewSummary(
        menuItemId: menuItemId,
        averageRating: 0,
        totalReviews: 0,
        fiveStarCount: 0,
        fourStarCount: 0,
        threeStarCount: 0,
        twoStarCount: 0,
        oneStarCount: 0,
      );
    }
  }

  /// Get current user's reviews
  Future<List<Review>> getMyReviews() async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.reviewsMy}'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return (json['data'] as List)
              .map((item) => Review.fromJson(item))
              .toList();
        }
      }
      return [];
    } catch (e) {
      print('Error fetching my reviews: $e');
      return [];
    }
  }

  /// Create a new review
  Future<Review?> createReview(CreateReviewRequest request) async {
    try {
      final response = await http.post(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.reviews}'),
        headers: _headers,
        body: jsonEncode(request.toJson()),
      );

      if (response.statusCode == 201 || response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return Review.fromJson(json['data']);
        }
      } else {
        final json = jsonDecode(response.body);
        print('Create review error: ${json['message']}');
      }
      return null;
    } catch (e) {
      print('Error creating review: $e');
      return null;
    }
  }

  /// Check if user can review an item
  Future<bool> canReviewItem(int orderId, int menuItemId) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.reviewsCanReview}?orderId=$orderId&menuItemId=$menuItemId'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        return json['success'] == true && json['data'] == true;
      }
      return false;
    } catch (e) {
      print('Error checking review eligibility: $e');
      return false;
    }
  }

  // ==================== LOYALTY ====================

  /// Get current user's loyalty points
  Future<LoyaltyPoints?> getMyPoints() async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.loyalty}'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return LoyaltyPoints.fromJson(json['data']);
        }
      }
      return null;
    } catch (e) {
      print('Error fetching loyalty points: $e');
      return null;
    }
  }

  /// Get loyalty points for a specific user (demo)
  Future<LoyaltyPoints?> getUserPoints(String userId) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.loyalty}/user/$userId'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return LoyaltyPoints.fromJson(json['data']);
        }
      }
      return null;
    } catch (e) {
      print('Error fetching user points: $e');
      return null;
    }
  }

  /// Get transaction history
  Future<List<LoyaltyTransaction>> getTransactionHistory({int? limit}) async {
    try {
      String url = '${ApiConstants.baseUrl}${ApiConstants.loyaltyHistory}';
      if (limit != null) {
        url += '?limit=$limit';
      }

      final response = await http.get(
        Uri.parse(url),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return (json['data'] as List)
              .map((item) => LoyaltyTransaction.fromJson(item))
              .toList();
        }
      }
      return [];
    } catch (e) {
      print('Error fetching transaction history: $e');
      return [];
    }
  }

  /// Redeem points for a discount
  Future<RedeemResult?> redeemPoints(RedeemPointsRequest request) async {
    try {
      final response = await http.post(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.loyaltyRedeem}'),
        headers: _headers,
        body: jsonEncode(request.toJson()),
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return RedeemResult.fromJson(json['data']);
        }
      } else {
        final json = jsonDecode(response.body);
        print('Redeem error: ${json['message']}');
      }
      return null;
    } catch (e) {
      print('Error redeeming points: $e');
      return null;
    }
  }

  /// Get loyalty tier information
  Future<List<LoyaltyTier>> getTiers() async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.loyaltyTiers}'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return (json['data'] as List)
              .map((item) => LoyaltyTier.fromJson(item))
              .toList();
        }
      }
      return [];
    } catch (e) {
      print('Error fetching tiers: $e');
      return [];
    }
  }

  /// Calculate discount for a given number of points
  Future<double> calculateDiscount(int points) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.loyaltyCalculateDiscount}?points=$points'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        return (json['discountAmount'] ?? 0).toDouble();
      }
      return 0;
    } catch (e) {
      print('Error calculating discount: $e');
      return 0;
    }
  }

  // ==================== FAVORITES ====================

  /// Get current user's favorites
  Future<List<dynamic>> getFavorites() async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.favorites}'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return json['data'] as List;
        }
      }
      return [];
    } catch (e) {
      print('Error fetching favorites: $e');
      return [];
    }
  }

  /// Check if an item is in favorites
  Future<bool> isFavorite(int menuItemId) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.favoritesCheck}/$menuItemId'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        return json['success'] == true && json['data'] == true;
      }
      return false;
    } catch (e) {
      print('Error checking favorite: $e');
      return false;
    }
  }

  /// Add item to favorites
  Future<bool> addFavorite(int menuItemId) async {
    try {
      final response = await http.post(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.favorites}/$menuItemId'),
        headers: _headers,
      );
      return response.statusCode == 200;
    } catch (e) {
      print('Error adding favorite: $e');
      return false;
    }
  }

  /// Remove item from favorites
  Future<bool> removeFavorite(int menuItemId) async {
    try {
      final response = await http.delete(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.favorites}/$menuItemId'),
        headers: _headers,
      );
      return response.statusCode == 200;
    } catch (e) {
      print('Error removing favorite: $e');
      return false;
    }
  }

  /// Toggle favorite status
  Future<bool> toggleFavorite(int menuItemId) async {
    try {
      final response = await http.post(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.favorites}/$menuItemId/toggle'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        return json['success'] == true && json['data'] == true;
      }
      return false;
    } catch (e) {
      print('Error toggling favorite: $e');
      return false;
    }
  }

  /// Get favorite count
  Future<int> getFavoriteCount() async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.favoritesCount}'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return json['data'] as int;
        }
      }
      return 0;
    } catch (e) {
      print('Error getting favorite count: $e');
      return 0;
    }
  }
}
