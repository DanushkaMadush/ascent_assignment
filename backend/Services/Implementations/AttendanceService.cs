using backend.Common;
using backend.Data;
using backend.Models.DTOs.Attendance;
using backend.Models.Entities;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly AppDbContext _context;

        private static readonly TimeZoneInfo SriLankaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows()
                    ? "Sri Lanka Standard Time"
                    : "Asia/Colombo");

        public AttendanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<AttendanceResponse>> CheckInAsync(
            CheckInRequest request,
            string authenticatedUserId,
            bool isAdmin)
        {
            var employeeId = ResolveEmployeeId(
                request.EmployeeId,
                authenticatedUserId,
                isAdmin);

            if (employeeId is null)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "Employee ID is required.");
            }

            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee is null)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "Employee not found.");
            }

            if (!employee.IsActive)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "Employee is inactive.");
            }

            var nowUtc = DateTime.UtcNow;

            var sriLankaNow = TimeZoneInfo.ConvertTimeFromUtc(
                nowUtc,
                SriLankaTimeZone);

            var workDate = sriLankaNow.Date;

            var alreadyCheckedIn = await _context.Attendances
                .AnyAsync(a =>
                    a.EmployeeId == employeeId &&
                    a.WorkDate == workDate);

            if (alreadyCheckedIn)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "Employee has already checked in today.");
            }

            var attendance = new Attendance
            {
                EmployeeId = employeeId,
                CheckInTime = nowUtc,
                CheckOutTime = null,
                WorkDate = workDate,
                Status = null,
                DeviceType = string.IsNullOrWhiteSpace(request.DeviceType)
                    ? null
                    : request.DeviceType.Trim()
            };

            _context.Attendances.Add(attendance);

            await _context.SaveChangesAsync();

            var result = await BuildAttendanceResponseAsync(
                attendance.Id);

            return ApiResponse<AttendanceResponse>
                .SuccessResponse(
                    result!,
                    "Check-in recorded successfully.");
        }

        public async Task<ApiResponse<AttendanceResponse>> CheckOutAsync(
            CheckOutRequest request,
            string authenticatedUserId,
            bool isAdmin)
        {
            var employeeId = ResolveEmployeeId(
                request.EmployeeId,
                authenticatedUserId,
                isAdmin);

            if (employeeId is null)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "Employee ID is required.");
            }

            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee is null)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "Employee not found.");
            }

            var nowUtc = DateTime.UtcNow;

            var sriLankaNow = TimeZoneInfo.ConvertTimeFromUtc(
                nowUtc,
                SriLankaTimeZone);

            var workDate = sriLankaNow.Date;

            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a =>
                    a.EmployeeId == employeeId &&
                    a.WorkDate == workDate);

            if (attendance is null)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "No check-in record found for today.");
            }

            if (attendance.CheckOutTime.HasValue)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "Employee has already checked out today.");
            }

            if (nowUtc <= attendance.CheckInTime)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "Check-out time must be after check-in time.");
            }

            attendance.CheckOutTime = nowUtc;

            await _context.SaveChangesAsync();

            var result = await BuildAttendanceResponseAsync(
                attendance.Id);

            return ApiResponse<AttendanceResponse>
                .SuccessResponse(
                    result!,
                    "Check-out recorded successfully.");
        }

        public async Task<ApiResponse<AttendanceResponse>> GetByIdAsync(
            int id,
            string authenticatedUserId,
            bool isAdmin)
        {
            var attendance = await _context.Attendances
                .AsNoTracking()
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance is null)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "Attendance record not found.");
            }

            if (!isAdmin &&
                attendance.EmployeeId != authenticatedUserId)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "You are not authorized to view this attendance record.");
            }

            return ApiResponse<AttendanceResponse>
                .SuccessResponse(
                    MapToResponse(attendance),
                    "Attendance retrieved successfully.");
        }

        public async Task<
            ApiResponse<PagedResponse<AttendanceResponse>>> GetAllAsync(
                PaginationRequest request,
                string? employeeId,
                DateTime? startDate,
                DateTime? endDate,
                string authenticatedUserId,
                bool isAdmin)
        {
            var query = _context.Attendances
                .AsNoTracking()
                .Include(a => a.Employee)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(a =>
                    a.EmployeeId == authenticatedUserId);
            }
            else if (!string.IsNullOrWhiteSpace(employeeId))
            {
                query = query.Where(a =>
                    a.EmployeeId == employeeId);
            }

            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;

                query = query.Where(a =>
                    a.WorkDate >= start);
            }

            if (endDate.HasValue)
            {
                var end = endDate.Value.Date;

                query = query.Where(a =>
                    a.WorkDate <= end);
            }

            query = query
                .OrderByDescending(a => a.WorkDate)
                .ThenByDescending(a => a.CheckInTime);

            var totalRecords = await query.CountAsync();

            var attendanceRecords = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var data = attendanceRecords
                .Select(MapToResponse)
                .ToList();

            var totalPages = (int)Math.Ceiling(
                totalRecords / (double)request.PageSize);

            var result = new PagedResponse<AttendanceResponse>
            {
                Data = data,
                Pagination = new PaginationMetadata
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    HasPrevious = request.PageNumber > 1,
                    HasNext = request.PageNumber < totalPages
                }
            };

            return ApiResponse<
                PagedResponse<AttendanceResponse>>
                .SuccessResponse(
                    result,
                    "Attendance records retrieved successfully.");
        }

        public async Task<ApiResponse<AttendanceResponse>> UpdateAsync(
            int id,
            UpdateAttendanceRequest request)
        {
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance is null)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "Attendance record not found.");
            }

            if (request.CheckOutTime.HasValue &&
                request.CheckOutTime.Value <= request.CheckInTime)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "Check-out time must be after check-in time.");
            }

            var workDate = request.WorkDate.Date;

            var duplicateExists = await _context.Attendances
                .AnyAsync(a =>
                    a.Id != id &&
                    a.EmployeeId == attendance.EmployeeId &&
                    a.WorkDate == workDate);

            if (duplicateExists)
            {
                return ApiResponse<AttendanceResponse>
                    .FailureResponse(
                        "An attendance record already exists for this employee on the selected work date.");
            }

            attendance.CheckInTime = request.CheckInTime;
            attendance.CheckOutTime = request.CheckOutTime;
            attendance.WorkDate = workDate;

            await _context.SaveChangesAsync();

            var result = await BuildAttendanceResponseAsync(id);

            return ApiResponse<AttendanceResponse>
                .SuccessResponse(
                    result!,
                    "Attendance updated successfully.");
        }

        private static string? ResolveEmployeeId(
            string? requestedEmployeeId,
            string authenticatedUserId,
            bool isAdmin)
        {
            if (isAdmin)
            {
                return string.IsNullOrWhiteSpace(requestedEmployeeId)
                    ? null
                    : requestedEmployeeId.Trim();
            }

            return authenticatedUserId;
        }

        private async Task<AttendanceResponse?>
            BuildAttendanceResponseAsync(int attendanceId)
        {
            var attendance = await _context.Attendances
                .AsNoTracking()
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Id == attendanceId);

            if (attendance is null)
            {
                return null;
            }

            return MapToResponse(attendance);
        }

        private static AttendanceResponse MapToResponse(
            Attendance attendance)
        {
            return new AttendanceResponse
            {
                Id = attendance.Id,
                EmployeeId = attendance.EmployeeId,
                EmployeeCode = attendance.Employee.EmployeeCode,
                EmployeeName =
                    $"{attendance.Employee.FirstName} " +
                    $"{attendance.Employee.LastName}",
                CheckInTime = attendance.CheckInTime,
                CheckOutTime = attendance.CheckOutTime,
                WorkDate = attendance.WorkDate,
                Status = attendance.Status,
                DeviceType = attendance.DeviceType
            };
        }
    }
}
