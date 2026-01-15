import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/theme/app_theme.dart';
import '../../../core/localization/app_localizations.dart';
import '../../../data/models/address_model.dart';
import '../../../data/providers/locale_provider.dart';
import '../../../data/services/api_service.dart';
import '../../../core/constants/constants.dart';
import 'select_location_screen.dart';

class AddAddressScreen extends StatefulWidget {
  final UserAddress? address;
  
  const AddAddressScreen({
    super.key,
    this.address,
  });

  @override
  State<AddAddressScreen> createState() => _AddAddressScreenState();
}

class _AddAddressScreenState extends State<AddAddressScreen> {
  final ApiService _apiService = ApiService();
  final _formKey = GlobalKey<FormState>();
  
  final _labelController = TextEditingController();
  final _addressLineController = TextEditingController();
  final _buildingController = TextEditingController();
  final _floorController = TextEditingController();
  final _apartmentController = TextEditingController();
  final _landmarkController = TextEditingController();
  
  double? _latitude;
  double? _longitude;
  bool _isDefault = false;
  bool _isLoading = false;
  String? _selectedLabel;
  
  final List<Map<String, dynamic>> _predefinedLabels = [
    {'key': 'address_home', 'icon': Icons.home_outlined},
    {'key': 'address_work', 'icon': Icons.work_outline},
    {'key': 'address_other', 'icon': Icons.location_on_outlined},
  ];

  bool get isEditing => widget.address != null;

  @override
  void initState() {
    super.initState();
    if (widget.address != null) {
      _labelController.text = widget.address!.label;
      _addressLineController.text = widget.address!.addressLine;
      _buildingController.text = widget.address!.buildingName ?? '';
      _floorController.text = widget.address!.floor ?? '';
      _apartmentController.text = widget.address!.apartment ?? '';
      _landmarkController.text = widget.address!.landmark ?? '';
      _latitude = widget.address!.latitude;
      _longitude = widget.address!.longitude;
      _isDefault = widget.address!.isDefault;
      
      // Determine selected label
      final lowerLabel = widget.address!.label.toLowerCase();
      if (lowerLabel.contains('home') || lowerLabel.contains('منزل')) {
        _selectedLabel = 'address_home';
      } else if (lowerLabel.contains('work') || lowerLabel.contains('عمل')) {
        _selectedLabel = 'address_work';
      } else {
        _selectedLabel = 'address_other';
      }
    }
  }

  @override
  void dispose() {
    _labelController.dispose();
    _addressLineController.dispose();
    _buildingController.dispose();
    _floorController.dispose();
    _apartmentController.dispose();
    _landmarkController.dispose();
    super.dispose();
  }

  Future<void> _selectLocation() async {
    final result = await Navigator.of(context).push<Map<String, dynamic>>(
      MaterialPageRoute(
        builder: (_) => SelectLocationScreen(
          initialLatitude: _latitude,
          initialLongitude: _longitude,
        ),
      ),
    );
    
    if (result != null) {
      setState(() {
        _latitude = result['latitude'];
        _longitude = result['longitude'];
        if (result['address'] != null) {
          _addressLineController.text = result['address'];
        }
      });
    }
  }

