export interface UpdateAttendanceRequest {
  checkInTime: string;
  checkOutTime: string;
  workDate: string;
}

export interface Attendance {
  id: number;
  employeeId: string;
  employeeCode: string;
  employeeName: string;
  checkInTime: string;
  checkOutTime: string;
  workDate: string;
  status: string;
  deviceType: string;
}

export interface AttendanceQueryParams {
  PageNumber?: number;
  PageSize?: number;
  employeeId?: string;
  startDate?: string;
  endDate?: string;
}