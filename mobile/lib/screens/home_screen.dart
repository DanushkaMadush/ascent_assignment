import 'package:flutter/material.dart';

import '../storage/secure_storage_service.dart';
import 'attendance_screen.dart';
import 'leaves_screen.dart';
import 'login_screen.dart';

class HomeScreen extends StatelessWidget {
  final String employeeId;
  final String firstName;
  final String lastName;

  const HomeScreen({
    super.key,
    required this.employeeId,
    required this.firstName,
    required this.lastName,
  });

  Future<void> _signOut(BuildContext context) async {
    final storageService = SecureStorageService();

    await storageService.clearTokens();

    if (!context.mounted) return;

    Navigator.of(context).pushAndRemoveUntil(
      MaterialPageRoute(
        builder: (_) => const LoginScreen(),
      ),
      (route) => false,
    );
  }

  void _showMessage(
    BuildContext context,
    String message,
  ) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('EMS'),
      ),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            children: [
              const SizedBox(height: 24),

              Text(
                'Hi $firstName $lastName,',
                textAlign: TextAlign.center,
                style: const TextStyle(
                  fontSize: 24,
                  fontWeight: FontWeight.bold,
                ),
              ),

              const SizedBox(height: 8),

              const Text(
                'Welcome back',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 18,
                ),
              ),

              const Spacer(),

              SizedBox(
                width: 220,
                height: 48,
                child: ElevatedButton(
                  onPressed: () {
                    _showMessage(
                      context,
                      'Check In will be implemented next.',
                    );
                  },
                  child: const Text('Check In'),
                ),
              ),

              const SizedBox(height: 16),

              SizedBox(
                width: 220,
                height: 48,
                child: ElevatedButton(
                  onPressed: () {
                    _showMessage(
                      context,
                      'Check Out will be implemented next.',
                    );
                  },
                  child: const Text('Check Out'),
                ),
              ),

              const SizedBox(height: 16),

              SizedBox(
                width: 220,
                height: 48,
                child: ElevatedButton(
                  onPressed: () {
                    Navigator.of(context).push(
                      MaterialPageRoute(
                        builder: (_) => const LeavesScreen(),
                      ),
                    );
                  },
                  child: const Text('Leaves'),
                ),
              ),

              const SizedBox(height: 16),

              SizedBox(
                width: 220,
                height: 48,
                child: ElevatedButton(
                  onPressed: () {
                    Navigator.of(context).push(
                      MaterialPageRoute(
                        builder: (_) => const AttendanceScreen(),
                      ),
                    );
                  },
                  child: const Text('Attendance'),
                ),
              ),

              const Spacer(),

              SizedBox(
                width: 220,
                height: 48,
                child: OutlinedButton(
                  onPressed: () => _signOut(context),
                  child: const Text('Sign Out'),
                ),
              ),

              const SizedBox(height: 24),
            ],
          ),
        ),
      ),
    );
  }
}