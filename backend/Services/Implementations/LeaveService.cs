using backend.Common;
using backend.Data;
using backend.Models.DTOs.Leave;
using backend.Models.Entities;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Implementations
{
    public class LeaveService : ILeaveService
    {
        private readonly AppDbContext _context;

        public LeaveService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<LeaveResponse>> CreateAsync(
            CreateLeaveRequest request,
            string authenticatedUserId)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.Id == authenticatedUserId);

            if (employee is null)
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Employee not found.");
            }

            if (!employee.IsActive)
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Employee is inactive.");
            }

            if (request.StartDate.Date > request.EndDate.Date)
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Start date cannot be after end date.");
            }

            if (string.IsNullOrWhiteSpace(request.LeaveType))
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Leave type is required.");
            }

            var leave = new Leave
            {
                EmployeeId = employee.Id,
                LeaveType = request.LeaveType.Trim(),
                StartDate = request.StartDate.Date,
                EndDate = request.EndDate.Date,
                Reason = string.IsNullOrWhiteSpace(request.Reason)
                    ? null
                    : request.Reason.Trim(),
                Status = "Pending",
                ApprovedById = null,
                AppliedOn = DateTime.UtcNow
            };

            _context.Leaves.Add(leave);

            await _context.SaveChangesAsync();

            var result = await BuildLeaveResponseAsync(leave.Id);

            return ApiResponse<LeaveResponse>
                .SuccessResponse(
                    result!,
                    "Leave request submitted successfully.");
        }

        public async Task<ApiResponse<LeaveResponse>> GetByIdAsync(
            int id,
            string authenticatedUserId,
            bool isAdmin)
        {
            var leave = await _context.Leaves
                .AsNoTracking()
                .Include(l => l.Employee)
                .Include(l => l.ApprovedBy)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leave is null)
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Leave request not found.");
            }

            if (!isAdmin &&
                leave.EmployeeId != authenticatedUserId)
            {
                var manager = await GetEmployeeAsync(
                    authenticatedUserId);

                if (manager is null ||
                    leave.Employee.ManagerId != manager.Id)
                {
                    return ApiResponse<LeaveResponse>
                        .FailureResponse(
                            "You are not authorized to view this leave request.");
                }
            }

            return ApiResponse<LeaveResponse>
                .SuccessResponse(
                    MapToResponse(leave),
                    "Leave request retrieved successfully.");
        }

        public async Task<
            ApiResponse<PagedResponse<LeaveResponse>>> GetAllAsync(
                PaginationRequest request,
                string? employeeId,
                string? status,
                string? leaveType,
                DateTime? startDate,
                DateTime? endDate,
                string authenticatedUserId,
                bool isAdmin)
        {
            var query = _context.Leaves
                .AsNoTracking()
                .Include(l => l.Employee)
                .Include(l => l.ApprovedBy)
                .AsQueryable();

            if (!isAdmin)
            {
                var authenticatedEmployee =
                    await GetEmployeeAsync(authenticatedUserId);

                if (authenticatedEmployee is null)
                {
                    return ApiResponse<
                        PagedResponse<LeaveResponse>>
                        .FailureResponse(
                            "Employee not found.");
                }

                if (!string.IsNullOrWhiteSpace(employeeId))
                {
                    if (employeeId != authenticatedUserId)
                    {
                        var managesEmployee =
                            await _context.Employees
                                .AnyAsync(e =>
                                    e.Id == employeeId &&
                                    e.ManagerId == authenticatedUserId);

                        if (!managesEmployee)
                        {
                            return ApiResponse<
                                PagedResponse<LeaveResponse>>
                                .FailureResponse(
                                    "You are not authorized to view this employee's leave requests.");
                        }
                    }

                    query = query.Where(l =>
                        l.EmployeeId == employeeId);
                }
                else
                {
                    query = query.Where(l =>
                        l.EmployeeId == authenticatedUserId ||
                        l.Employee.ManagerId == authenticatedUserId);
                }
            }
            else if (!string.IsNullOrWhiteSpace(employeeId))
            {
                query = query.Where(l =>
                    l.EmployeeId == employeeId);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.Trim();

                query = query.Where(l =>
                    l.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(leaveType))
            {
                leaveType = leaveType.Trim();

                query = query.Where(l =>
                    l.LeaveType == leaveType);
            }

            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;

                query = query.Where(l =>
                    l.EndDate >= start);
            }

            if (endDate.HasValue)
            {
                var end = endDate.Value.Date;

                query = query.Where(l =>
                    l.StartDate <= end);
            }

            query = query
                .OrderByDescending(l => l.AppliedOn);

            var totalRecords = await query.CountAsync();

            var leaves = await query
                .Skip((request.PageNumber - 1) *
                      request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var data = leaves
                .Select(MapToResponse)
                .ToList();

            var totalPages = (int)Math.Ceiling(
                totalRecords / (double)request.PageSize);

            var result = new PagedResponse<LeaveResponse>
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
                PagedResponse<LeaveResponse>>
                .SuccessResponse(
                    result,
                    "Leave requests retrieved successfully.");
        }

        public async Task<ApiResponse<LeaveResponse>> UpdateAsync(
            int id,
            UpdateLeaveRequest request,
            string authenticatedUserId,
            bool isAdmin)
        {
            var leave = await _context.Leaves
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leave is null)
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Leave request not found.");
            }

            if (!isAdmin &&
                leave.EmployeeId != authenticatedUserId)
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "You are not authorized to edit this leave request.");
            }

            if (leave.Status != "Pending")
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Only pending leave requests can be edited.");
            }

            if (request.StartDate.Date >
                request.EndDate.Date)
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Start date cannot be after end date.");
            }

            if (string.IsNullOrWhiteSpace(request.LeaveType))
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Leave type is required.");
            }

            leave.LeaveType = request.LeaveType.Trim();

            leave.StartDate = request.StartDate.Date;

            leave.EndDate = request.EndDate.Date;

            leave.Reason =
                string.IsNullOrWhiteSpace(request.Reason)
                    ? null
                    : request.Reason.Trim();

            await _context.SaveChangesAsync();

            var result =
                await BuildLeaveResponseAsync(id);

            return ApiResponse<LeaveResponse>
                .SuccessResponse(
                    result!,
                    "Leave request updated successfully.");
        }

        public async Task<ApiResponse<LeaveResponse>> CancelAsync(
            int id,
            string authenticatedUserId,
            bool isAdmin)
        {
            var leave = await _context.Leaves
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leave is null)
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Leave request not found.");
            }

            if (!isAdmin &&
                leave.EmployeeId != authenticatedUserId)
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "You are not authorized to cancel this leave request.");
            }

            if (leave.Status != "Pending")
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Only pending leave requests can be cancelled.");
            }

            leave.Status = "Cancelled";

            await _context.SaveChangesAsync();

            var result =
                await BuildLeaveResponseAsync(id);

            return ApiResponse<LeaveResponse>
                .SuccessResponse(
                    result!,
                    "Leave request cancelled successfully.");
        }

        public async Task<ApiResponse<LeaveResponse>> ApproveAsync(
            int id,
            string authenticatedUserId,
            bool isAdmin)
        {
            var leave = await _context.Leaves
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leave is null)
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Leave request not found.");
            }

            if (!isAdmin)
            {
                var canApprove =
                    await CanManagerProcessLeaveAsync(
                        leave.EmployeeId,
                        authenticatedUserId);

                if (!canApprove)
                {
                    return ApiResponse<LeaveResponse>
                        .FailureResponse(
                            "You are not authorized to approve this leave request.");
                }
            }

            if (leave.Status != "Pending")
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Only pending leave requests can be approved.");
            }

            leave.Status = "Approved";
            leave.ApprovedById = authenticatedUserId;

            await _context.SaveChangesAsync();

            var result =
                await BuildLeaveResponseAsync(id);

            return ApiResponse<LeaveResponse>
                .SuccessResponse(
                    result!,
                    "Leave request approved successfully.");
        }

        public async Task<ApiResponse<LeaveResponse>> RejectAsync(
            int id,
            string authenticatedUserId,
            bool isAdmin)
        {
            var leave = await _context.Leaves
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leave is null)
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Leave request not found.");
            }

            if (!isAdmin)
            {
                var canReject =
                    await CanManagerProcessLeaveAsync(
                        leave.EmployeeId,
                        authenticatedUserId);

                if (!canReject)
                {
                    return ApiResponse<LeaveResponse>
                        .FailureResponse(
                            "You are not authorized to reject this leave request.");
                }
            }

            if (leave.Status != "Pending")
            {
                return ApiResponse<LeaveResponse>
                    .FailureResponse(
                        "Only pending leave requests can be rejected.");
            }

            leave.Status = "Rejected";
            leave.ApprovedById = authenticatedUserId;

            await _context.SaveChangesAsync();

            var result =
                await BuildLeaveResponseAsync(id);

            return ApiResponse<LeaveResponse>
                .SuccessResponse(
                    result!,
                    "Leave request rejected successfully.");
        }

        private async Task<bool> CanManagerProcessLeaveAsync(
            string employeeId,
            string managerId)
        {
            return await _context.Employees
                .AnyAsync(e =>
                    e.Id == employeeId &&
                    e.ManagerId == managerId);
        }

        private async Task<Employee?> GetEmployeeAsync(
            string employeeId)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.Id == employeeId);
        }

        private async Task<LeaveResponse?>
            BuildLeaveResponseAsync(int leaveId)
        {
            var leave = await _context.Leaves
                .AsNoTracking()
                .Include(l => l.Employee)
                .Include(l => l.ApprovedBy)
                .FirstOrDefaultAsync(l =>
                    l.Id == leaveId);

            if (leave is null)
            {
                return null;
            }

            return MapToResponse(leave);
        }

        private static LeaveResponse MapToResponse(
            Leave leave)
        {
            return new LeaveResponse
            {
                Id = leave.Id,

                EmployeeId = leave.EmployeeId,

                EmployeeName =
                    $"{leave.Employee.FirstName} " +
                    $"{leave.Employee.LastName}",

                LeaveType = leave.LeaveType,

                StartDate = leave.StartDate,

                EndDate = leave.EndDate,

                Reason = leave.Reason,

                Status = leave.Status,

                ApprovedById = leave.ApprovedById,

                ApprovedByName = leave.ApprovedBy is null
                    ? null
                    : $"{leave.ApprovedBy.FirstName} " +
                      $"{leave.ApprovedBy.LastName}",

                AppliedOn = leave.AppliedOn
            };
        }
    }
}
