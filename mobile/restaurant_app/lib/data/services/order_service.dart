import 'dart:convert';
import 'package:http/http.dart' as http;
import '../../core/constants/constants.dart';
import '../models/order.dart';

/// Service for order-related API calls
class OrderService {
  String? authToken;

  OrderService({this.authToken});

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

  /// Create new order
  Future<Order?> createOrder(Map<String, dynamic> orderData) async {
    try {
      final response = await http.post(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.orders}'),
        headers: _headers,
        body: jsonEncode(orderData),
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200 || response.statusCode == 201) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return Order.fromJson(json['data']);
        }
      }
      return null;
    } catch (e) {
      print('Error creating order: $e');
      return null;
    }
  }

  /// Get user's orders with pagination
  Future<List<Order>> getOrders({int page = 1, int pageSize = 10}) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.orders}?page=$page&pageSize=$pageSize'),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return (json['data'] as List)
              .map((item) => Order.fromJson(item))
              .toList();
        }
      }
      return [];
    } catch (e) {
      print('Error fetching orders: $e');
      return [];
    }
  }

  /// Get order by ID
  Future<Order?> getOrderById(int id) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}${ApiConstants.orders}/$id'),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return Order.fromJson(json['data']);
        }
      }
      return null;
    } catch (e) {
      print('Error fetching order: $e');
      return null;
    }
  }

  /// Track order status
  Future<Map<String, dynamic>?> trackOrder(int id) async {
    try {
      final trackUrl = ApiConstants.orderTrack.replaceAll('{id}', id.toString());
      final response = await http.get(
        Uri.parse('${ApiConstants.baseUrl}$trackUrl'),
        headers: _headers,
      ).timeout(ApiConstants.connectionTimeout);

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        if (json['success'] == true && json['data'] != null) {
          return json['data'] as Map<String, dynamic>;
        }
      }
      return null;
    } catch (e) {
      print('Error tracking order: $e');
      return null;
    }
  }

  /// Cancel order
  Future<bool> cancelOrder(int id, String reason) async {
    try {
      final cancelUrl = ApiConstants.orderCancel.replaceAll('{id}', id.toString());
      final response = await http.post(
        Uri.parse('${ApiConstants.baseUrl}$cancelUrl'),
        headers: _headers,
        body: jsonEncode({'reason': reason}),
      ).timeout(ApiConstants.connectionTimeout);

      return response.statusCode == 200;
    } catch (e) {
      print('Error cancelling order: $e');
      return false;
    }
  }

  /// Reorder (create new order from existing order)
  Future<Order?> reorder(int orderId) async {
    try {
      // First get the order details
      final order = await getOrderById(orderId);
      if (order == null) return null;

      // Create new order with same items
      final orderData = {
        'branchId': order.branchId,
        'items': order.items.map((item) => {
          'menuItemId': item.menuItemId,
          'quantity': item.quantity,
          'specialInstructions': item.specialInstructions,
        }).toList(),
        'deliveryAddressId': order.deliveryAddressId,
        'paymentMethod': order.paymentMethod,
      };

      return await createOrder(orderData);
    } catch (e) {
      print('Error reordering: $e');
      return null;
    }
  }
}
