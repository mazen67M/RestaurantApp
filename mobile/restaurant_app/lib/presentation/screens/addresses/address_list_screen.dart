import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../data/models/address.dart';
import '../../../data/providers/address_provider.dart';
import '../../../data/providers/locale_provider.dart';
import '../../../core/theme/app_theme.dart';
import 'add_edit_address_screen.dart';

class AddressListScreen extends StatefulWidget {
  final bool isFromCheckout;

  const AddressListScreen({
    super.key,
    this.isFromCheckout = false,
  });

  @override
  State<AddressListScreen> createState() => _AddressListScreenState();
}

class _AddressListScreenState extends State<AddressListScreen> {
  @override
  void initState() {
    super.initState();
    _loadAddresses();
  }

  Future<void> _loadAddresses() async {
    await context.read<AddressProvider>().loadAddresses();
  }

  @override
  Widget build(BuildContext context) {
    final isArabic = context.watch<LocaleProvider>().isArabic;
    
    return Scaffold(
      appBar: AppBar(
        title: Text(isArabic ? 'العناوين' : 'Addresses'),
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: Colors.white,
      ),
      body: Consumer<AddressProvider>(
        builder: (context, addressProvider, child) {
          if (addressProvider.isLoading) {
            return const Center(child: CircularProgressIndicator());
          }

          if (addressProvider.error != null) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.error_outline, size: 64, color: Colors.grey.shade400),
                  const SizedBox(height: 16),
                  Text(
                    isArabic ? 'حدث خطأ' : 'Error occurred',
                    style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    addressProvider.error!,
                    textAlign: TextAlign.center,
                    style: TextStyle(color: Colors.grey.shade600),
                  ),
                  const SizedBox(height: 24),
                  ElevatedButton.icon(
                    onPressed: _loadAddresses,
                    icon: const Icon(Icons.refresh),
                    label: Text(isArabic ? 'إعادة المحاولة' : 'Retry'),
                  ),
                ],
              ),
            );
          }

          if (!addressProvider.hasAddresses) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.location_off_outlined, size: 64, color: Colors.grey.shade400),
                  const SizedBox(height: 16),
                  Text(
                    isArabic ? 'لا توجد عناوين محفوظة' : 'No saved addresses',
                    style: const TextStyle(fontSize: 18),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    isArabic ? 'أضف عنوان جديد للتوصيل' : 'Add a new delivery address',
                    style: TextStyle(color: Colors.grey.shade600),
                  ),
                  const SizedBox(height: 24),
                  ElevatedButton.icon(
                    onPressed: () => _addNewAddress(context, isArabic),
                    icon: const Icon(Icons.add),
                    label: Text(isArabic ? 'إضافة عنوان' : 'Add Address'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppTheme.primaryColor,
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
                    ),
                  ),
                ],
              ),
            );
          }

          return RefreshIndicator(
            onRefresh: _loadAddresses,
            child: ListView.builder(
              padding: const EdgeInsets.all(16),
              itemCount: addressProvider.addresses.length,
              itemBuilder: (context, index) {
                final address = addressProvider.addresses[index];
                final isSelected = addressProvider.selectedAddress?.id == address.id;
                
                return _buildAddressCard(
                  address,
                  isSelected,
                  isArabic,
                  addressProvider,
                );
              },
            ),
          );
        },
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => _addNewAddress(context, isArabic),
        icon: const Icon(Icons.add),
        label: Text(isArabic ? 'إضافة عنوان' : 'Add Address'),
        backgroundColor: AppTheme.primaryColor,
      ),
    );
  }

  Widget _buildAddressCard(
    Address address,
    bool isSelected,
    bool isArabic,
    AddressProvider addressProvider,
  ) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      elevation: isSelected ? 4 : 1,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(
          color: isSelected ? AppTheme.primaryColor : Colors.transparent,
          width: 2,
        ),
      ),
      child: InkWell(
        onTap: () {
          addressProvider.selectAddress(address);
          if (widget.isFromCheckout) {
            Navigator.pop(context, address);
          }
        },
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Container(
                    padding: const EdgeInsets.all(10),
                    decoration: BoxDecoration(
                      color: isSelected 
                          ? AppTheme.primaryColor.withOpacity(0.1)
                          : Colors.grey.shade100,
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Icon(
                      _getAddressIcon(address.label),
                      color: isSelected ? AppTheme.primaryColor : Colors.grey.shade600,
                      size: 24,
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          children: [
                            Text(
                              address.label,
                              style: TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.bold,
                                color: isSelected ? AppTheme.primaryColor : Colors.black87,
                              ),
                            ),
                            if (address.isDefault) ...[
                              const SizedBox(width: 8),
                              Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 8,
                                  vertical: 4,
                                ),
                                decoration: BoxDecoration(
                                  color: Colors.green.shade50,
                                  borderRadius: BorderRadius.circular(4),
                                ),
                                child: Text(
                                  isArabic ? 'افتراضي' : 'Default',
                                  style: TextStyle(
                                    color: Colors.green.shade700,
                                    fontSize: 11,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              ),
                            ],
                          ],
                        ),
                        const SizedBox(height: 4),
                        Text(
                          '${address.building}, ${address.street}',
                          style: TextStyle(
                            color: Colors.grey.shade600,
                            fontSize: 14,
                          ),
                        ),
                      ],
                    ),
                  ),
                  if (isSelected)
                    const Icon(
                      Icons.check_circle,
                      color: AppTheme.primaryColor,
                      size: 24,
                    ),
                ],
              ),
              const SizedBox(height: 12),
              const Divider(height: 1),
              const SizedBox(height: 12),
              Text(
                _getFullAddress(address),
                style: TextStyle(
                  color: Colors.grey.shade700,
                  fontSize: 14,
                ),
              ),
              if (address.additionalDirections != null &&
                  address.additionalDirections!.isNotEmpty) ...[
                const SizedBox(height: 8),
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Icon(Icons.info_outline, size: 16, color: Colors.grey.shade600),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        address.additionalDirections!,
                        style: TextStyle(
                          color: Colors.grey.shade600,
                          fontSize: 13,
                          fontStyle: FontStyle.italic,
                        ),
                      ),
                    ),
                  ],
                ),
              ],
              const SizedBox(height: 12),
              Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  if (!address.isDefault)
                    TextButton.icon(
                      onPressed: () => _setAsDefault(address.id, addressProvider, isArabic),
                      icon: const Icon(Icons.star_outline, size: 18),
                      label: Text(isArabic ? 'تعيين كافتراضي' : 'Set as Default'),
                      style: TextButton.styleFrom(
                        foregroundColor: AppTheme.primaryColor,
                      ),
                    ),
                  TextButton.icon(
                    onPressed: () => _editAddress(context, address, isArabic),
                    icon: const Icon(Icons.edit_outlined, size: 18),
                    label: Text(isArabic ? 'تعديل' : 'Edit'),
                  ),
                  TextButton.icon(
                    onPressed: () => _deleteAddress(address.id, addressProvider, isArabic),
                    icon: const Icon(Icons.delete_outline, size: 18),
                    label: Text(isArabic ? 'حذف' : 'Delete'),
                    style: TextButton.styleFrom(
                      foregroundColor: Colors.red,
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  IconData _getAddressIcon(String label) {
    final lowerLabel = label.toLowerCase();
    if (lowerLabel.contains('home') || lowerLabel.contains('منزل')) {
      return Icons.home;
    } else if (lowerLabel.contains('work') || lowerLabel.contains('عمل')) {
      return Icons.work;
    } else {
      return Icons.location_on;
    }
  }

  String _getFullAddress(Address address) {
    final parts = [
      if (address.apartment.isNotEmpty) 'Apt ${address.apartment}',
      if (address.floor.isNotEmpty) 'Floor ${address.floor}',
      address.building,
      address.street,
      address.area,
      address.city,
    ];
    return parts.where((p) => p.isNotEmpty).join(', ');
  }

  Future<void> _addNewAddress(BuildContext context, bool isArabic) async {
    final result = await Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => const AddEditAddressScreen(),
      ),
    );

    if (result == true) {
      _loadAddresses();
    }
  }

  Future<void> _editAddress(BuildContext context, Address address, bool isArabic) async {
    final result = await Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => AddEditAddressScreen(address: address),
      ),
    );

    if (result == true) {
      _loadAddresses();
    }
  }

  Future<void> _setAsDefault(int id, AddressProvider provider, bool isArabic) async {
    final success = await provider.setDefaultAddress(id);
    if (success && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(isArabic ? 'تم تعيين العنوان كافتراضي' : 'Address set as default'),
          backgroundColor: Colors.green,
        ),
      );
    }
  }

  Future<void> _deleteAddress(int id, AddressProvider provider, bool isArabic) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(isArabic ? 'حذف العنوان' : 'Delete Address'),
        content: Text(
          isArabic
              ? 'هل أنت متأكد من حذف هذا العنوان؟'
              : 'Are you sure you want to delete this address?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: Text(isArabic ? 'إلغاء' : 'Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: Text(isArabic ? 'حذف' : 'Delete'),
          ),
        ],
      ),
    );

    if (confirmed == true) {
      final success = await provider.deleteAddress(id);
      if (success && mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(isArabic ? 'تم حذف العنوان' : 'Address deleted'),
            backgroundColor: Colors.green,
          ),
        );
      }
    }
  }
}
