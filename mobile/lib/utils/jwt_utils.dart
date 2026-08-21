import 'package:jwt_decoder/jwt_decoder.dart';

class JwtUtils {
  static Map<String, dynamic> decode(String token) {
    return JwtDecoder.decode(token);
  }

  static String getEmployeeId(String token) {
    final payload = decode(token);

    return payload['employeeId']?.toString() ?? '';
  }

  static String getFirstName(String token) {
    final payload = decode(token);

    return payload['firstName']?.toString() ?? '';
  }

  static String getLastName(String token) {
    final payload = decode(token);

    return payload['lastName']?.toString() ?? '';
  }

  static bool isExpired(String token) {
    return JwtDecoder.isExpired(token);
  }
}