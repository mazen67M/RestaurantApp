import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../models/menu_model.dart';
import '../models/order_model.dart';
import '../models/restaurant_model.dart';
import '../../core/constants/constants.dart';

class CartProvider extends ChangeNotifier {
  List<CartItem> _items = [];
  Branch? _selectedBranch;
  String? _couponCode;
  double _discount = 0;
  OrderType _orderType = OrderType.delivery;
  String? _deliveryAddress;
  double? _deliveryLat;
  double? _deliveryLng;
  String? _customerNotes;
  int _paymentMethod = 0; // 0 = CashOnDelivery, 1 = Online

  List<CartItem> get items => _items;
  Branch? get selectedBranch => _selectedBranch;
  String? get couponCode => _couponCode;
  double get discount => _discount;
  OrderType get orderType => _orderType;
  String? get deliveryAddress => _deliveryAddress;
  String? get customerNotes => _customerNotes;
  int get paymentMethod => _paymentMethod;

  int get itemCount => _items.fold(0, (sum, item) => sum + item.quantity);
  
  double get subTotal => 
      _items.fold(0, (sum, item) => sum + item.itemTotal);
  
  double get deliveryFee {
    if (_orderType == OrderType.pickup) return 0;
    if (_selectedBranch == null) return 0;
    
    // Free delivery if above threshold
    if (_selectedBranch!.freeDeliveryThreshold > 0 && 
        subTotal >= _selectedBranch!.freeDeliveryThreshold) {
      return 0;
    }
    return _selectedBranch!.deliveryFee;
  }
  
  double get total => subTotal + deliveryFee - _discount;

  bool get isEmpty => _items.isEmpty;
  bool get isNotEmpty => _items.isNotEmpty;

  bool get canCheckout {
    if (_selectedBranch == null) return false;
    if (subTotal < _selectedBranch!.minOrderAmount) return false;
    if (_orderType == OrderType.delivery && _deliveryAddress == null) return false;
    return true;
  }

  void addItem(MenuItem menuItem, {
    int quantity = 1,
    String? notes,
    List<MenuItemAddOn>? addOns,
  }) {
    // Check if item already exists with same add-ons
    final existingIndex = _items.indexWhere((item) => 
        item.menuItem.id == menuItem.id &&
        _addOnsMatch(item.selectedAddOns, addOns ?? []));
    
    if (existingIndex != -1) {
      _items[existingIndex].quantity += quantity;
      if (notes != null) _items[existingIndex].notes = notes;
    } else {
      _items.add(CartItem(
        menuItem: menuItem,
        quantity: quantity,
        notes: notes,
        selectedAddOns: addOns ?? [],
      ));
    }
    
    _saveCart();
    notifyListeners();
  }

  void updateItemQuantity(int index, int quantity) {
    if (index < 0 || index >= _items.length) return;
    
    if (quantity <= 0) {
      _items.removeAt(index);
    } else {
      _items[index].quantity = quantity;
    }
    
    _saveCart();
    notifyListeners();
  }

  void removeItem(int index) {
    if (index < 0 || index >= _items.length) return;
    _items.removeAt(index);
    _saveCart();
    notifyListeners();
  }

  void clearCart() {
    _items.clear();
    _couponCode = null;
    _discount = 0;
    _customerNotes = null;
    _saveCart();
    notifyListeners();
  }

  void setSelectedBranch(Branch branch) {
    _selectedBranch = branch;
    notifyListeners();
  }

  void setOrderType(OrderType type) {
    _orderType = type;
    notifyListeners();
  }

  void setDeliveryAddress(String address, double lat, double lng) {
    _deliveryAddress = address;
    _deliveryLat = lat;
    _deliveryLng = lng;
    notifyListeners();
  }

  void setCustomerNotes(String? notes) {
    _customerNotes = notes;
    notifyListeners();
  }

  void setPaymentMethod(int method) {
    _paymentMethod = method;
    notifyListeners();
  }

  void applyCoupon(String code, double discountAmount) {
    _couponCode = code;
    _discount = discountAmount;
    notifyListeners();
  }

  void removeCoupon() {
    _couponCode = null;
    _discount = 0;
    notifyListeners();
  }

  Map<String, dynamic> toOrderJson() {
    return {
      'branchId': _selectedBranch!.id,
      'orderType': _orderType.index,
      'paymentMethod': _paymentMethod,
      'deliveryAddressLine': _deliveryAddress,
      'deliveryLatitude': _deliveryLat,
      'deliveryLongitude': _deliveryLng,
      'customerNotes': _customerNotes,
      'couponCode': _couponCode,
      'items': _items.map((item) => item.toOrderItemJson()).toList(),
    };
  }

  bool _addOnsMatch(List<MenuItemAddOn> a, List<MenuItemAddOn> b) {
    if (a.length != b.length) return false;
    final aIds = a.map((x) => x.id).toSet();
    final bIds = b.map((x) => x.id).toSet();
    return aIds.containsAll(bIds) && bIds.containsAll(aIds);
  }

  Future<void> _saveCart() async {
    // Simplified cart persistence - in production, serialize properly
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(StorageKeys.cart, jsonEncode({
      'itemCount': _items.length,
      // Full serialization would go here
    }));
  }

  Future<void> loadCart() async {
    // Load cart from storage
    // Implementation depends on requirements
  }
}
