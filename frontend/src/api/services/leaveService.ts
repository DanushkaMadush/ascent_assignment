import { apiClient } from "../client/apiClient";
import type {
  ApiResponse,
  PaginatedResponse,
} from "../models/common";
import type {
  CreateLeaveRequest,
  Leave,
  LeaveQueryParams,
  UpdateLeaveRequest,
} from "../models/leave";

export const leaveService = {
  async create(
    request: CreateLeaveRequest
  ): Promise<ApiResponse<Leave>> {
    const response = await apiClient.post<ApiResponse<Leave>>(
      "/leave",
      request
    );

    return response.data;
  },

  async getAll(
    params?: LeaveQueryParams
  ): Promise<PaginatedResponse<Leave>> {
    const response = await apiClient.get<PaginatedResponse<Leave>>(
      "/leave",
      {
        params,
      }
    );

    return response.data;
  },

  async getById(id: number): Promise<ApiResponse<Leave>> {
    const response = await apiClient.get<ApiResponse<Leave>>(
      `/leave/${id}`
    );

    return response.data;
  },

  async update(
    id: number,
    request: UpdateLeaveRequest
  ): Promise<ApiResponse<Leave>> {
    const response = await apiClient.put<ApiResponse<Leave>>(
      `/leave/${id}`,
      request
    );

    return response.data;
  },

  async cancel(id: number): Promise<ApiResponse<Leave>> {
    const response = await apiClient.patch<ApiResponse<Leave>>(
      `/leave/${id}/cancel`
    );

    return response.data;
  },

  async approve(id: number): Promise<ApiResponse<Leave>> {
    const response = await apiClient.patch<ApiResponse<Leave>>(
      `/leave/${id}/approve`
    );

    return response.data;
  },

  async reject(id: number): Promise<ApiResponse<Leave>> {
    const response = await apiClient.patch<ApiResponse<Leave>>(
      `/leave/${id}/reject`
    );

    return response.data;
  },
};