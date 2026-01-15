import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../providers/order_tracking_provider.dart';
import '../../../data/services/signalr_service.dart';

class OrderTrackingScreen extends StatefulWidget {
  final int orderId;
  final String orderNumber;

  const OrderTrackingScreen({
    super.key,
    required this.orderId,
    required this.orderNumber,
  });

  @override
  State<OrderTrackingScreen> createState() => _OrderTrackingScreenState();
}

class _OrderTrackingScreenState extends State<OrderTrackingScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final provider = context.read<OrderTrackingProvider>();
      provider.startTrackingOrder(widget.orderId);
    });
  }

  @override
  void dispose() {
    // Don't stop tracking on dispose - let the user continue receiving updates
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F5F5),
      appBar: AppBar(
        backgroundColor: Colors.white,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back, color: Colors.black87),
          onPressed: () => Navigator.pop(context),
        ),
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Track Order',
              style: TextStyle(
                color: Colors.black87,
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            Text(
              '#${widget.orderNumber}',
              style: TextStyle(
                color: Colors.grey[600],
                fontSize: 12,
              ),
            ),
          ],
        ),
      ),
      body: Consumer<OrderTrackingProvider>(
        builder: (context, provider, child) {
          return SingleChildScrollView(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Connection Status
                _buildConnectionStatus(provider),
                const SizedBox(height: 20),
                
                // Order Status Card
                _buildStatusCard(provider),
                const SizedBox(height: 20),
                
                // Status Timeline
                _buildStatusTimeline(provider),
                const SizedBox(height: 20),
                
                // Estimated Time
                if (provider.latestStatusUpdate?.estimatedTime != null)
                  _buildEstimatedTime(provider.latestStatusUpdate!.estimatedTime!),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildConnectionStatus(OrderTrackingProvider provider) {
    final isConnected = provider.isConnected;
    
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
      decoration: BoxDecoration(
        color: isConnected ? Colors.green[50] : Colors.orange[50],
        borderRadius: BorderRadius.circular(20),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            width: 8,
            height: 8,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: isConnected ? Colors.green : Colors.orange,
            ),
          ),
          const SizedBox(width: 8),
          Text(
            isConnected ? 'Live tracking' : 'Connecting...',
            style: TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w500,
              color: isConnected ? Colors.green[700] : Colors.orange[700],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildStatusCard(OrderTrackingProvider provider) {
    final update = provider.latestStatusUpdate;
    final status = update?.statusText ?? 'Waiting for update...';
    
    return Container(
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            Theme.of(context).primaryColor,
            Theme.of(context).primaryColor.withOpacity(0.8),
          ],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: Theme.of(context).primaryColor.withOpacity(0.3),
            blurRadius: 15,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Column(
        children: [
          Icon(
            _getStatusIcon(update?.status ?? ''),
            size: 60,
            color: Colors.white,
          ),
          const SizedBox(height: 16),
          Text(
            status,
            style: const TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
              color: Colors.white,
            ),
            textAlign: TextAlign.center,
          ),
          if (update != null) ...[
            const SizedBox(height: 8),
            Text(
              'Updated ${_formatTime(update.updatedAt)}',
              style: TextStyle(
                fontSize: 12,
                color: Colors.white.withOpacity(0.8),
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildStatusTimeline(OrderTrackingProvider provider) {
    final currentStatus = provider.latestStatusUpdate?.status ?? 'Pending';
    
    final statuses = [
      {'key': 'Pending', 'label': 'Order Received', 'icon': Icons.receipt_long},
      {'key': 'Confirmed', 'label': 'Order Confirmed', 'icon': Icons.check_circle},
      {'key': 'Preparing', 'label': 'Preparing', 'icon': Icons.restaurant},
      {'key': 'Ready', 'label': 'Ready', 'icon': Icons.done_all},
      {'key': 'OutForDelivery', 'label': 'On the Way', 'icon': Icons.delivery_dining},
      {'key': 'Delivered', 'label': 'Delivered', 'icon': Icons.home},
    ];
    
    final currentIndex = statuses.indexWhere((s) => s['key'] == currentStatus);
    
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.05),
            blurRadius: 10,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Order Progress',
            style: TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 20),
          ...statuses.asMap().entries.map((entry) {
            final index = entry.key;
            final status = entry.value;
            final isActive = index <= currentIndex;
            final isCurrent = index == currentIndex;
            
            return _buildTimelineItem(
              label: status['label'] as String,
              icon: status['icon'] as IconData,
              isActive: isActive,
              isCurrent: isCurrent,
              isLast: index == statuses.length - 1,
            );
          }),
        ],
      ),
    );
  }

  Widget _buildTimelineItem({
    required String label,
    required IconData icon,
    required bool isActive,
    required bool isCurrent,
    required bool isLast,
  }) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Column(
          children: [
            Container(
              width: 40,
              height: 40,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                color: isActive
                    ? Theme.of(context).primaryColor
                    : Colors.grey[200],
                border: isCurrent
                    ? Border.all(
                        color: Theme.of(context).primaryColor,
                        width: 3,
                      )
                    : null,
              ),
              child: Icon(
                icon,
                size: 20,
                color: isActive ? Colors.white : Colors.grey[400],
              ),
            ),
            if (!isLast)
              Container(
                width: 2,
                height: 30,
                color: isActive
                    ? Theme.of(context).primaryColor
                    : Colors.grey[200],
              ),
          ],
        ),
        const SizedBox(width: 16),
        Expanded(
          child: Padding(
            padding: const EdgeInsets.only(top: 8),
            child: Text(
              label,
              style: TextStyle(
                fontSize: 14,
                fontWeight: isCurrent ? FontWeight.bold : FontWeight.normal,
                color: isActive ? Colors.black87 : Colors.grey[400],
              ),
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildEstimatedTime(String time) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.blue[50],
        borderRadius: BorderRadius.circular(12),
      ),
      child: Row(
        children: [
          Icon(Icons.access_time, color: Colors.blue[700]),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Estimated Delivery',
                  style: TextStyle(
                    fontSize: 12,
                    color: Colors.grey,
                  ),
                ),
                Text(
                  time,
                  style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    color: Colors.blue[700],
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  IconData _getStatusIcon(String status) {
    switch (status) {
      case 'Pending':
        return Icons.receipt_long;
      case 'Confirmed':
        return Icons.check_circle;
      case 'Preparing':
        return Icons.restaurant;
      case 'Ready':
        return Icons.done_all;
      case 'OutForDelivery':
        return Icons.delivery_dining;
      case 'Delivered':
        return Icons.home;
      case 'Cancelled':
        return Icons.cancel;
      default:
        return Icons.hourglass_empty;
    }
  }

  String _formatTime(DateTime time) {
    final now = DateTime.now();
    final diff = now.difference(time);
    
    if (diff.inSeconds < 60) return 'just now';
    if (diff.inMinutes < 60) return '${diff.inMinutes}m ago';
    if (diff.inHours < 24) return '${diff.inHours}h ago';
    return '${diff.inDays}d ago';
  }
}
