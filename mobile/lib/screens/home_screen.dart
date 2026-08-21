import 'package:flutter/material.dart';

import '../models/attendance_models.dart';
import '../services/attendance_service.dart';
import '../storage/secure_storage_service.dart';
import 'attendance_screen.dart';
import 'leaves_screen.dart';
import 'login_screen.dart';

class HomeScreen extends StatefulWidget {
  final String employeeId;
  final String firstName;
  final String lastName;

  const HomeScreen({
    super.key,
    required this.employeeId,
    required this.firstName,
    required this.lastName,
  });

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final SecureStorageService _storageService = SecureStorageService();
  final AttendanceService _attendanceService = AttendanceService();

  bool _isCheckingIn = false;
  bool _isCheckingOut = false;

  Future<void> _checkIn() async {
    setState(() {
      _isCheckingIn = true;
    });

    try {
      final accessToken = await _storageService.getAccessToken();

      if (accessToken == null || accessToken.isEmpty) {
        _showMessage('Session expired. Please sign in again.');
        return;
      }

      final request = CheckInRequest(
        employeeId: widget.employeeId,
        deviceType: 'Android',
      );

      final result = await _attendanceService.checkIn(
        request,
        accessToken,
      );

      if (!mounted) return;

      if (result.success) {
        _showMessage(
          result.message.isNotEmpty
              ? result.message
              : 'Check-in successful.',
        );
      } else {
        _showMessage(
          result.message.isNotEmpty
              ? result.message
              : 'Check-in failed.',
        );
      }
    } catch (e) {
      if (!mounted) return;

      _showMessage(
        'Unable to connect to the server.',
      );
    } finally {
      if (mounted) {
        setState(() {
          _isCheckingIn = false;
        });
      }
    }
  }

  Future<void> _checkOut() async {
    setState(() {
      _isCheckingOut = true;
    });

    try {
      final accessToken = await _storageService.getAccessToken();

      if (accessToken == null || accessToken.isEmpty) {
        _showMessage('Session expired. Please sign in again.');
        return;
      }

      final request = CheckOutRequest(
        employeeId: widget.employeeId,
      );

      final result = await _attendanceService.checkOut(
        request,
        accessToken,
      );

      if (!mounted) return;

      if (result.success) {
        _showMessage(
          result.message.isNotEmpty
              ? result.message
              : 'Check-out successful.',
        );
      } else {
        _showMessage(
          result.message.isNotEmpty
              ? result.message
              : 'Check-out failed.',
        );
      }
    } catch (e) {
      if (!mounted) return;

      _showMessage(
        'Unable to connect to the server.',
      );
    } finally {
      if (mounted) {
        setState(() {
          _isCheckingOut = false;
        });
      }
    }
  }

  void _showMessage(String message) {
    if (!mounted) return;

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
      ),
    );
  }

  Future<void> _signOut() async {
    await _storageService.clearTokens();

    if (!mounted) return;

    Navigator.of(context).pushAndRemoveUntil(
      MaterialPageRoute(
        builder: (_) => const LoginScreen(),
      ),
      (route) => false,
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
                'Hi ${widget.firstName} ${widget.lastName},',
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
                  onPressed: _isCheckingIn || _isCheckingOut
                      ? null
                      : _checkIn,
                  child: _isCheckingIn
                      ? const SizedBox(
                          width: 22,
                          height: 22,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                          ),
                        )
                      : const Text('Check In'),
                ),
              ),

              const SizedBox(height: 16),

              SizedBox(
                width: 220,
                height: 48,
                child: ElevatedButton(
                  onPressed: _isCheckingIn || _isCheckingOut
                      ? null
                      : _checkOut,
                  child: _isCheckingOut
                      ? const SizedBox(
                          width: 22,
                          height: 22,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                          ),
                        )
                      : const Text('Check Out'),
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
                  onPressed: _isCheckingIn || _isCheckingOut
                      ? null
                      : _signOut,
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