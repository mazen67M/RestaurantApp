import 'package:flutter/material.dart';

/// Animation and transition constants
class AppAnimations {
  // Duration
  static const Duration durationFast = Duration(milliseconds: 150);
  static const Duration durationBase = Duration(milliseconds: 250);
  static const Duration durationSlow = Duration(milliseconds: 350);
  static const Duration durationVerySlow = Duration(milliseconds: 500);
  
  // Curves
  static const Curve curveDefault = Curves.easeInOut;
  static const Curve curveEaseIn = Curves.easeIn;
  static const Curve curveEaseOut = Curves.easeOut;
  static const Curve curveBounce = Curves.bounceOut;
  static const Curve curveElastic = Curves.elasticOut;
  
  // Page Transitions
  static const Duration pageTransitionDuration = Duration(milliseconds: 300);
  
  // List Item Stagger
  static const Duration staggerDelay = Duration(milliseconds: 50);
  
  // Shimmer
  static const Duration shimmerDuration = Duration(milliseconds: 1500);
}