  Future<void> _saveAddress() async {
    if (!_formKey.currentState!.validate()) return;
    
    if (_latitude == null || _longitude == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(context.tr('please_select_location')),
          backgroundColor: AppTheme.errorColor,
        ),
      );
      return;
    }

    setState(() => _isLoading = true);

    try {
      final body = {
        'label': _labelController.text.trim(),
        'addressLine': _addressLineController.text.trim(),
        'buildingName': _buildingController.text.trim().isNotEmpty 
            ? _buildingController.text.trim() 
            : null,
        'floor': _floorController.text.trim().isNotEmpty 
            ? _floorController.text.trim() 
            : null,
        'apartment': _apartmentController.text.trim().isNotEmpty 
            ? _apartmentController.text.trim() 
            : null,
        'landmark': _landmarkController.text.trim().isNotEmpty 
            ? _landmarkController.text.trim() 
            : null,
        'latitude': _latitude,
        'longitude': _longitude,
        'isDefault': _isDefault,
      };

      final response = isEditing
          ? await _apiService.put<void>(
              '${ApiConstants.addresses}/${widget.address!.id}',
              body: body,
            )
          : await _apiService.post<void>(
              ApiConstants.addresses,
              body: body,
            );

      if (response.success && mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              isEditing 
                  ? context.tr('address_updated') 
                  : context.tr('address_saved'),
            ),
            backgroundColor: AppTheme.successColor,
          ),
        );
        Navigator.of(context).pop(true);
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(response.message ?? 'Failed to save address'),
            backgroundColor: AppTheme.errorColor,
          ),
        );
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('An error occurred. Please try again.'),
          backgroundColor: AppTheme.errorColor,
        ),
      );
    }

    setState(() => _isLoading = false);
  }

  @override
  Widget build(BuildContext context) {
    final isArabic = context.watch<LocaleProvider>().isArabic;

    return Scaffold(
      appBar: AppBar(
        title: Text(isEditing 
            ? context.tr('edit_address') 
            : context.tr('add_new_address')),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Location picker
              _buildLocationPicker(),
              
              const SizedBox(height: 24),
              
              // Label selection
              Text(
                context.tr('address_label'),
                style: const TextStyle(
                  fontWeight: FontWeight.bold,
                  fontSize: 16,
                ),
              ),
              const SizedBox(height: 12),
              _buildLabelSelector(),
              
              const SizedBox(height: 16),
              
              // Custom label input
              TextFormField(
                controller: _labelController,
                decoration: InputDecoration(
                  labelText: context.tr('label_name'),
                  hintText: context.tr('label_hint'),
                  prefixIcon: const Icon(Icons.label_outline),
                ),
                validator: (value) {
                  if (value == null || value.trim().isEmpty) {
                    return context.tr('label_required');
                  }
                  return null;
                },
              ),
              
              const SizedBox(height: 16),
              
              // Address line
              TextFormField(
                controller: _addressLineController,
                decoration: InputDecoration(
                  labelText: context.tr('address_line'),
                  hintText: context.tr('address_line_hint'),
                  prefixIcon: const Icon(Icons.location_on_outlined),
                ),
                maxLines: 2,
                validator: (value) {
                  if (value == null || value.trim().isEmpty) {
                    return context.tr('address_required');
                  }
                  return null;
                },
              ),
              
              const SizedBox(height: 16),
              
              // Building name
              TextFormField(
                controller: _buildingController,
                decoration: InputDecoration(
                  labelText: context.tr('building'),
                  hintText: context.tr('building_hint'),
                  prefixIcon: const Icon(Icons.apartment_outlined),
                ),
              ),
              
              const SizedBox(height: 16),
              
              // Floor and Apartment
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _floorController,
                      decoration: InputDecoration(
                        labelText: context.tr('floor'),
                        prefixIcon: const Icon(Icons.stairs_outlined),
                      ),
                      keyboardType: TextInputType.text,
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: TextFormField(
                      controller: _apartmentController,
                      decoration: InputDecoration(
                        labelText: context.tr('apartment'),
                        prefixIcon: const Icon(Icons.door_front_door_outlined),
                      ),
                    ),
                  ),
                ],
              ),
              
              const SizedBox(height: 16),
              
              // Landmark
              TextFormField(
                controller: _landmarkController,
                decoration: InputDecoration(
                  labelText: context.tr('landmark'),
                  hintText: context.tr('landmark_hint'),
                  prefixIcon: const Icon(Icons.near_me_outlined),
                ),
              ),
              
              const SizedBox(height: 16),
              
              // Default address toggle
              SwitchListTile(
                title: Text(context.tr('set_as_default')),
                subtitle: Text(
                  context.tr('default_address_desc'),
                  style: TextStyle(
                    fontSize: 12,
                    color: AppTheme.textSecondary,
                  ),
                ),
                value: _isDefault,
                onChanged: (value) => setState(() => _isDefault = value),
                activeColor: AppTheme.primaryColor,
                contentPadding: EdgeInsets.zero,
              ),
              
              const SizedBox(height: 32),
              
              // Save button
              SizedBox(
                width: double.infinity,
                height: 50,
                child: ElevatedButton(
                  onPressed: _isLoading ? null : _saveAddress,
                  child: _isLoading
                      ? const SizedBox(
                          width: 24,
                          height: 24,
                          child: CircularProgressIndicator(
                            color: Colors.white,
                            strokeWidth: 2,
                          ),
                        )
                      : Text(
                          context.tr('save_address'),
                          style: const TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                ),
              ),
              
              const SizedBox(height: 32),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildLocationPicker() {
    final hasLocation = _latitude != null && _longitude != null;
    
    return InkWell(
      onTap: _selectLocation,
      borderRadius: BorderRadius.circular(12),
      child: Container(
        height: 180,
        decoration: BoxDecoration(
          color: Colors.grey[200],
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: hasLocation ? AppTheme.primaryColor : Colors.grey[300]!,
            width: hasLocation ? 2 : 1,
          ),
        ),
        child: hasLocation
            ? Stack(
                children: [
                  // Map placeholder with coordinates
                  Center(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Icon(
                          Icons.location_on,
                          size: 48,
                          color: AppTheme.primaryColor,
                        ),
                        const SizedBox(height: 8),
                        Text(
                          context.tr('location_selected'),
                          style: const TextStyle(
                            fontWeight: FontWeight.bold,
                            color: AppTheme.primaryColor,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          '${_latitude!.toStringAsFixed(4)}, ${_longitude!.toStringAsFixed(4)}',
                          style: TextStyle(
                            fontSize: 12,
                            color: AppTheme.textSecondary,
                          ),
                        ),
                      ],
                    ),
                  ),
                  // Edit button
                  Positioned(
                    top: 8,
                    right: 8,
                    child: Container(
                      padding: const EdgeInsets.all(8),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(8),
                        boxShadow: [
                          BoxShadow(
                            color: Colors.black.withOpacity(0.1),
                            blurRadius: 4,
                          ),
                        ],
                      ),
                      child: const Icon(Icons.edit, size: 20),
                    ),
                  ),
                ],
              )
            : Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(
                    Icons.add_location_alt_outlined,
                    size: 48,
                    color: Colors.grey[400],
                  ),
                  const SizedBox(height: 8),
                  Text(
                    context.tr('tap_to_select_location'),
                    style: TextStyle(
                      color: AppTheme.textSecondary,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                ],
              ),
      ),
    );
  }

  Widget _buildLabelSelector() {
    return Row(
      children: _predefinedLabels.map((label) {
        final isSelected = _selectedLabel == label['key'];
        return Expanded(
          child: Padding(
            padding: EdgeInsets.only(
              right: label != _predefinedLabels.last ? 8 : 0,
            ),
            child: InkWell(
              onTap: () {
                setState(() {
                  _selectedLabel = label['key'];
                  _labelController.text = context.tr(label['key']);
                });
              },
              borderRadius: BorderRadius.circular(12),
              child: Container(
                padding: const EdgeInsets.symmetric(vertical: 16),
                decoration: BoxDecoration(
                  color: isSelected 
                      ? AppTheme.primaryColor.withOpacity(0.1) 
                      : Colors.grey[100],
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(
                    color: isSelected 
                        ? AppTheme.primaryColor 
                        : Colors.grey[300]!,
                    width: isSelected ? 2 : 1,
                  ),
                ),
                child: Column(
                  children: [
                    Icon(
                      label['icon'] as IconData,
                      color: isSelected 
                          ? AppTheme.primaryColor 
                          : AppTheme.textSecondary,
                    ),
                    const SizedBox(height: 4),
                    Text(
                      context.tr(label['key']),
                      style: TextStyle(
                        fontSize: 12,
                        fontWeight: isSelected 
                            ? FontWeight.bold 
                            : FontWeight.normal,
                        color: isSelected 
                            ? AppTheme.primaryColor 
                            : AppTheme.textSecondary,
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        );
      }).toList(),
    );
  }
}
