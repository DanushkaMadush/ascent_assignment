using backend.Common;
using backend.Models.DTOs.Employee;

namespace backend.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<ApiResponse<EmployeeResponse>> CreateAsync(CreateEmployeeRequest request);

        Task<ApiResponse<EmployeeResponse>> GetByIdAsync(string id);

        Task<ApiResponse<PagedResponse<EmployeeResponse>>> GetAllAsync(PaginationRequest request,string? search,int? departmentId);

        Task<ApiResponse<EmployeeResponse>> UpdateAsync(string id,UpdateEmployeeRequest request);

        Task<ApiResponse<object>> DeleteAsync(string id);
    }
}
