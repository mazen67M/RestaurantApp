import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/theme/app_theme.dart';
import '../../../core/localization/app_localizations.dart';
import '../../../data/providers/auth_provider.dart';
import '../../../data/providers/locale_provider.dart';
import '../auth/login_screen.dart';
import '../addresses/addresses_screen.dart';

class ProfileScreen extends StatelessWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final authProvider = context.watch<AuthProvider>();
    final localeProvider = context.watch<LocaleProvider>();
    final isArabic = localeProvider.isArabic;

    return Scaffold(
      appBar: AppBar(
        title: Text(context.tr('profile')),
        automaticallyImplyLeading: false,
      ),
      body: authProvider.isAuthenticated
          ? _buildAuthenticatedContent(context, authProvider, localeProvider, isArabic)
          : _buildGuestContent(context),
    );
  }

  Widget _buildGuestContent(BuildContext context) {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.person_outline,
            size: 80,
            color: Colors.grey[400],
          ),
          const SizedBox(height: 16),
          Text(
            context.tr('my_profile'),
            style: const TextStyle(
              fontSize: 20,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            'Login to access your profile',
            style: TextStyle(color: AppTheme.textSecondary),
          ),
          const SizedBox(height: 24),
          ElevatedButton(
            onPressed: () {
              Navigator.of(context).push(
                MaterialPageRoute(builder: (_) => const LoginScreen()),
              );
            },
            child: Text(context.tr('login')),
          ),
        ],
      ),
    );
  }

  Widget _buildAuthenticatedContent(
    BuildContext context,
    AuthProvider authProvider,
    LocaleProvider localeProvider,
    bool isArabic,
  ) {
    final user = authProvider.user;

    return SingleChildScrollView(
      child: Column(
        children: [
          // Profile header
          Container(
            padding: const EdgeInsets.all(24),
            child: Column(
              children: [
                CircleAvatar(
                  radius: 50,
                  backgroundColor: AppTheme.primaryColor.withOpacity(0.1),
                  backgroundImage: user?.profileImageUrl != null
                      ? NetworkImage(user!.profileImageUrl!)
                      : null,
                  child: user?.profileImageUrl == null
                      ? Text(
                          user?.fullName.substring(0, 1).toUpperCase() ?? 'U',
                          style: const TextStyle(
                            fontSize: 36,
                            fontWeight: FontWeight.bold,
                            color: AppTheme.primaryColor,
                          ),
                        )
                      : null,
                ),
                const SizedBox(height: 16),
                Text(
                  user?.fullName ?? '',
                  style: const TextStyle(
                    fontSize: 22,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  user?.email ?? '',
                  style: TextStyle(
                    color: AppTheme.textSecondary,
                  ),
                ),
              ],
            ),
          ),

          const Divider(height: 1),

          // Menu items
          _buildMenuItem(
            context,
            icon: Icons.person_outline,
            title: context.tr('edit_profile'),
            onTap: () {
              // TODO: Navigate to edit profile
            },
          ),

          _buildMenuItem(
            context,
            icon: Icons.location_on_outlined,
            title: context.tr('my_addresses'),
            onTap: () {
              Navigator.of(context).push(
                MaterialPageRoute(builder: (_) => const AddressesScreen()),
              );
            },
          ),

          _buildMenuItem(
            context,
            icon: Icons.language,
            title: context.tr('language'),
            trailing: Text(
              isArabic ? 'العربية' : 'English',
              style: TextStyle(color: AppTheme.textSecondary),
            ),
            onTap: () => localeProvider.toggleLocale(),
          ),

          _buildMenuItem(
            context,
            icon: Icons.notifications_outlined,
            title: context.tr('notifications'),
            onTap: () {
              // TODO: Navigate to notifications settings
            },
          ),

          const Divider(height: 1),

          _buildMenuItem(
            context,
            icon: Icons.help_outline,
            title: context.tr('help_support'),
            onTap: () {
              // TODO: Navigate to help
            },
          ),

          _buildMenuItem(
            context,
            icon: Icons.info_outline,
            title: context.tr('about_us'),
            onTap: () {
              // TODO: Navigate to about
            },
          ),

          const Divider(height: 1),

          _buildMenuItem(
            context,
            icon: Icons.logout,
            title: context.tr('logout'),
            iconColor: AppTheme.errorColor,
            titleColor: AppTheme.errorColor,
            onTap: () async {
              final confirmed = await showDialog<bool>(
                context: context,
                builder: (ctx) => AlertDialog(
                  title: Text(context.tr('logout')),
                  content: const Text('Are you sure you want to logout?'),
                  actions: [
                    TextButton(
                      onPressed: () => Navigator.of(ctx).pop(false),
                      child: Text(context.tr('cancel')),
                    ),
                    TextButton(
                      onPressed: () => Navigator.of(ctx).pop(true),
                      child: Text(
                        context.tr('logout'),
                        style: const TextStyle(color: AppTheme.errorColor),
                      ),
                    ),
                  ],
                ),
              );

              if (confirmed == true) {
                await authProvider.logout();
              }
            },
          ),

          const SizedBox(height: 24),

          // App version
          Text(
            'Version 1.0.0',
            style: TextStyle(
              fontSize: 12,
              color: AppTheme.textSecondary,
            ),
          ),

          const SizedBox(height: 24),
        ],
      ),
    );
  }

  Widget _buildMenuItem(
    BuildContext context, {
    required IconData icon,
    required String title,
    Widget? trailing,
    Color? iconColor,
    Color? titleColor,
    required VoidCallback onTap,
  }) {
    return ListTile(
      leading: Icon(icon, color: iconColor ?? AppTheme.textSecondary),
      title: Text(
        title,
        style: TextStyle(color: titleColor),
      ),
      trailing: trailing ?? const Icon(Icons.chevron_right),
      onTap: onTap,
    );
  }
}
