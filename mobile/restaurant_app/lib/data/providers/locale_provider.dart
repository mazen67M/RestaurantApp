import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../core/constants/constants.dart';

class LocaleProvider extends ChangeNotifier {
  Locale _locale = const Locale('ar', 'AE');

  Locale get locale => _locale;
  bool get isArabic => _locale.languageCode == 'ar';

  LocaleProvider() {
    _loadLocale();
  }

  Future<void> _loadLocale() async {
    final prefs = await SharedPreferences.getInstance();
    final languageCode = prefs.getString(StorageKeys.language) ?? 'ar';
    _locale = Locale(languageCode, languageCode == 'ar' ? 'AE' : 'US');
    notifyListeners();
  }

  Future<void> setLocale(String languageCode) async {
    if (!AppConstants.supportedLanguages.contains(languageCode)) return;
    
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(StorageKeys.language, languageCode);
    
    _locale = Locale(languageCode, languageCode == 'ar' ? 'AE' : 'US');
    notifyListeners();
  }

  void toggleLocale() {
    setLocale(isArabic ? 'en' : 'ar');
  }
}
