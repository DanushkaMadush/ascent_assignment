import { apiClient } from "../client/apiClient";
import type {
  AuthTokenResponse,
  LoginRequest,
  RefreshTokenRequest,
} from "../models/auth";
import type { ApiResponse } from "../models/common";

export const authService = {
  async login(request: LoginRequest): Promise<ApiResponse<AuthTokenResponse>> {
    const response = await apiClient.post<ApiResponse<AuthTokenResponse>>(
      "/auth/login",
      request,
    );

    return response.data;
  },

  async refresh(
    request: RefreshTokenRequest,
  ): Promise<ApiResponse<AuthTokenResponse>> {
    const response = await apiClient.post<ApiResponse<AuthTokenResponse>>(
      "/auth/refresh",
      request,
    );

    return response.data;
  },
};
