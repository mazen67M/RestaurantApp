import 'package:flutter/material.dart';
import '../models/restaurant_model.dart';
import '../models/menu_model.dart';
import '../services/api_service.dart';
import '../../core/constants/constants.dart';

class RestaurantProvider extends ChangeNotifier {
  final ApiService _apiService = ApiService();

  Restaurant? _restaurant;
  List<Branch> _branches = [];
  Branch? _selectedBranch;
  List<MenuCategory> _categories = [];
  Map<int, List<MenuItem>> _itemsByCategory = {};
  List<MenuItem> _popularItems = [];
  List<MenuItem> _searchResults = [];
  
  bool _isLoading = false;
  String? _error;

  Restaurant? get restaurant => _restaurant;
  List<Branch> get branches => _branches;
  Branch? get selectedBranch => _selectedBranch;
  List<MenuCategory> get categories => _categories;
  List<MenuItem> get popularItems => _popularItems;
  List<MenuItem> get searchResults => _searchResults;
  bool get isLoading => _isLoading;
  String? get error => _error;

  List<MenuItem> getItemsByCategory(int categoryId) => 
      _itemsByCategory[categoryId] ?? [];

  Future<void> loadRestaurant() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      final response = await _apiService.get<Map<String, dynamic>>(
        ApiConstants.restaurant,
        fromJson: (data) => data,
      );

      if (response.success && response.data != null) {
        _restaurant = Restaurant.fromJson(response.data!);
      } else {
        _error = response.message;
      }
    } catch (e) {
      _error = 'Failed to load restaurant';
    }

    _isLoading = false;
    notifyListeners();
  }

  Future<void> loadBranches({double? latitude, double? longitude}) async {
    try {
      final queryParams = <String, dynamic>{};
      if (latitude != null) queryParams['latitude'] = latitude;
      if (longitude != null) queryParams['longitude'] = longitude;

      final response = await _apiService.get<List<dynamic>>(
        ApiConstants.branches,
        queryParams: queryParams.isNotEmpty ? queryParams : null,
        fromJson: (data) => data,
      );

      if (response.success && response.data != null) {
        _branches = response.data!.map((b) => Branch.fromJson(b)).toList();
        
        // Auto-select first branch if none selected
        if (_selectedBranch == null && _branches.isNotEmpty) {
          _selectedBranch = _branches.first;
        }
      }
    } catch (e) {
      // Silent fail
    }
    notifyListeners();
  }

  Future<Branch?> findNearestBranch(double latitude, double longitude) async {
    try {
      final response = await _apiService.get<Map<String, dynamic>>(
        ApiConstants.nearestBranch,
        queryParams: {'latitude': latitude, 'longitude': longitude},
        fromJson: (data) => data,
      );

      if (response.success && response.data != null) {
        return Branch.fromJson(response.data!);
      }
    } catch (e) {
      // Return null
    }
    return null;
  }

  void selectBranch(Branch branch) {
    _selectedBranch = branch;
    notifyListeners();
  }

  Future<void> loadCategories() async {
    try {
      final response = await _apiService.get<List<dynamic>>(
        ApiConstants.menuCategories,
        fromJson: (data) => data,
      );

      if (response.success && response.data != null) {
        _categories = response.data!.map((c) => MenuCategory.fromJson(c)).toList();
      }
    } catch (e) {
      // Silent fail
    }
    notifyListeners();
  }

  Future<void> loadItemsByCategory(int categoryId) async {
    if (_itemsByCategory.containsKey(categoryId)) {
      notifyListeners();
      return;
    }

    try {
      final response = await _apiService.get<List<dynamic>>(
        '${ApiConstants.menuCategories}/$categoryId/items',
        fromJson: (data) => data,
      );

      if (response.success && response.data != null) {
        _itemsByCategory[categoryId] = 
            response.data!.map((i) => MenuItem.fromJson(i)).toList();
      }
    } catch (e) {
      _itemsByCategory[categoryId] = [];
    }
    notifyListeners();
  }

  Future<MenuItem?> getItemDetails(int itemId) async {
    try {
      final response = await _apiService.get<Map<String, dynamic>>(
        '${ApiConstants.menuItems}/$itemId',
        fromJson: (data) => data,
      );

      if (response.success && response.data != null) {
        return MenuItem.fromJson(response.data!);
      }
    } catch (e) {
      // Return null
    }
    return null;
  }

  Future<void> loadPopularItems({int count = 10}) async {
    try {
      final response = await _apiService.get<List<dynamic>>(
        ApiConstants.menuPopular,
        queryParams: {'count': count},
        fromJson: (data) => data,
      );

      if (response.success && response.data != null) {
        _popularItems = response.data!.map((i) => MenuItem.fromJson(i)).toList();
      }
    } catch (e) {
      _popularItems = [];
    }
    notifyListeners();
  }

  Future<void> searchItems(String query) async {
    if (query.trim().isEmpty) {
      _searchResults = [];
      notifyListeners();
      return;
    }

    try {
      final response = await _apiService.get<List<dynamic>>(
        ApiConstants.menuSearch,
        queryParams: {'q': query},
        fromJson: (data) => data,
      );

      if (response.success && response.data != null) {
        _searchResults = response.data!.map((i) => MenuItem.fromJson(i)).toList();
      }
    } catch (e) {
      _searchResults = [];
    }
    notifyListeners();
  }

  void clearSearch() {
    _searchResults = [];
    notifyListeners();
  }

  Future<void> loadAll() async {
    _isLoading = true;
    notifyListeners();

    await Future.wait([
      loadRestaurant(),
      loadBranches(),
      loadCategories(),
      loadPopularItems(),
    ]);

    _isLoading = false;
    notifyListeners();
  }
}
