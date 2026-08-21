import 'dart:convert';

import '../models/api_response.dart';
import '../models/attendance_models.dart';
import 'api_client.dart';

class AttendanceService {
  final ApiClient _apiClient;

  AttendanceService({ApiClient? apiClient})
      : _apiClient = apiClient ?? ApiClient();

  Future<ApiResponse<AttendanceResponse>> checkIn(
    CheckInRequest request,
    String accessToken,
  ) async {
    final response = await _apiClient.post(
      '/attendance/check-in',
      body: request.toJson(),
      accessToken: accessToken,
    );

    return _parseAttendanceResponse(response);
  }

  Future<ApiResponse<AttendanceResponse>> checkOut(
    CheckOutRequest request,
    String accessToken,
  ) async {
    final response = await _apiClient.post(
      '/attendance/check-out',
      body: request.toJson(),
      accessToken: accessToken,
    );

    return _parseAttendanceResponse(response);
  }

  ApiResponse<AttendanceResponse> _parseAttendanceResponse(
    dynamic response,
  ) {
    final Map<String, dynamic> jsonResponse =
        jsonDecode(response.body);

    if (response.statusCode >= 200 && response.statusCode < 300) {
      return ApiResponse<AttendanceResponse>.fromJson(
        jsonResponse,
        (json) => AttendanceResponse.fromJson(
          json as Map<String, dynamic>,
        ),
      );
    }

    return ApiResponse<AttendanceResponse>(
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