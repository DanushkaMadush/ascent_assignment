import 'dart:convert';
import 'package:http/http.dart' as http;

class ApiClient {
  // backend url
  // static const String baseUrl = 'https://10.0.2.2:7131/api/v1';
  static const String baseUrl = 'https://localhost:7131/api/v1';

  Future<http.Response> post(
    String endpoint, {
    Map<String, dynamic>? body,
    String? accessToken,
  }) async {
    final headers = {
      'Content-Type': 'application/json',
      'Accept': 'text/plain',
    };

    if (accessToken != null && accessToken.isNotEmpty) {
      headers['Authorization'] = 'Bearer $accessToken';
    }

    final response = await http.post(
      Uri.parse('$baseUrl$endpoint'),
      headers: headers,
      body: body != null ? jsonEncode(body) : null,
    );

    return response;
  }
}