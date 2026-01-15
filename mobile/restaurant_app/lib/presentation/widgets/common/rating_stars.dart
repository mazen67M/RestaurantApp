import 'package:flutter/material.dart';
import 'package:flutter_rating_bar/flutter_rating_bar.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/theme/app_typography.dart';
import '../../../core/theme/app_dimensions.dart';

class RatingStars extends StatelessWidget {
  final double rating;
  final int? reviewCount;
  final double size;
  final bool showRatingText;
  final bool allowHalfRating;
  final Color? color;

  const RatingStars({
    super.key,
    required this.rating,
    this.reviewCount,
    this.size = 16.0,
    this.showRatingText = true,
    this.allowHalfRating = true,
    this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        RatingBarIndicator(
          rating: rating,
          itemBuilder: (context, index) => Icon(
            Icons.star,
            color: color ?? AppColors.accentYellow,
          ),
          itemCount: 5,
          itemSize: size,
          unratedColor: AppColors.borderColor,
        ),
        if (showRatingText) ...[
          const SizedBox(width: AppDimensions.spaceXs),
          Text(
            rating.toStringAsFixed(1),
            style: AppTypography.bodySmall.copyWith(
              color: AppColors.textSecondary,
              fontWeight: AppTypography.fontMedium,
            ),
          ),
        ],
        if (reviewCount != null) ...[
          const SizedBox(width: AppDimensions.spaceXs),
          Text(
            '($reviewCount)',
            style: AppTypography.bodySmall.copyWith(
              color: AppColors.textLight,
            ),
          ),
        ],
      ],
    );
  }
}

class RatingInput extends StatelessWidget {
  final double initialRating;
  final ValueChanged<double> onRatingUpdate;
  final double size;

  const RatingInput({
    super.key,
    this.initialRating = 0,
    required this.onRatingUpdate,
    this.size = 32.0,
  });

  @override
  Widget build(BuildContext context) {
    return RatingBar.builder(
      initialRating: initialRating,
      minRating: 1,
      direction: Axis.horizontal,
      allowHalfRating: true,
      itemCount: 5,
      itemSize: size,
      itemPadding: const EdgeInsets.symmetric(horizontal: 4.0),
      itemBuilder: (context, _) => const Icon(
        Icons.star,
        color: AppColors.accentYellow,
      ),
      onRatingUpdate: onRatingUpdate,
      unratedColor: AppColors.borderColor,
    );
  }
}
