import 'package:flutter/material.dart';

/// Typography system for the app
class AppTypography {
  // Font Family
  static const String fontFamily = 'Inter';
  
  // Font Sizes
  static const double fontXs = 12.0;
  static const double fontSm = 14.0;
  static const double fontBase = 16.0;
  static const double fontLg = 18.0;
  static const double fontXl = 20.0;
  static const double font2xl = 24.0;
  static const double font3xl = 32.0;
  
  // Font Weights
  static const FontWeight fontRegular = FontWeight.w400;
  static const FontWeight fontMedium = FontWeight.w500;
  static const FontWeight fontSemiBold = FontWeight.w600;
  static const FontWeight fontBold = FontWeight.w700;
  
  // Text Styles - Headings
  static const TextStyle h1 = TextStyle(
    fontSize: font3xl,
    fontWeight: fontBold,
    height: 1.2,
    letterSpacing: -0.5,
  );
  
  static const TextStyle h2 = TextStyle(
    fontSize: font2xl,
    fontWeight: fontBold,
    height: 1.3,
    letterSpacing: -0.3,
  );
  
  static const TextStyle h3 = TextStyle(
    fontSize: fontXl,
    fontWeight: fontSemiBold,
    height: 1.4,
  );
  
  static const TextStyle h4 = TextStyle(
    fontSize: fontLg,
    fontWeight: fontSemiBold,
    height: 1.4,
  );
  
  // Text Styles - Body
  static const TextStyle bodyLarge = TextStyle(
    fontSize: fontBase,
    fontWeight: fontRegular,
    height: 1.5,
  );
  
  static const TextStyle bodyMedium = TextStyle(
    fontSize: fontSm,
    fontWeight: fontRegular,
    height: 1.5,
  );
  
  static const TextStyle bodySmall = TextStyle(
    fontSize: fontXs,
    fontWeight: fontRegular,
    height: 1.5,
  );
  
  // Text Styles - Special
  static const TextStyle button = TextStyle(
    fontSize: fontBase,
    fontWeight: fontSemiBold,
    height: 1.2,
    letterSpacing: 0.5,
  );
  
  static const TextStyle caption = TextStyle(
    fontSize: fontXs,
    fontWeight: fontRegular,
    height: 1.3,
  );
  
  static const TextStyle overline = TextStyle(
    fontSize: fontXs,
    fontWeight: fontMedium,
    height: 1.3,
    letterSpacing: 1.0,
  );
  
  static const TextStyle price = TextStyle(
    fontSize: fontLg,
    fontWeight: fontBold,
    height: 1.2,
  );
  
  static const TextStyle priceSmall = TextStyle(
    fontSize: fontBase,
    fontWeight: fontBold,
    height: 1.2,
  );
}
