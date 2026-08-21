import 'dart:convert';
import '../models/api_response.dart';
import '../models/auth_models.dart';
import 'api_client.dart';

class AuthService {
  final ApiClient _apiClient;

  AuthService({ApiClient? apiClient})
      : _apiClient = apiClient ?? ApiClient();

  Future<ApiResponse<AuthTokenResponse>> login(
    LoginRequest request,
  ) async {
    final response = await _apiClient.post(
      '/auth/login',
      body: request.toJson(),
    );

    final Map<String, dynamic> jsonResponse = jsonDecode(response.body);

    if (response.statusCode >= 200 && response.statusCode < 300) {
      return ApiResponse<AuthTokenResponse>.fromJson(
        jsonResponse,
        (json) => AuthTokenResponse.fromJson(
          json as Map<String, dynamic>,
        ),
      );
    }

    return ApiResponse<AuthTokenResponse>(
      success: false,
      message: _getErrorMessage(jsonResponse),
      errors: jsonResponse['errors'],
    );
  }

  String _getErrorMessage(Map<String, dynamic> json) {
    if (json['message'] != null) {
      return json['message'].toString();
    }

    if (json['title'] != null) {
      return json['title'].toString();
    }

    return 'Something went wrong. Please try again.';
  }
}