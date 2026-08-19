using backend.Common;
using backend.Data;
using backend.Models.DTOs.Department;
using backend.Models.Entities;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _context;

        public DepartmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<DepartmentResponse>> CreateAsync(
            CreateDepartmentRequest request)
        {
            var name = request.Name.Trim();

            var nameExists = await _context.Departments
                .AnyAsync(d => d.Name == name);

            if (nameExists)
            {
                return ApiResponse<DepartmentResponse>
                    .FailureResponse(
                        "Department name already exists.");
            }

            var department = new Department
            {
                Name = name,
                Description = string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim()
            };

            _context.Departments.Add(department);

            await _context.SaveChangesAsync();

            return ApiResponse<DepartmentResponse>
                .SuccessResponse(
                    MapToResponse(department),
                    "Department created successfully.");
        }

        public async Task<ApiResponse<DepartmentResponse>> GetByIdAsync(
            int id)
        {
            var department = await _context.Departments
                .AsNoTracking()
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department is null)
            {
                return ApiResponse<DepartmentResponse>
                    .FailureResponse(
                        "Department not found.");
            }

            return ApiResponse<DepartmentResponse>
                .SuccessResponse(
                    MapToResponse(department),
                    "Department retrieved successfully.");
        }

        public async Task<
            ApiResponse<PagedResponse<DepartmentResponse>>> GetAllAsync(
                PaginationRequest request,
                string? search)
        {
            var query = _context.Departments
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(d =>
                    d.Name.Contains(search) ||
                    (d.Description != null &&
                     d.Description.Contains(search)));
            }

            query = query.OrderBy(d => d.Name);

            var totalRecords = await query.CountAsync();

            var departments = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    EmployeeCount = d.Employees.Count
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(
                totalRecords / (double)request.PageSize);

            var result = new PagedResponse<DepartmentResponse>
            {
                Data = departments,
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
                PagedResponse<DepartmentResponse>>
                .SuccessResponse(
                    result,
                    "Departments retrieved successfully.");
        }

        public async Task<ApiResponse<DepartmentResponse>> UpdateAsync(
            int id,
            UpdateDepartmentRequest request)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department is null)
            {
                return ApiResponse<DepartmentResponse>
                    .FailureResponse(
                        "Department not found.");
            }

            var name = request.Name.Trim();

            var nameExists = await _context.Departments
                .AnyAsync(d =>
                    d.Id != id &&
                    d.Name == name);

            if (nameExists)
            {
                return ApiResponse<DepartmentResponse>
                    .FailureResponse(
                        "Department name already exists.");
            }

            department.Name = name;

            department.Description =
                string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim();

            await _context.SaveChangesAsync();

            var result = await _context.Departments
                .AsNoTracking()
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);

            return ApiResponse<DepartmentResponse>
                .SuccessResponse(
                    MapToResponse(result!),
                    "Department updated successfully.");
        }

        private static DepartmentResponse MapToResponse(
            Department department)
        {
            return new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                EmployeeCount = department.Employees.Count
            };
        }
    }
}
