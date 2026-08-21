import 'package:flutter/material.dart';

import 'screens/login_screen.dart';

void main() {
  runApp(const EmsApp());
}

class EmsApp extends StatelessWidget {
  const EmsApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'EMS',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(
          seedColor: Colors.blue,
        ),
        useMaterial3: true,
      ),
      home: const LoginScreen(),
    );
  }
}