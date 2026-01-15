import 'dart:convert';
import 'dart:math' as math;
import 'package:http/http.dart' as http;
import '../../core/constants/constants.dart';
import '../models/branch.dart';

/// Service for branch-related API calls
class BranchService {
  String? authToken;

  BranchService({this.authToken});

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

  /// Get all branches
  Future<List<Branch>> getBranches({double? latitude, double? longitude}) async {
    try {
      String url = '${ApiConstants.baseUrl}${ApiConstants.branches}';
      
      // Add location parameters if provided
      if (latitude != null && longitude != null) {
        url += '?latitude=$latitude&longitude=$longitude';
      }

      final response = await http.get(
        Uri.parse(url),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return (json['data'] as List)
              .map((item) => Branch.fromJson(item))
              .toList();
        }
      }
      return [];
    } catch (e) {
      print('Error fetching branches: $e');
      return [];
    }
  }

  /// Get branch by ID
  Future<Branch?> getBranchById(int id) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.branches}/$id'),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return Branch.fromJson(json['data']);
        }
      }
      return null;
    } catch (e) {
      print('Error fetching branch: $e');
      return null;
    }
  }

  /// Get nearest branch based on user location
  Future<Branch?> getNearestBranch(double latitude, double longitude) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.nearestBranch}?latitude=$latitude&longitude=$longitude'),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return Branch.fromJson(json['data']);
        }
      }
      return null;
    } catch (e) {
      print('Error fetching nearest branch: $e');
      return null;
    }
  }

  /// Calculate distance between two coordinates (Haversine formula)
  double calculateDistance(
    double lat1,
    double lon1,
    double lat2,
    double lon2,
  ) {
    const double earthRadius = 6371; // km

    final dLat = _toRadians(lat2 - lat1);
    final dLon = _toRadians(lon2 - lon1);

    final a = math.sin(dLat / 2) * math.sin(dLat / 2) +
        math.cos(_toRadians(lat1)) *
            math.cos(_toRadians(lat2)) *
            math.sin(dLon / 2) *
            math.sin(dLon / 2);

    final c = 2 * math.atan2(math.sqrt(a), math.sqrt(1 - a));

    return earthRadius * c;
  }

  double _toRadians(double degrees) {
    return degrees * (math.pi / 180);
  }
}
