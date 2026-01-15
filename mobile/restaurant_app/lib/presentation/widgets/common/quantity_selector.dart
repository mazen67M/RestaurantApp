import 'package:flutter/material.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/theme/app_dimensions.dart';
import '../../../core/theme/app_animations.dart';

class QuantitySelector extends StatefulWidget {
  final int quantity;
  final ValueChanged<int> onQuantityChanged;
  final int minQuantity;
  final int maxQuantity;
  final QuantitySelectorSize size;

  const QuantitySelector({
    super.key,
    required this.quantity,
    required this.onQuantityChanged,
    this.minQuantity = 1,
    this.maxQuantity = 99,
    this.size = QuantitySelectorSize.medium,
  });

  @override
  State<QuantitySelector> createState() => _QuantitySelectorState();
}

enum QuantitySelectorSize { small, medium, large }

class _QuantitySelectorState extends State<QuantitySelector>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _scaleAnimation;
  int _displayQuantity = 0;

  @override
  void initState() {
    super.initState();
    _displayQuantity = widget.quantity;
    _animationController = AnimationController(
      duration: AppAnimations.durationFast,
      vsync: this,
    );
    _scaleAnimation = Tween<double>(begin: 1.0, end: 1.2).animate(
      CurvedAnimation(
        parent: _animationController,
        curve: AppAnimations.curveBounce,
      ),
    );
  }

  @override
  void didUpdateWidget(QuantitySelector oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.quantity != widget.quantity) {
      setState(() {
        _displayQuantity = widget.quantity;
      });
      _animateQuantityChange();
    }
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  void _animateQuantityChange() {
    _animationController.forward().then((_) {
      _animationController.reverse();
    });
  }

  void _increment() {
    if (widget.quantity < widget.maxQuantity) {
      widget.onQuantityChanged(widget.quantity + 1);
    }
  }

  void _decrement() {
    if (widget.quantity > widget.minQuantity) {
      widget.onQuantityChanged(widget.quantity - 1);
    }
  }

  double get _buttonSize {
    switch (widget.size) {
      case QuantitySelectorSize.small:
        return 28.0;
      case QuantitySelectorSize.medium:
        return 36.0;
      case QuantitySelectorSize.large:
        return 44.0;
    }
  }

  double get _fontSize {
    switch (widget.size) {
      case QuantitySelectorSize.small:
        return AppTypography.fontSm;
      case QuantitySelectorSize.medium:
        return AppTypography.fontBase;
      case QuantitySelectorSize.large:
        return AppTypography.fontLg;
    }
  }

  @override
  Widget build(BuildContext context) {
    final bool canDecrement = widget.quantity > widget.minQuantity;
    final bool canIncrement = widget.quantity < widget.maxQuantity;

    return Container(
      decoration: BoxDecoration(
        color: AppColors.bgSecondary,
        borderRadius: BorderRadius.circular(AppDimensions.radiusFull),
        border: Border.all(color: AppColors.borderColor),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          _buildButton(
            icon: Icons.remove,
            onPressed: canDecrement ? _decrement : null,
            enabled: canDecrement,
          ),
          ScaleTransition(
            scale: _scaleAnimation,
            child: Container(
              constraints: BoxConstraints(minWidth: _buttonSize),
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingSm,
              ),
              child: Text(
                _displayQuantity.toString(),
                textAlign: TextAlign.center,
                style: AppTypography.button.copyWith(
                  fontSize: _fontSize,
                  color: AppColors.textPrimary,
                ),
              ),
            ),
          ),
          _buildButton(
            icon: Icons.add,
            onPressed: canIncrement ? _increment : null,
            enabled: canIncrement,
          ),
        ],
      ),
    );
  }

  Widget _buildButton({
    required IconData icon,
    required VoidCallback? onPressed,
    required bool enabled,
  }) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onPressed,
        borderRadius: BorderRadius.circular(AppDimensions.radiusFull),
        child: Container(
          width: _buttonSize,
          height: _buttonSize,
          decoration: BoxDecoration(
            color: enabled ? AppColors.primaryOrange : AppColors.borderColor,
            shape: BoxShape.circle,
          ),
          child: Icon(
            icon,
            size: _buttonSize * 0.5,
            color: enabled ? AppColors.textWhite : AppColors.textLight,
          ),
        ),
      ),
    );
  }
}
