import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../data/models/address.dart';
import '../../../data/providers/address_provider.dart';
import '../../../data/providers/locale_provider.dart';
import '../../../core/theme/app_theme.dart';

class AddEditAddressScreen extends StatefulWidget {
  final Address? address;

  const AddEditAddressScreen({
    super.key,
    this.address,
  });

  @override
  State<AddEditAddressScreen> createState() => _AddEditAddressScreenState();
}

class _AddEditAddressScreenState extends State<AddEditAddressScreen> {
  final _formKey = GlobalKey<FormState>();
  late TextEditingController _labelController;
  late TextEditingController _streetController;
  late TextEditingController _buildingController;
  late TextEditingController _floorController;
  late TextEditingController _apartmentController;
  late TextEditingController _cityController;
  late TextEditingController _areaController;
  late TextEditingController _directionsController;
  
  bool _isDefault = false;
  bool _isSaving = false;

  @override
  void initState() {
    super.initState();
    _labelController = TextEditingController(text: widget.address?.label ?? '');
    _streetController = TextEditingController(text: widget.address?.street ?? '');
    _buildingController = TextEditingController(text: widget.address?.building ?? '');
    _floorController = TextEditingController(text: widget.address?.floor ?? '');
    _apartmentController = TextEditingController(text: widget.address?.apartment ?? '');
    _cityController = TextEditingController(text: widget.address?.city ?? '');
    _areaController = TextEditingController(text: widget.address?.area ?? '');
    _directionsController = TextEditingController(text: widget.address?.additionalDirections ?? '');
    _isDefault = widget.address?.isDefault ?? false;
  }

