export interface CreateDepartmentRequest {
  name: string;
  description: string;
}

export interface UpdateDepartmentRequest {
  name: string;
  description: string;
}

export interface Department {
  id: number;
  name: string;
  description: string;
  employeeCount: number;
}

export interface DepartmentQueryParams {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
}