import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../../core/constants/constants.dart';

class ApiService {
  static final ApiService _instance = ApiService._internal();
  factory ApiService() => _instance;
  ApiService._internal();

  final FlutterSecureStorage _storage = const FlutterSecureStorage();
  
  Future<String?> get _token async => await _storage.read(key: StorageKeys.accessToken);

  Future<Map<String, String>> get _headers async {
    final token = await _token;
    return {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      if (token != null) 'Authorization': 'Bearer $token',
    };
  }

  Future<ApiResponse<T>> get<T>(
    String endpoint, {
    Map<String, dynamic>? queryParams,
    T Function(dynamic)? fromJson,
  }) async {
    try {
      final uri = Uri.parse('${ApiConstants.baseUrl}$endpoint')
          .replace(queryParameters: queryParams?.map((k, v) => MapEntry(k, v.toString())));
      
      final response = await http.get(uri, headers: await _headers)
          .timeout(ApiConstants.connectionTimeout);
      
      return _handleResponse<T>(response, fromJson);
    } catch (e) {
      return ApiResponse.error(e.toString());
    }
  }

  Future<ApiResponse<T>> post<T>(
    String endpoint, {
    Map<String, dynamic>? body,
    T Function(dynamic)? fromJson,
  }) async {
    try {
      final uri = Uri.parse('${ApiConstants.baseUrl}$endpoint');
      
      final response = await http.post(
        uri,
        headers: await _headers,
        body: body != null ? jsonEncode(body) : null,
      ).timeout(ApiConstants.connectionTimeout);
      
      return _handleResponse<T>(response, fromJson);
    } catch (e) {
      return ApiResponse.error(e.toString());
    }
  }

  Future<ApiResponse<T>> put<T>(
    String endpoint, {
    Map<String, dynamic>? body,
    T Function(dynamic)? fromJson,
  }) async {
    try {
      final uri = Uri.parse('${ApiConstants.baseUrl}$endpoint');
      
      final response = await http.put(
        uri,
        headers: await _headers,
        body: body != null ? jsonEncode(body) : null,
      ).timeout(ApiConstants.connectionTimeout);
      
      return _handleResponse<T>(response, fromJson);
    } catch (e) {
      return ApiResponse.error(e.toString());
    }
  }

  Future<ApiResponse<T>> delete<T>(
    String endpoint, {
    T Function(dynamic)? fromJson,
  }) async {
    try {
      final uri = Uri.parse('${ApiConstants.baseUrl}$endpoint');
      
      final response = await http.delete(uri, headers: await _headers)
          .timeout(ApiConstants.connectionTimeout);
      
      return _handleResponse<T>(response, fromJson);
    } catch (e) {
      return ApiResponse.error(e.toString());
    }
  }

  ApiResponse<T> _handleResponse<T>(
    http.Response response,
    T Function(dynamic)? fromJson,
  ) {
    // Handle 401 Unauthorized - clear token
    if (response.statusCode == 401) {
      clearToken();
      return ApiResponse.error('Session expired. Please login again.');
    }
    
    // Handle empty response
    if (response.body.isEmpty) {
      return ApiResponse.error('Empty response from server');
    }
    
    final body = jsonDecode(response.body);
    
    if (response.statusCode >= 200 && response.statusCode < 300) {
      if (body['success'] == true || body['success'] == null) {
        // Handle both ApiResponse format and direct data
        final data = body['data'] ?? body;
        if (fromJson != null && data != null) {
          return ApiResponse.success(fromJson(data), body['message']);
        }
        return ApiResponse.success(data as T?, body['message']);
      }
    }
    
    final message = body['message'] ?? 'An error occurred';
    final errors = body['errors'] != null 
        ? List<String>.from(body['errors']) 
        : null;
    
    return ApiResponse.error(message, errors);
  }

  // Token management
  Future<void> saveToken(String token) async {
    await _storage.write(key: StorageKeys.accessToken, value: token);
  }

  Future<void> clearToken() async {
    await _storage.delete(key: StorageKeys.accessToken);
  }

  Future<bool> hasToken() async {
    final token = await _token;
    return token != null && token.isNotEmpty;
  }
}

class ApiResponse<T> {
  final bool success;
  final T? data;
  final String? message;
  final List<String>? errors;

  ApiResponse._({
    required this.success,
    this.data,
    this.message,
    this.errors,
  });

  factory ApiResponse.success(T? data, [String? message]) {
    return ApiResponse._(success: true, data: data, message: message);
  }

  factory ApiResponse.error(String message, [List<String>? errors]) {
    return ApiResponse._(success: false, message: message, errors: errors);
  }
}
