using backend.Common;
using backend.Models.DTOs.Leave;

namespace backend.Services.Interfaces
{
    public interface ILeaveService
    {
        Task<ApiResponse<LeaveResponse>> CreateAsync(
            CreateLeaveRequest request,
            string authenticatedUserId);

        Task<ApiResponse<LeaveResponse>> GetByIdAsync(
            int id,
            string authenticatedUserId,
            bool isAdmin);

        Task<ApiResponse<PagedResponse<LeaveResponse>>> GetAllAsync(
            PaginationRequest request,
            string? employeeId,
            string? status,
            string? leaveType,
            DateTime? startDate,
            DateTime? endDate,
            string authenticatedUserId,
            bool isAdmin);

        Task<ApiResponse<LeaveResponse>> UpdateAsync(
            int id,
            UpdateLeaveRequest request,
            string authenticatedUserId,
            bool isAdmin);

        Task<ApiResponse<LeaveResponse>> CancelAsync(
            int id,
            string authenticatedUserId,
            bool isAdmin);

        Task<ApiResponse<LeaveResponse>> ApproveAsync(
            int id,
            string authenticatedUserId,
            bool isAdmin);

        Task<ApiResponse<LeaveResponse>> RejectAsync(
            int id,
            string authenticatedUserId,
            bool isAdmin);
    }
}
