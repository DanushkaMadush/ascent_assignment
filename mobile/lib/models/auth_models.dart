class LoginRequest {
  final String email;
  final String password;

  LoginRequest({required this.email, required this.password});

  Map<String, dynamic> toJson() {
    return {'email': email, 'password': password};
  }
}

class AuthTokenResponse {
  final String accessToken;
  final String refreshToken;

  AuthTokenResponse({required this.accessToken, required this.refreshToken});

  factory AuthTokenResponse.fromJson(Map<String, dynamic> json) {
    return AuthTokenResponse(
      accessToken: json['accessToken'],
      refreshToken: json['refreshToken'],
    );
  }
}
