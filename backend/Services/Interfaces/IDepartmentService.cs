using backend.Common;
using backend.Models.DTOs.Department;

namespace backend.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<ApiResponse<DepartmentResponse>> CreateAsync(
            CreateDepartmentRequest request);

        Task<ApiResponse<DepartmentResponse>> GetByIdAsync(
            int id);

        Task<ApiResponse<PagedResponse<DepartmentResponse>>> GetAllAsync(
            PaginationRequest request,
            string? search);

        Task<ApiResponse<DepartmentResponse>> UpdateAsync(
            int id,
            UpdateDepartmentRequest request);
    }
}
