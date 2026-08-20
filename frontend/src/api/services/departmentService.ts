import { apiClient } from "../client/apiClient";
import type { ApiResponse, PaginatedResponse } from "../models/common";
import type {
  CreateDepartmentRequest,
  Department,
  DepartmentQueryParams,
  UpdateDepartmentRequest,
} from "../models/department";

export const departmentService = {
  async create(
    request: CreateDepartmentRequest,
  ): Promise<ApiResponse<Department>> {
    const response = await apiClient.post<ApiResponse<Department>>(
      "/department",
      request,
    );

    return response.data;
  },

  async getAll(
    params?: DepartmentQueryParams,
  ): Promise<PaginatedResponse<Department>> {
    const response = await apiClient.get<PaginatedResponse<Department>>(
      "/department",
      {
        params,
      },
    );

    return response.data;
  },

  async getById(id: number): Promise<ApiResponse<Department>> {
    const response = await apiClient.get<ApiResponse<Department>>(
      `/department/${id}`,
    );

    return response.data;
  },

  async update(
    id: number,
    request: UpdateDepartmentRequest,
  ): Promise<ApiResponse<Department>> {
    const response = await apiClient.put<ApiResponse<Department>>(
      `/department/${id}`,
      request,
    );

    return response.data;
  },
};
