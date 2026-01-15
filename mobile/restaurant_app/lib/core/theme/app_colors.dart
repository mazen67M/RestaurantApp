import 'package:flutter/material.dart';

/// App color palette based on the reference designs
class AppColors {
  // Primary Colors - Orange theme
  static const Color primaryOrange = Color(0xFFFF6B35);
  static const Color primaryOrangeLight = Color(0xFFFF8C5A);
  static const Color primaryOrangeDark = Color(0xFFE85A2A);
  
  // Secondary Colors
  static const Color secondaryBlack = Color(0xFF1A1A1A);
  static const Color secondaryGray = Color(0xFFF5F5F5);
  static const Color secondaryWhite = Color(0xFFFFFFFF);
  
  // Accent Colors
  static const Color accentGreen = Color(0xFF4CAF50);
  static const Color accentRed = Color(0xFFF44336);
  static const Color accentYellow = Color(0xFFFFC107);
  
  // Text Colors
  static const Color textPrimary = Color(0xFF2D2D2D);
  static const Color textSecondary = Color(0xFF757575);
  static const Color textLight = Color(0xFF9E9E9E);
  static const Color textWhite = Color(0xFFFFFFFF);
  
  // Background Colors
  static const Color bgPrimary = Color(0xFFFFFFFF);
  static const Color bgSecondary = Color(0xFFF9F9F9);
  static const Color bgCard = Color(0xFFFFFFFF);
  static const Color bgOverlay = Color(0x80000000);
  
  // Border & Divider
  static const Color borderColor = Color(0xFFE0E0E0);
  static const Color dividerColor = Color(0xFFEEEEEE);
  
  // Status Colors
  static const Color statusPending = Color(0xFFFFC107);
  static const Color statusConfirmed = Color(0xFF2196F3);
  static const Color statusPreparing = Color(0xFFFF9800);
  static const Color statusReady = Color(0xFF4CAF50);
  static const Color statusDelivered = Color(0xFF4CAF50);
  static const Color statusCancelled = Color(0xFFF44336);
  
  // Gradient Colors
  static const LinearGradient primaryGradient = LinearGradient(
    colors: [primaryOrange, primaryOrangeLight],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
  );
  
  static const LinearGradient successGradient = LinearGradient(
    colors: [Color(0xFF4CAF50), Color(0xFF66BB6A)],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
  );
  
  // Shadow Colors
  static const Color shadowLight = Color(0x14000000);
  static const Color shadowMedium = Color(0x1F000000);
  static const Color shadowDark = Color(0x29000000);
}
