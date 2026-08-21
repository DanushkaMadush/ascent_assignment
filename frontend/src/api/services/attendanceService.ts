import { apiClient } from "../client/apiClient";
import type {
  ApiResponse,
  PaginatedResponse,
} from "../models/common";
import type {
  Attendance,
  AttendanceQueryParams,
  UpdateAttendanceRequest,
} from "../models/attendance";

export const attendanceService = {
  async getAll(
    params?: AttendanceQueryParams
  ): Promise<PaginatedResponse<Attendance>> {
    const response = await apiClient.get<PaginatedResponse<Attendance>>(
      "/attendance",
      {
        params,
      }
    );

    return response.data;
  },

  async getById(id: number): Promise<ApiResponse<Attendance>> {
    const response = await apiClient.get<ApiResponse<Attendance>>(
      `/attendance/${id}`
    );

    return response.data;
  },

  async update(
    id: number,
    request: UpdateAttendanceRequest
  ): Promise<ApiResponse<Attendance>> {
    const response = await apiClient.put<ApiResponse<Attendance>>(
      `/attendance/${id}`,
      request
    );

    return response.data;
  },
};