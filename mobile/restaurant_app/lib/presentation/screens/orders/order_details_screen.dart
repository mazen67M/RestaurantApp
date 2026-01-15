import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/theme/app_theme.dart';
import '../../../core/localization/app_localizations.dart';
import '../../../data/models/order_model.dart';
import '../../../data/providers/auth_provider.dart';
import '../../../data/providers/locale_provider.dart';
import '../../../data/services/api_service.dart';
import '../../../core/constants/constants.dart';

class OrderDetailsScreen extends StatefulWidget {
  final int orderId;
  
  const OrderDetailsScreen({
    super.key,
    required this.orderId,
  });

  @override
  State<OrderDetailsScreen> createState() => _OrderDetailsScreenState();
}

class _OrderDetailsScreenState extends State<OrderDetailsScreen> {
  final ApiService _apiService = ApiService();
  Order? _order;
  bool _isLoading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadOrderDetails();
  }

  Future<void> _loadOrderDetails() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    try {
      final response = await _apiService.get<Order>(
        '${ApiConstants.orders}/${widget.orderId}',
        fromJson: (data) => Order.fromJson(data),
      );

      if (response.success && response.data != null) {
        _order = response.data;
      } else {
        _error = response.message ?? 'Failed to load order details';
      }
    } catch (e) {
      _error = 'An error occurred. Please try again.';
    }

    setState(() => _isLoading = false);
  }

  Future<void> _cancelOrder() async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(context.tr('cancel_order')),
        content: Text(context.tr('cancel_order_confirm')),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: Text(context.tr('no')),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            style: TextButton.styleFrom(foregroundColor: AppTheme.errorColor),
            child: Text(context.tr('yes')),
          ),
        ],
      ),
    );

    if (confirmed != true) return;

    setState(() => _isLoading = true);

    try {
      final response = await _apiService.post(
        '${ApiConstants.orders}/${widget.orderId}/cancel',
        body: {'reason': 'Customer requested cancellation'},
      );

      if (response.success && mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(context.tr('order_cancelled_success')),
            backgroundColor: AppTheme.successColor,
          ),
        );
        _loadOrderDetails();
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(response.message ?? 'Failed to cancel order'),
            backgroundColor: AppTheme.errorColor,
          ),
        );
        setState(() => _isLoading = false);
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('An error occurred'),
          backgroundColor: AppTheme.errorColor,
        ),
      );
      setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final isArabic = context.watch<LocaleProvider>().isArabic;

    return Scaffold(
      appBar: AppBar(
        title: Text(context.tr('order_details')),
      ),
      body: _buildBody(isArabic),
    );
  }

  Widget _buildBody(bool isArabic) {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_error != null) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(Icons.error_outline, size: 60, color: AppTheme.errorColor),
            const SizedBox(height: 16),
            Text(_error!),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _loadOrderDetails,
              child: Text(context.tr('retry')),
            ),
          ],
        ),
      );
    }

    if (_order == null) {
      return Center(child: Text(context.tr('order_not_found')));
    }

    return RefreshIndicator(
      onRefresh: _loadOrderDetails,
      child: SingleChildScrollView(
        physics: const AlwaysScrollableScrollPhysics(),
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Order header
            _buildOrderHeader(isArabic),
            
            const SizedBox(height: 16),
            
            // Status timeline
            _buildStatusTimeline(),
            
            const SizedBox(height: 24),
            
            // Order items
            _buildOrderItems(isArabic),
            
            const SizedBox(height: 24),
            
            // Delivery info
            if (_order!.deliveryAddressLine != null)
              _buildDeliveryInfo(),
            
            const SizedBox(height: 24),
            
            // Price breakdown
            _buildPriceBreakdown(),
            
            const SizedBox(height: 24),
            
            // Actions
            if (_order!.status == OrderStatus.pending)
              _buildCancelButton(),
              
            const SizedBox(height: 32),
          ],
        ),
      ),
    );
  }

  Widget _buildOrderHeader(bool isArabic) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      '${context.tr('order_number')}:',
                      style: TextStyle(
                        color: AppTheme.textSecondary,
                        fontSize: 12,
                      ),
                    ),
                    Text(
                      '#${_order!.orderNumber}',
                      style: const TextStyle(
                        fontSize: 20,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
                _buildStatusChip(_order!.status),
              ],
            ),
            const Divider(height: 24),
            Row(
              children: [
                const Icon(Icons.store_outlined, size: 20, color: AppTheme.primaryColor),
                const SizedBox(width: 8),
                Text(_order!.branch.getName(isArabic)),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                const Icon(Icons.access_time_outlined, size: 20, color: AppTheme.primaryColor),
                const SizedBox(width: 8),
                Text(_formatDateTime(_order!.createdAt)),
              ],
            ),
            if (_order!.estimatedDeliveryTime != null) ...[
              const SizedBox(height: 8),
              Row(
                children: [
                  const Icon(Icons.delivery_dining_outlined, size: 20, color: AppTheme.primaryColor),
                  const SizedBox(width: 8),
                  Text(
                    '${context.tr('estimated_time')}: ${_formatTime(_order!.estimatedDeliveryTime!)}',
                  ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildStatusTimeline() {
    final statuses = [
      OrderStatus.pending,
      OrderStatus.confirmed,
      OrderStatus.preparing,
      OrderStatus.ready,
      if (_order!.orderType == OrderType.delivery) OrderStatus.outForDelivery,
      OrderStatus.delivered,
    ];

    final currentIndex = statuses.indexOf(_order!.status);
    final isCancelled = _order!.status == OrderStatus.cancelled || 
                        _order!.status == OrderStatus.rejected;

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              context.tr('order_status'),
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            if (isCancelled)
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: AppTheme.errorColor.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.cancel_outlined, color: AppTheme.errorColor),
                    const SizedBox(width: 8),
                    Text(
                      context.tr('order_cancelled'),
                      style: const TextStyle(
                        color: AppTheme.errorColor,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
              )
            else
              ...List.generate(statuses.length, (index) {
                final isCompleted = index <= currentIndex;
                final isCurrent = index == currentIndex;
                
                return Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Column(
                      children: [
                        Container(
                          width: 24,
                          height: 24,
                          decoration: BoxDecoration(
                            color: isCompleted 
                                ? AppTheme.primaryColor 
                                : Colors.grey[300],
                            shape: BoxShape.circle,
                          ),
                          child: isCompleted
                              ? const Icon(Icons.check, size: 16, color: Colors.white)
                              : null,
                        ),
                        if (index < statuses.length - 1)
                          Container(
                            width: 2,
                            height: 30,
                            color: isCompleted 
                                ? AppTheme.primaryColor 
                                : Colors.grey[300],
                          ),
                      ],
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Padding(
                        padding: const EdgeInsets.only(bottom: 16),
                        child: Text(
                          _getStatusText(statuses[index]),
                          style: TextStyle(
                            fontWeight: isCurrent ? FontWeight.bold : FontWeight.normal,
                            color: isCompleted 
                                ? AppTheme.textPrimary 
                                : AppTheme.textSecondary,
                          ),
                        ),
                      ),
                    ),
                  ],
                );
              }),
          ],
        ),
      ),
    );
  }

  Widget _buildOrderItems(bool isArabic) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              context.tr('order_items'),
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            ...(_order!.items.map((item) => _buildOrderItem(item, isArabic))),
          ],
        ),
      ),
    );
  }

  Widget _buildOrderItem(OrderItem item, bool isArabic) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 32,
            height: 32,
            decoration: BoxDecoration(
              color: AppTheme.primaryColor.withOpacity(0.1),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Center(
              child: Text(
                '${item.quantity}x',
                style: const TextStyle(
                  fontWeight: FontWeight.bold,
                  color: AppTheme.primaryColor,
                ),
              ),
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  item.getName(isArabic),
                  style: const TextStyle(fontWeight: FontWeight.w500),
                ),
                if (item.addOns.isNotEmpty) ...[
                  const SizedBox(height: 4),
                  Text(
                    item.addOns.map((a) => a.getName(isArabic)).join(', '),
                    style: TextStyle(
                      fontSize: 12,
                      color: AppTheme.textSecondary,
                    ),
                  ),
                ],
                if (item.notes != null && item.notes!.isNotEmpty) ...[
                  const SizedBox(height: 4),
                  Text(
                    '📝 ${item.notes}',
                    style: TextStyle(
                      fontSize: 12,
                      color: AppTheme.textSecondary,
                      fontStyle: FontStyle.italic,
                    ),
                  ),
                ],
              ],
            ),
          ),
          Text(
            '${item.totalPrice.toStringAsFixed(0)} ${context.tr('currency')}',
            style: const TextStyle(fontWeight: FontWeight.bold),
          ),
        ],
      ),
    );
  }

  Widget _buildDeliveryInfo() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              context.tr('delivery_address'),
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                const Icon(Icons.location_on_outlined, color: AppTheme.primaryColor),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(_order!.deliveryAddressLine!),
                ),
              ],
            ),
            if (_order!.customerNotes != null && _order!.customerNotes!.isNotEmpty) ...[
              const SizedBox(height: 12),
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Icon(Icons.note_outlined, color: AppTheme.primaryColor),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      _order!.customerNotes!,
                      style: TextStyle(color: AppTheme.textSecondary),
                    ),
                  ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildPriceBreakdown() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              context.tr('price_breakdown'),
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            _buildPriceRow(context.tr('subtotal'), _order!.subTotal),
            if (_order!.discount > 0)
              _buildPriceRow(context.tr('discount'), -_order!.discount, isDiscount: true),
            _buildPriceRow(context.tr('delivery_fee'), _order!.deliveryFee),
            const Divider(height: 24),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  context.tr('total'),
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                Text(
                  '${_order!.total.toStringAsFixed(0)} ${context.tr('currency')}',
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: AppTheme.primaryColor,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPriceRow(String label, double amount, {bool isDiscount = false}) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            label,
            style: TextStyle(color: AppTheme.textSecondary),
          ),
          Text(
            '${isDiscount ? '-' : ''}${amount.abs().toStringAsFixed(0)} ${context.tr('currency')}',
            style: TextStyle(
              color: isDiscount ? AppTheme.successColor : null,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCancelButton() {
    return SizedBox(
      width: double.infinity,
      child: OutlinedButton(
        onPressed: _cancelOrder,
        style: OutlinedButton.styleFrom(
          foregroundColor: AppTheme.errorColor,
          side: const BorderSide(color: AppTheme.errorColor),
        ),
        child: Text(context.tr('cancel_order')),
      ),
    );
  }

  Widget _buildStatusChip(OrderStatus status) {
    Color color;
    String text;
    
    switch (status) {
      case OrderStatus.pending:
        color = AppTheme.warningColor;
        text = context.tr('pending');
        break;
      case OrderStatus.confirmed:
        color = AppTheme.infoColor;
        text = context.tr('confirmed');
        break;
      case OrderStatus.preparing:
        color = AppTheme.infoColor;
        text = context.tr('preparing');
        break;
      case OrderStatus.ready:
        color = AppTheme.successColor;
        text = context.tr('ready');
        break;
      case OrderStatus.outForDelivery:
        color = AppTheme.primaryColor;
        text = context.tr('out_for_delivery');
        break;
      case OrderStatus.delivered:
        color = AppTheme.successColor;
        text = context.tr('delivered');
        break;
      case OrderStatus.cancelled:
      case OrderStatus.rejected:
        color = AppTheme.errorColor;
        text = context.tr('cancelled');
        break;
    }

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(16),
      ),
      child: Text(
        text,
        style: TextStyle(
          color: color,
          fontWeight: FontWeight.bold,
          fontSize: 12,
        ),
      ),
    );
  }

  String _getStatusText(OrderStatus status) {
    switch (status) {
      case OrderStatus.pending:
        return context.tr('pending');
      case OrderStatus.confirmed:
        return context.tr('confirmed');
      case OrderStatus.preparing:
        return context.tr('preparing');
      case OrderStatus.ready:
        return context.tr('ready');
      case OrderStatus.outForDelivery:
        return context.tr('out_for_delivery');
      case OrderStatus.delivered:
        return context.tr('delivered');
      case OrderStatus.cancelled:
      case OrderStatus.rejected:
        return context.tr('cancelled');
    }
  }

  String _formatDateTime(DateTime dateTime) {
    return '${dateTime.day}/${dateTime.month}/${dateTime.year} ${_formatTime(dateTime)}';
  }

  String _formatTime(DateTime dateTime) {
    return '${dateTime.hour}:${dateTime.minute.toString().padLeft(2, '0')}';
  }
}
