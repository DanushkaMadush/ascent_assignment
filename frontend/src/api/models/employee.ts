export interface CreateEmployeeRequest {
  employeeCode: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  password: string;
  departmentId: number;
  managerId: string;
  role: string;
}

export interface UpdateEmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  departmentId: number;
  managerId: string;
  role: string;
  isActive: boolean;
}

export interface Employee {
  id: string;
  employeeCode: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  departmentId: number;
  departmentName: string;
  managerId: string;
  managerName: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

export interface EmployeeQueryParams {
  PageNumber?: number;
  PageSize?: number;
  search?: string;
  departmentId?: number;
}