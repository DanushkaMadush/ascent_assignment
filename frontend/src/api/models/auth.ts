export interface LoginRequest {
  email: string;
  password: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface AuthTokenResponse {
  accessToken: string;
  refreshToken: string;
}
