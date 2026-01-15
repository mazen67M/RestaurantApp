import 'dart:convert';
import 'package:http/http.dart' as http;
import '../../core/constants/constants.dart';
import '../models/menu_category.dart';
import '../models/menu_item.dart';

/// Service for menu-related API calls
class MenuService {
  String? authToken;

  MenuService({this.authToken});

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

  /// Get all menu categories
  Future<List<MenuCategory>> getCategories() async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.menuCategories}'),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return (json['data'] as List)
              .map((item) => MenuCategory.fromJson(item))
              .toList();
        }
      }
      return [];
    } catch (e) {
      print('Error fetching categories: $e');
      return [];
    }
  }

  /// Get category by ID
  Future<MenuCategory?> getCategoryById(int id) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.menuCategories}/$id'),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return MenuCategory.fromJson(json['data']);
        }
      }
      return null;
    } catch (e) {
      print('Error fetching category: $e');
      return null;
    }
  }

  /// Get items by category
  Future<List<MenuItem>> getItemsByCategory(int categoryId) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.menuCategories}/$categoryId/items'),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return (json['data'] as List)
              .map((item) => MenuItem.fromJson(item))
              .toList();
        }
      }
      return [];
    } catch (e) {
      print('Error fetching items by category: $e');
      return [];
    }
  }

  /// Get all menu items
  Future<List<MenuItem>> getAllItems() async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.menuItems}'),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return (json['data'] as List)
              .map((item) => MenuItem.fromJson(item))
              .toList();
        }
      }
      return [];
    } catch (e) {
      print('Error fetching all items: $e');
      return [];
    }
  }

  /// Get menu item by ID
  Future<MenuItem?> getItemById(int id) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.menuItems}/$id'),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return MenuItem.fromJson(json['data']);
        }
      }
      return null;
    } catch (e) {
      print('Error fetching item: $e');
      return null;
    }
  }

  /// Search menu items
  Future<List<MenuItem>> searchItems(String query) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.menuSearch}?q=$query'),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return (json['data'] as List)
              .map((item) => MenuItem.fromJson(item))
              .toList();
        }
      }
      return [];
    } catch (e) {
      print('Error searching items: $e');
      return [];
    }
  }

  /// Get popular items
  Future<List<MenuItem>> getPopularItems({int count = 10}) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.menuPopular}?count=$count'),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return (json['data'] as List)
              .map((item) => MenuItem.fromJson(item))
              .toList();
        }
      }
      return [];
    } catch (e) {
      print('Error fetching popular items: $e');
      return [];
    }
  }
}
