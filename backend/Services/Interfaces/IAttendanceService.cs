using backend.Common;
using backend.Models.DTOs.Attendance;

namespace backend.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<ApiResponse<AttendanceResponse>> CheckInAsync(
            CheckInRequest request,
            string authenticatedUserId,
            bool isAdmin);

        Task<ApiResponse<AttendanceResponse>> CheckOutAsync(
            CheckOutRequest request,
            string authenticatedUserId,
            bool isAdmin);

        Task<ApiResponse<AttendanceResponse>> GetByIdAsync(
            int id,
            string authenticatedUserId,
            bool isAdmin);

        Task<ApiResponse<PagedResponse<AttendanceResponse>>> GetAllAsync(
            PaginationRequest request,
            string? employeeId,
            DateTime? startDate,
            DateTime? endDate,
            string authenticatedUserId,
            bool isAdmin);

        Task<ApiResponse<AttendanceResponse>> UpdateAsync(
            int id,
            UpdateAttendanceRequest request);
    }
}
