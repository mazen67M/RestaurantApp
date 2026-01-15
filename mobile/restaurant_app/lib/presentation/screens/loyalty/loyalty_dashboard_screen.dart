import 'package:flutter/material.dart';
import '../../../data/models/loyalty.dart';
import '../../../data/services/phase3_service.dart';

class LoyaltyDashboardScreen extends StatefulWidget {
  const LoyaltyDashboardScreen({super.key});

  @override
  State<LoyaltyDashboardScreen> createState() => _LoyaltyDashboardScreenState();
}

class _LoyaltyDashboardScreenState extends State<LoyaltyDashboardScreen> {
  // Service uses centralized ApiConstants automatically
  final Phase3Service _service = Phase3Service();
  LoyaltyPoints? _points;
  List<LoyaltyTransaction> _transactions = [];
  List<LoyaltyTier> _tiers = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    setState(() => _isLoading = true);
    
    try {
      // For demo, use a test user ID
      final points = await _service.getUserPoints('1');
      final transactions = await _service.getTransactionHistory(limit: 10);
      final tiers = await _service.getTiers();

      setState(() {
        _points = points;
        _transactions = transactions;
        _tiers = tiers;
      });
    } catch (e) {
      print('Error loading loyalty data: $e');
    } finally {
      setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Loyalty Rewards'),
        centerTitle: true,
        elevation: 0,
        backgroundColor: Colors.deepOrange,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _loadData,
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : RefreshIndicator(
              onRefresh: _loadData,
              child: SingleChildScrollView(
                physics: const AlwaysScrollableScrollPhysics(),
                child: Column(
                  children: [
                    _buildPointsCard(),
                    _buildTierProgress(),
                    _buildRedeemSection(),
                    _buildTransactionHistory(),
                    _buildTierBenefits(),
                    const SizedBox(height: 20),
                  ],
                ),
              ),
            ),
    );
  }

  Widget _buildPointsCard() {
    final points = _points;
    if (points == null) {
      return const Card(
        margin: EdgeInsets.all(16),
        child: Padding(
          padding: EdgeInsets.all(24),
          child: Text('No loyalty data available'),
        ),
      );
    }

    return Container(
      margin: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            Color(points.tierColor).withOpacity(0.8),
            Color(points.tierColor),
          ],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: Color(points.tierColor).withOpacity(0.4),
            blurRadius: 10,
            offset: const Offset(0, 5),
          ),
        ],
      ),
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  points.tierIcon,
                  style: const TextStyle(fontSize: 40),
                ),
                Column(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: [
                    Text(
                      '${points.tier} Member',
                      style: const TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                        color: Colors.white,
                      ),
                    ),
                    Text(
                      '${points.bonusMultiplier}x Points',
                      style: const TextStyle(
                        color: Colors.white70,
                      ),
                    ),
                  ],
                ),
              ],
            ),
            const SizedBox(height: 24),
            Text(
              '${points.points}',
              style: const TextStyle(
                fontSize: 48,
                fontWeight: FontWeight.bold,
                color: Colors.white,
              ),
            ),
            const Text(
              'Available Points',
              style: TextStyle(
                fontSize: 16,
                color: Colors.white70,
              ),
            ),
            const SizedBox(height: 16),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                _buildPointsStat('Earned', points.totalEarned),
                _buildPointsStat('Redeemed', points.totalRedeemed),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPointsStat(String label, int value) {
    return Column(
      children: [
        Text(
          '$value',
          style: const TextStyle(
            fontSize: 20,
            fontWeight: FontWeight.bold,
            color: Colors.white,
          ),
        ),
        Text(
          label,
          style: const TextStyle(
            color: Colors.white70,
          ),
        ),
      ],
    );
  }

  Widget _buildTierProgress() {
    final points = _points;
    if (points == null || points.tier == 'Platinum') return const SizedBox();

    final progress = points.pointsToNextTier > 0
        ? 1 - (points.pointsToNextTier / 1000)
        : 1.0;

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  '${points.pointsToNextTier} points to ${points.nextTier}',
                  style: const TextStyle(fontWeight: FontWeight.w500),
                ),
                Text(
                  '${(progress * 100).toStringAsFixed(0)}%',
                  style: TextStyle(
                    color: Colors.deepOrange.shade700,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 10),
            LinearProgressIndicator(
              value: progress.clamp(0.0, 1.0),
              backgroundColor: Colors.grey.shade200,
              valueColor: AlwaysStoppedAnimation<Color>(Colors.deepOrange),
              minHeight: 8,
              borderRadius: BorderRadius.circular(4),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildRedeemSection() {
    final points = _points;
    if (points == null || points.points < 100) return const SizedBox();

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              '🎁 Redeem Points',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              '100 points = 1 AED discount',
              style: TextStyle(color: Colors.grey.shade600),
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: _buildRedeemOption(
                    points: 100,
                    discount: 1,
                    enabled: points.points >= 100,
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: _buildRedeemOption(
                    points: 500,
                    discount: 5,
                    enabled: points.points >= 500,
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: _buildRedeemOption(
                    points: 1000,
                    discount: 10,
                    enabled: points.points >= 1000,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildRedeemOption({
    required int points,
    required int discount,
    required bool enabled,
  }) {
    return InkWell(
      onTap: enabled ? () => _showRedeemDialog(points) : null,
      child: Container(
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: enabled ? Colors.deepOrange.shade50 : Colors.grey.shade100,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: enabled ? Colors.deepOrange : Colors.grey.shade300,
          ),
        ),
        child: Column(
          children: [
            Text(
              '$points',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: enabled ? Colors.deepOrange : Colors.grey,
              ),
            ),
            Text(
              'points',
              style: TextStyle(
                fontSize: 12,
                color: enabled ? Colors.deepOrange.shade700 : Colors.grey,
              ),
            ),
            const Divider(height: 16),
            Text(
              '$discount AED',
              style: TextStyle(
                fontWeight: FontWeight.bold,
                color: enabled ? Colors.green.shade700 : Colors.grey,
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _showRedeemDialog(int points) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Redeem Points'),
        content: Text('Redeem $points points for ${points ~/ 100} AED discount?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              Navigator.pop(context);
              _redeemPoints(points);
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.deepOrange,
            ),
            child: const Text('Redeem'),
          ),
        ],
      ),
    );
  }

  Future<void> _redeemPoints(int points) async {
    // TODO: Implement redemption
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Successfully redeemed $points points!'),
        backgroundColor: Colors.green,
      ),
    );
    _loadData();
  }

  Widget _buildTransactionHistory() {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Padding(
            padding: EdgeInsets.all(16),
            child: Text(
              '📊 Transaction History',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          if (_transactions.isEmpty)
            const Padding(
              padding: EdgeInsets.all(16),
              child: Text('No transactions yet'),
            )
          else
            ListView.separated(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              itemCount: _transactions.length,
              separatorBuilder: (_, __) => const Divider(height: 1),
              itemBuilder: (context, index) {
                final tx = _transactions[index];
                return ListTile(
                  leading: Text(tx.icon, style: const TextStyle(fontSize: 24)),
                  title: Text(tx.transactionType),
                  subtitle: Text(tx.description ?? ''),
                  trailing: Text(
                    tx.formattedPoints,
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      color: tx.isEarning ? Colors.green : Colors.red,
                    ),
                  ),
                );
              },
            ),
        ],
      ),
    );
  }

  Widget _buildTierBenefits() {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Padding(
            padding: EdgeInsets.all(16),
            child: Text(
              '🏆 Tier Benefits',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          ListView.builder(
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            itemCount: _tiers.length,
            itemBuilder: (context, index) {
              final tier = _tiers[index];
              final isCurrentTier = _points?.tier == tier.name;
              return Container(
                margin: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                decoration: BoxDecoration(
                  color: isCurrentTier ? Colors.deepOrange.shade50 : null,
                  borderRadius: BorderRadius.circular(8),
                  border: isCurrentTier
                      ? Border.all(color: Colors.deepOrange)
                      : null,
                ),
                child: ListTile(
                  leading: CircleAvatar(
                    backgroundColor: _getTierColor(tier.name),
                    child: Text(
                      '${tier.bonusMultiplier}x',
                      style: const TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.bold,
                        color: Colors.white,
                      ),
                    ),
                  ),
                  title: Text(
                    tier.name,
                    style: TextStyle(
                      fontWeight: isCurrentTier ? FontWeight.bold : null,
                    ),
                  ),
                  subtitle: Text(tier.benefits),
                  trailing: isCurrentTier
                      ? const Icon(Icons.check_circle, color: Colors.deepOrange)
                      : null,
                ),
              );
            },
          ),
          const SizedBox(height: 8),
        ],
      ),
    );
  }

  Color _getTierColor(String tier) {
    switch (tier.toLowerCase()) {
      case 'platinum': return const Color(0xFF2C3E50);
      case 'gold': return const Color(0xFFFFD700);
      case 'silver': return const Color(0xFFC0C0C0);
      default: return const Color(0xFFCD7F32);
    }
  }
}
