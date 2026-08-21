import { apiClient } from "../client/apiClient";
import type {
  ApiResponse,
  PaginatedResponse,
} from "../models/common";
import type {
  CreateEmployeeRequest,
  Employee,
  EmployeeQueryParams,
  UpdateEmployeeRequest,
} from "../models/employee";

export const employeeService = {
  async create(
    request: CreateEmployeeRequest
  ): Promise<ApiResponse<Employee>> {
    const response = await apiClient.post<ApiResponse<Employee>>(
      "/employee",
      request
    );

    return response.data;
  },

  async getAll(
    params?: EmployeeQueryParams
  ): Promise<PaginatedResponse<Employee>> {
    const response = await apiClient.get<PaginatedResponse<Employee>>(
      "/employee",
      {
        params,
      }
    );

    return response.data;
  },

  async getById(id: string): Promise<ApiResponse<Employee>> {
    const response = await apiClient.get<ApiResponse<Employee>>(
      `/employee/${id}`
    );

    return response.data;
  },

  async update(
    id: string,
    request: UpdateEmployeeRequest
  ): Promise<ApiResponse<Employee>> {
    const response = await apiClient.put<ApiResponse<Employee>>(
      `/employee/${id}`,
      request
    );

    return response.data;
  },

  async remove(id: string): Promise<ApiResponse<string>> {
    const response = await apiClient.delete<ApiResponse<string>>(
      `/employee/${id}`
    );

    return response.data;
  },
};