using backend.Common;
using backend.Data;
using backend.Models.DTOs.Employee;
using backend.Models.Entities;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private static readonly string[] AllowedRoles =
        {
        "Admin",
        "Manager",
        "Employee"
        };

        public EmployeeService(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApiResponse<EmployeeResponse>> CreateAsync(
            CreateEmployeeRequest request)
        {
            if (!AllowedRoles.Contains(
                    request.Role,
                    StringComparer.OrdinalIgnoreCase))
            {
                return ApiResponse<EmployeeResponse>.FailureResponse(
                    "Invalid role.",
                    new
                    {
                        allowedRoles = AllowedRoles
                    });
            }

            var employeeCodeExists = await _context.Employees
                .AnyAsync(e => e.EmployeeCode == request.EmployeeCode);

            if (employeeCodeExists)
            {
                return ApiResponse<EmployeeResponse>.FailureResponse(
                    "Employee code already exists.");
            }

            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == request.DepartmentId);

            if (!departmentExists)
            {
                return ApiResponse<EmployeeResponse>.FailureResponse(
                    "Department not found.");
            }

            if (!string.IsNullOrWhiteSpace(request.ManagerId))
            {
                var managerExists = await _context.Employees
                    .AnyAsync(e =>
                        e.Id == request.ManagerId &&
                        e.IsActive);

                if (!managerExists)
                {
                    return ApiResponse<EmployeeResponse>.FailureResponse(
                        "Manager not found or inactive.");
                }
            }

            var existingUser =
                await _userManager.FindByEmailAsync(request.Email);

            if (existingUser is not null)
            {
                return ApiResponse<EmployeeResponse>.FailureResponse(
                    "An account with this email already exists.");
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    EmailConfirmed = true
                };

                var userResult = await _userManager.CreateAsync(
                    user,
                    request.Password);

                if (!userResult.Succeeded)
                {
                    var errors = userResult.Errors
                        .GroupBy(e => e.Code)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.Description)
                                .ToArray());

                    await transaction.RollbackAsync();

                    return ApiResponse<EmployeeResponse>
                        .FailureResponse(
                            "Failed to create employee account.",
                            errors);
                }

                var employee = new Employee
                {
                    // Shared primary key
                    Id = user.Id,

                    EmployeeCode = request.EmployeeCode,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    DepartmentId = request.DepartmentId,
                    ManagerId = request.ManagerId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Employees.Add(employee);

                await _context.SaveChangesAsync();

                var roleResult = await _userManager.AddToRoleAsync(
                    user,
                    request.Role);

                if (!roleResult.Succeeded)
                {
                    var errors = roleResult.Errors
                        .GroupBy(e => e.Code)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.Description)
                                .ToArray());

                    await transaction.RollbackAsync();

                    return ApiResponse<EmployeeResponse>
                        .FailureResponse(
                            "Failed to assign employee role.",
                            errors);
                }

                await transaction.CommitAsync();

                var result = await BuildEmployeeResponseAsync(
                    employee.Id);

                return ApiResponse<EmployeeResponse>
                    .SuccessResponse(
                        result!,
                        "Employee created successfully.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ApiResponse<EmployeeResponse>> GetByIdAsync(
            string id)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee is null)
            {
                return ApiResponse<EmployeeResponse>.FailureResponse(
                    "Employee not found.");
            }

            var roles = await GetRolesAsync(employee.Id);

            return ApiResponse<EmployeeResponse>.SuccessResponse(
                MapToResponse(employee, roles),
                "Employee retrieved successfully.");
        }

        public async Task<ApiResponse<PagedResponse<EmployeeResponse>>>
            GetAllAsync(
                PaginationRequest request,
                string? search,
                int? departmentId)
        {
            var query = _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(e =>
                    e.EmployeeCode.Contains(search) ||
                    e.FirstName.Contains(search) ||
                    e.LastName.Contains(search) ||
                    e.Email.Contains(search));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(e =>
                    e.DepartmentId == departmentId.Value);
            }

            query = query.OrderBy(e => e.EmployeeCode);

            var totalRecords = await query.CountAsync();

            var employees = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var employeeIds = employees
                .Select(e => e.Id)
                .ToList();

            var users = await _userManager.Users
                .Where(u => employeeIds.Contains(u.Id))
                .ToListAsync();

            var roleMap = new Dictionary<string, string>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                roleMap[user.Id] =
                    roles.FirstOrDefault() ?? string.Empty;
            }

            var data = employees
                .Select(e => MapToResponse(
                    e,
                    roleMap.TryGetValue(
                        e.Id,
                        out var role)
                        ? role
                        : string.Empty))
                .ToList();

            var totalPages = (int)Math.Ceiling(
                totalRecords / (double)request.PageSize);

            var result = new PagedResponse<EmployeeResponse>
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
                PagedResponse<EmployeeResponse>>
                .SuccessResponse(
                    result,
                    "Employees retrieved successfully.");
        }

        public async Task<ApiResponse<EmployeeResponse>> UpdateAsync(
            string id,
            UpdateEmployeeRequest request)
        {
            if (!AllowedRoles.Contains(
                    request.Role,
                    StringComparer.OrdinalIgnoreCase))
            {
                return ApiResponse<EmployeeResponse>.FailureResponse(
                    "Invalid role.");
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee is null)
            {
                return ApiResponse<EmployeeResponse>.FailureResponse(
                    "Employee not found.");
            }

            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == request.DepartmentId);

            if (!departmentExists)
            {
                return ApiResponse<EmployeeResponse>.FailureResponse(
                    "Department not found.");
            }

            if (request.ManagerId == id)
            {
                return ApiResponse<EmployeeResponse>.FailureResponse(
                    "An employee cannot be their own manager.");
            }

            if (!string.IsNullOrWhiteSpace(request.ManagerId))
            {
                var managerExists = await _context.Employees
                    .AnyAsync(e =>
                        e.Id == request.ManagerId &&
                        e.IsActive);

                if (!managerExists)
                {
                    return ApiResponse<EmployeeResponse>.FailureResponse(
                        "Manager not found or inactive.");
                }
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                return ApiResponse<EmployeeResponse>.FailureResponse(
                    "Associated user account not found.");
            }

            var emailChanged =
                !string.Equals(
                    user.Email,
                    request.Email,
                    StringComparison.OrdinalIgnoreCase);

            if (emailChanged)
            {
                var emailExists = await _userManager.Users
                    .AnyAsync(u =>
                        u.Email == request.Email &&
                        u.Id != id);

                if (emailExists)
                {
                    return ApiResponse<EmployeeResponse>
                        .FailureResponse(
                            "An account with this email already exists.");
                }
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                employee.FirstName = request.FirstName;
                employee.LastName = request.LastName;
                employee.Email = request.Email;
                employee.PhoneNumber = request.PhoneNumber;
                employee.DepartmentId = request.DepartmentId;
                employee.ManagerId = request.ManagerId;
                employee.IsActive = request.IsActive;

                user.Email = request.Email;
                user.UserName = request.Email;
                user.PhoneNumber = request.PhoneNumber;

                var userUpdateResult =
                    await _userManager.UpdateAsync(user);

                if (!userUpdateResult.Succeeded)
                {
                    var errors = userUpdateResult.Errors
                        .GroupBy(e => e.Code)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.Description)
                                .ToArray());

                    await transaction.RollbackAsync();

                    return ApiResponse<EmployeeResponse>
                        .FailureResponse(
                            "Failed to update user account.",
                            errors);
                }

                await _context.SaveChangesAsync();

                var currentRoles =
                    await _userManager.GetRolesAsync(user);

                if (!currentRoles.Contains(request.Role))
                {
                    if (currentRoles.Count > 0)
                    {
                        await _userManager.RemoveFromRolesAsync(
                            user,
                            currentRoles);
                    }

                    var roleResult =
                        await _userManager.AddToRoleAsync(
                            user,
                            request.Role);

                    if (!roleResult.Succeeded)
                    {
                        await transaction.RollbackAsync();

                        return ApiResponse<EmployeeResponse>
                            .FailureResponse(
                                "Failed to update employee role.");
                    }
                }

                await transaction.CommitAsync();

                var result =
                    await BuildEmployeeResponseAsync(id);

                return ApiResponse<EmployeeResponse>
                    .SuccessResponse(
                        result!,
                        "Employee updated successfully.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ApiResponse<object>> DeleteAsync(
            string id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee is null)
            {
                return ApiResponse<object>.FailureResponse(
                    "Employee not found.");
            }

            employee.IsActive = false;

            var user = await _userManager.FindByIdAsync(id);

            if (user is not null)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors
                        .GroupBy(e => e.Code)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.Description)
                                .ToArray());

                    return ApiResponse<object>.FailureResponse(
                        "Failed to deactivate employee.",
                        errors);
                }
            }

            await _context.SaveChangesAsync();

            return ApiResponse<object>.SuccessResponse(
                null!,
                "Employee deactivated successfully.");
        }

        private async Task<EmployeeResponse?> BuildEmployeeResponseAsync(
            string employeeId)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee is null)
            {
                return null;
            }

            var roles = await GetRolesAsync(employee.Id);

            return MapToResponse(employee, roles);
        }

        private async Task<string> GetRolesAsync(
            string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return string.Empty;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return roles.FirstOrDefault() ?? string.Empty;
        }

        private static EmployeeResponse MapToResponse(
            Employee employee,
            string role)
        {
            return new EmployeeResponse
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name
                    ?? string.Empty,
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager is null
                    ? null
                    : $"{employee.Manager.FirstName} {employee.Manager.LastName}",
                Role = role,
                IsActive = employee.IsActive,
                CreatedAt = employee.CreatedAt
            };
        }
    }
}
