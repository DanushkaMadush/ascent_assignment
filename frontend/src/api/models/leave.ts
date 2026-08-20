export interface CreateLeaveRequest {
  leaveType: string;
  startDate: string;
  endDate: string;
  reason: string;
}

export interface UpdateLeaveRequest {
  leaveType: string;
  startDate: string;
  endDate: string;
  reason: string;
}

export interface Leave {
  id: number;
  employeeId: string;
  employeeName: string;
  leaveType: string;
  startDate: string;
  endDate: string;
  reason: string;
  status: string;
  approvedById: string;
  approvedByName: string;
  appliedOn: string;
}

export interface LeaveQueryParams {
  PageNumber?: number;
  PageSize?: number;
  employeeId?: string;
  status?: string;
  leaveType?: string;
  startDate?: string;
  endDate?: string;
}