  @override
  void dispose() {
    _labelController.dispose();
    _streetController.dispose();
    _buildingController.dispose();
    _floorController.dispose();
    _apartmentController.dispose();
    _cityController.dispose();
    _areaController.dispose();
    _directionsController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final isArabic = context.watch<LocaleProvider>().isArabic;
    final isEditing = widget.address != null;

    return Scaffold(
      appBar: AppBar(
        title: Text(
          isEditing
              ? (isArabic ? 'تعديل العنوان' : 'Edit Address')
              : (isArabic ? 'إضافة عنوان' : 'Add Address'),
        ),
        backgroundColor: AppTheme.primaryColor,
        foregroundColor: Colors.white,
      ),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            // Label
            _buildTextField(
              controller: _labelController,
              label: isArabic ? 'التسمية' : 'Label',
              hint: isArabic ? 'مثال: المنزل، العمل' : 'e.g., Home, Work',
              icon: Icons.label_outline,
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return isArabic ? 'الرجاء إدخال التسمية' : 'Please enter label';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),

            // Quick label buttons
            Wrap(
              spacing: 8,
              children: [
                _buildQuickLabelButton(isArabic ? 'المنزل' : 'Home', Icons.home),
                _buildQuickLabelButton(isArabic ? 'العمل' : 'Work', Icons.work),
                _buildQuickLabelButton(isArabic ? 'آخر' : 'Other', Icons.location_on),
              ],
            ),
            const SizedBox(height: 24),

            // City
            _buildTextField(
              controller: _cityController,
              label: isArabic ? 'المدينة' : 'City',
              hint: isArabic ? 'مثال: دبي' : 'e.g., Dubai',
              icon: Icons.location_city,
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return isArabic ? 'الرجاء إدخال المدينة' : 'Please enter city';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),

            // Area
            _buildTextField(
              controller: _areaController,
              label: isArabic ? 'المنطقة' : 'Area',
              hint: isArabic ? 'مثال: الخليج التجاري' : 'e.g., Business Bay',
              icon: Icons.map,
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return isArabic ? 'الرجاء إدخال المنطقة' : 'Please enter area';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),

            // Street
            _buildTextField(
              controller: _streetController,
              label: isArabic ? 'الشارع' : 'Street',
              hint: isArabic ? 'اسم الشارع' : 'Street name',
              icon: Icons.route,
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return isArabic ? 'الرجاء إدخال الشارع' : 'Please enter street';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),

            // Building
            _buildTextField(
              controller: _buildingController,
              label: isArabic ? 'المبنى' : 'Building',
              hint: isArabic ? 'اسم أو رقم المبنى' : 'Building name or number',
              icon: Icons.apartment,
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return isArabic ? 'الرجاء إدخال المبنى' : 'Please enter building';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),

            // Floor and Apartment in a row
            Row(
              children: [
                Expanded(
                  child: _buildTextField(
                    controller: _floorController,
                    label: isArabic ? 'الطابق' : 'Floor',
                    hint: isArabic ? 'رقم الطابق' : 'Floor number',
                    icon: Icons.stairs,
                    keyboardType: TextInputType.number,
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: _buildTextField(
                    controller: _apartmentController,
                    label: isArabic ? 'الشقة' : 'Apartment',
                    hint: isArabic ? 'رقم الشقة' : 'Apt number',
                    icon: Icons.door_front_door,
                    keyboardType: TextInputType.number,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),

            // Additional Directions
            _buildTextField(
              controller: _directionsController,
              label: isArabic ? 'إرشادات إضافية' : 'Additional Directions',
              hint: isArabic
                  ? 'مثال: بجانب المسجد، المدخل الخلفي'
                  : 'e.g., Next to mosque, Back entrance',
              icon: Icons.info_outline,
              maxLines: 3,
            ),
            const SizedBox(height: 16),

            // Set as default checkbox
            CheckboxListTile(
              value: _isDefault,
              onChanged: (value) {
                setState(() {
                  _isDefault = value ?? false;
                });
              },
              title: Text(isArabic ? 'تعيين كعنوان افتراضي' : 'Set as default address'),
              activeColor: AppTheme.primaryColor,
              contentPadding: EdgeInsets.zero,
            ),
            const SizedBox(height: 24),

            // Save button
            SizedBox(
              height: 50,
              child: ElevatedButton(
                onPressed: _isSaving ? null : _saveAddress,
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppTheme.primaryColor,
                  foregroundColor: Colors.white,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
                child: _isSaving
                    ? const CircularProgressIndicator(color: Colors.white)
                    : Text(
                        isArabic ? 'حفظ العنوان' : 'Save Address',
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildTextField({
    required TextEditingController controller,
    required String label,
    required String hint,
    required IconData icon,
    String? Function(String?)? validator,
    TextInputType? keyboardType,
    int maxLines = 1,
  }) {
    return TextFormField(
      controller: controller,
      decoration: InputDecoration(
        labelText: label,
        hintText: hint,
        prefixIcon: Icon(icon),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: BorderSide(color: Colors.grey.shade300),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: AppTheme.primaryColor, width: 2),
        ),
      ),
      validator: validator,
      keyboardType: keyboardType,
      maxLines: maxLines,
    );
  }

  Widget _buildQuickLabelButton(String label, IconData icon) {
    final isSelected = _labelController.text == label;
    
    return FilterChip(
      label: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 16),
          const SizedBox(width: 4),
          Text(label),
        ],
      ),
      selected: isSelected,
      onSelected: (selected) {
        setState(() {
          _labelController.text = label;
        });
      },
      selectedColor: AppTheme.primaryColor.withOpacity(0.2),
      checkmarkColor: AppTheme.primaryColor,
    );
  }

  Future<void> _saveAddress() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    setState(() {
      _isSaving = true;
    });

    final addressProvider = context.read<AddressProvider>();
    final isArabic = context.read<LocaleProvider>().isArabic;

    final address = Address(
      id: widget.address?.id ?? 0,
      label: _labelController.text,
      street: _streetController.text,
      building: _buildingController.text,
      floor: _floorController.text,
      apartment: _apartmentController.text,
      city: _cityController.text,
      area: _areaController.text,
      latitude: widget.address?.latitude ?? 25.2048, // Default Dubai coordinates
      longitude: widget.address?.longitude ?? 55.2708,
      isDefault: _isDefault,
      additionalDirections: _directionsController.text.isEmpty 
          ? null 
          : _directionsController.text,
    );

    bool success;
    if (widget.address != null) {
      success = await addressProvider.updateAddress(widget.address!.id, address);
    } else {
      success = await addressProvider.addAddress(address);
    }

    setState(() {
      _isSaving = false;
    });

    if (success && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            widget.address != null
                ? (isArabic ? 'تم تحديث العنوان' : 'Address updated')
                : (isArabic ? 'تم إضافة العنوان' : 'Address added'),
          ),
          backgroundColor: Colors.green,
        ),
      );
      Navigator.pop(context, true);
    } else if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            isArabic ? 'حدث خطأ، الرجاء المحاولة مرة أخرى' : 'Error occurred, please try again',
          ),
          backgroundColor: Colors.red,
        ),
      );
    }
  }
}
