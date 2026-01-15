import 'package:flutter/material.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/theme/app_dimensions.dart';
import '../../../core/theme/app_animations.dart';

enum CustomButtonType { primary, secondary, text, icon }
enum CustomButtonSize { small, medium, large }

class CustomButton extends StatefulWidget {
  final String? text;
  final IconData? icon;
  final VoidCallback? onPressed;
  final CustomButtonType type;
  final CustomButtonSize size;
  final bool isLoading;
  final bool isFullWidth;
  final Widget? child;

  const CustomButton({
    super.key,
    this.text,
    this.icon,
    required this.onPressed,
    this.type = CustomButtonType.primary,
    this.size = CustomButtonSize.medium,
    this.isLoading = false,
    this.isFullWidth = false,
    this.child,
  });

  const CustomButton.primary({
    Key? key,
    required String text,
    required VoidCallback? onPressed,
    CustomButtonSize size = CustomButtonSize.medium,
    bool isLoading = false,
    bool isFullWidth = false,
    IconData? icon,
  }) : this(
          key: key,
          text: text,
          icon: icon,
          onPressed: onPressed,
          type: CustomButtonType.primary,
          size: size,
          isLoading: isLoading,
          isFullWidth: isFullWidth,
        );

  const CustomButton.secondary({
    Key? key,
    required String text,
    required VoidCallback? onPressed,
    CustomButtonSize size = CustomButtonSize.medium,
    bool isLoading = false,
    bool isFullWidth = false,
    IconData? icon,
  }) : this(
          key: key,
          text: text,
          icon: icon,
          onPressed: onPressed,
          type: CustomButtonType.secondary,
          size: size,
          isLoading: isLoading,
          isFullWidth: isFullWidth,
        );

  const CustomButton.text({
    Key? key,
    required String text,
    required VoidCallback? onPressed,
    CustomButtonSize size = CustomButtonSize.medium,
    IconData? icon,
  }) : this(
          key: key,
          text: text,
          icon: icon,
          onPressed: onPressed,
          type: CustomButtonType.text,
          size: size,
          isLoading: false,
          isFullWidth: false,
        );

  const CustomButton.icon({
    Key? key,
    required IconData icon,
    required VoidCallback? onPressed,
    CustomButtonSize size = CustomButtonSize.medium,
  }) : this(
          key: key,
          icon: icon,
          onPressed: onPressed,
          type: CustomButtonType.icon,
          size: size,
          isLoading: false,
          isFullWidth: false,
        );

  @override
  State<CustomButton> createState() => _CustomButtonState();
}

class _CustomButtonState extends State<CustomButton>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _scaleAnimation;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      duration: AppAnimations.durationFast,
      vsync: this,
    );
    _scaleAnimation = Tween<double>(begin: 1.0, end: 0.95).animate(
      CurvedAnimation(parent: _controller, curve: AppAnimations.curveDefault),
    );
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  double get _height {
    switch (widget.size) {
      case CustomButtonSize.small:
        return AppDimensions.buttonHeightSm;
      case CustomButtonSize.medium:
        return AppDimensions.buttonHeightMd;
      case CustomButtonSize.large:
        return AppDimensions.buttonHeightLg;
    }
  }

  EdgeInsets get _padding {
    switch (widget.size) {
      case CustomButtonSize.small:
        return EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingMd,
          vertical: AppDimensions.paddingSm,
        );
      case CustomButtonSize.medium:
        return EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingLg,
          vertical: AppDimensions.paddingMd,
        );
      case CustomButtonSize.large:
        return EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingXl,
          vertical: AppDimensions.paddingLg,
        );
    }
  }

  Widget _buildButtonContent() {
    if (widget.isLoading) {
      return SizedBox(
        width: 20,
        height: 20,
        child: CircularProgressIndicator(
          strokeWidth: 2,
          valueColor: AlwaysStoppedAnimation<Color>(
            widget.type == CustomButtonType.primary
                ? AppColors.textWhite
                : AppColors.primaryOrange,
          ),
        ),
      );
    }

    if (widget.child != null) {
      return widget.child!;
    }

    if (widget.type == CustomButtonType.icon && widget.icon != null) {
      return Icon(widget.icon, size: AppDimensions.iconMd);
    }

    final List<Widget> children = [];
    
    if (widget.icon != null) {
      children.add(Icon(widget.icon, size: AppDimensions.iconSm));
      children.add(const SizedBox(width: AppDimensions.spaceSm));
    }
    
    if (widget.text != null) {
      children.add(
        Text(
          widget.text!,
          style: AppTypography.button,
        ),
      );
    }

    return Row(
      mainAxisSize: MainAxisSize.min,
      mainAxisAlignment: MainAxisAlignment.center,
      children: children,
    );
  }

  @override
  Widget build(BuildContext context) {
    Widget button;

    switch (widget.type) {
      case CustomButtonType.primary:
        button = ElevatedButton(
          onPressed: widget.isLoading ? null : widget.onPressed,
          style: ElevatedButton.styleFrom(
            backgroundColor: AppColors.primaryOrange,
            foregroundColor: AppColors.textWhite,
            padding: _padding,
            minimumSize: Size(widget.isFullWidth ? double.infinity : 0, _height),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(AppDimensions.radiusMd),
            ),
            elevation: AppDimensions.cardElevation,
          ),
          child: _buildButtonContent(),
        );
        break;

      case CustomButtonType.secondary:
        button = OutlinedButton(
          onPressed: widget.isLoading ? null : widget.onPressed,
          style: OutlinedButton.styleFrom(
            foregroundColor: AppColors.primaryOrange,
            side: BorderSide(color: AppColors.primaryOrange, width: 1.5),
            padding: _padding,
            minimumSize: Size(widget.isFullWidth ? double.infinity : 0, _height),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(AppDimensions.radiusMd),
            ),
          ),
          child: _buildButtonContent(),
        );
        break;

      case CustomButtonType.text:
        button = TextButton(
          onPressed: widget.onPressed,
          style: TextButton.styleFrom(
            foregroundColor: AppColors.primaryOrange,
            padding: _padding,
          ),
          child: _buildButtonContent(),
        );
        break;

      case CustomButtonType.icon:
        button = IconButton(
          onPressed: widget.onPressed,
          icon: _buildButtonContent(),
          color: AppColors.primaryOrange,
          iconSize: AppDimensions.iconMd,
        );
        break;
    }

    if (widget.type == CustomButtonType.icon ||
        widget.type == CustomButtonType.text) {
      return button;
    }

    return GestureDetector(
      onTapDown: (_) => _controller.forward(),
      onTapUp: (_) => _controller.reverse(),
      onTapCancel: () => _controller.reverse(),
      child: ScaleTransition(
        scale: _scaleAnimation,
        child: button,
      ),
    );
  }
}
