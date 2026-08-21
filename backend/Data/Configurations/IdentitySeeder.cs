using backend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Data.Configurations
{
    public static class IdentitySeeder
    {
        private const string AdminRole = "Admin";
        private const string ManagerRole = "Manager";
        private const string EmployeeRole = "Employee";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            var context =
                services.GetRequiredService<AppDbContext>();

            await SeedRolesAsync(roleManager);

            var departments = await SeedDepartmentsAsync(context);

            await SeedAdminAsync(
                userManager,
                context,
                departments["Administration"]);

            await SeedDepartmentUsersAsync(
                userManager,
                context,
                departments["IT"],
                "IT",
                "IT Manager",
                "it.manager@ems.com");

            await SeedDepartmentUsersAsync(
                userManager,
                context,
                departments["HR"],
                "HR",
                "HR Manager",
                "hr.manager@ems.com");

            await SeedDepartmentUsersAsync(
                userManager,
                context,
                departments["Administration"],
                "ADM",
                "Administration Manager",
                "admin.manager@ems.com");
        }

        private static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            var roles = new[]
            {
                AdminRole,
                ManagerRole,
                EmployeeRole
            };

            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role))
                {
                    continue;
                }

                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(
                        $"Failed to create role '{role}': {errors}");
                }
            }
        }

        private static async Task<Dictionary<string, Department>>
            SeedDepartmentsAsync(AppDbContext context)
        {
            var departmentDefinitions = new[]
            {
                new
                {
                    Name = "IT",
                    Description = "Information Technology department"
                },
                new
                {
                    Name = "HR",
                    Description = "Human Resources department"
                },
                new
                {
                    Name = "Administration",
                    Description = "Administration department"
                }
            };

            var departments =
                new Dictionary<string, Department>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var definition in departmentDefinitions)
            {
                var department = await context.Departments
                    .FirstOrDefaultAsync(d =>
                        d.Name == definition.Name);

                if (department is null)
                {
                    department = new Department
                    {
                        Name = definition.Name,
                        Description = definition.Description
                    };

                    context.Departments.Add(department);

                    await context.SaveChangesAsync();
                }

                departments[definition.Name] = department;
            }

            return departments;
        }

        private static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            AppDbContext context,
            Department department)
        {
            const string adminEmail = "admin@ems.com";
            const string adminPassword = "Admin@12345";

            var adminUser =
                await userManager.FindByEmailAsync(adminEmail);

            if (adminUser is not null)
            {
                await EnsureRoleAsync(
                    userManager,
                    adminUser,
                    AdminRole);

                var existingEmployee =
                    await context.Employees
                        .FirstOrDefaultAsync(e =>
                            e.Id == adminUser.Id);

                if (existingEmployee is null)
                {
                    var employee = new Employee
                    {
                        Id = adminUser.Id,
                        EmployeeCode = "EMP-ADMIN-001",
                        FirstName = "System",
                        LastName = "Administrator",
                        Email = adminEmail,
                        DepartmentId = department.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Employees.Add(employee);

                    await context.SaveChangesAsync();
                }

                return;
            }

            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                PhoneNumberConfirmed = false
            };

            var userResult = await userManager.CreateAsync(
                adminUser,
                adminPassword);

            if (!userResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    userResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to create Admin user: {errors}");
            }

            var adminEmployee = new Employee
            {
                Id = adminUser.Id,
                EmployeeCode = "EMP-ADMIN-001",
                FirstName = "System",
                LastName = "Administrator",
                Email = adminEmail,
                DepartmentId = department.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Employees.Add(adminEmployee);

            await context.SaveChangesAsync();

            await EnsureRoleAsync(
                userManager,
                adminUser,
                AdminRole);
        }

        private static async Task SeedDepartmentUsersAsync(
            UserManager<ApplicationUser> userManager,
            AppDbContext context,
            Department department,
            string codePrefix,
            string managerName,
            string managerEmail)
        {

            var managerUser =
                await userManager.FindByEmailAsync(managerEmail);

            Employee managerEmployee;

            if (managerUser is null)
            {
                managerUser = new ApplicationUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false
                };

                var managerResult =
                    await userManager.CreateAsync(
                        managerUser,
                        "Manager@12345");

                if (!managerResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        managerResult.Errors.Select(
                            e => e.Description));

                    throw new InvalidOperationException(
                        $"Failed to create manager '{managerEmail}': {errors}");
                }

                managerEmployee = new Employee
                {
                    Id = managerUser.Id,
                    EmployeeCode = $"MGR-{codePrefix}-001",
                    FirstName = managerName.Split(' ')[0],
                    LastName = managerName.Contains(' ')
                        ? managerName[(managerName.IndexOf(' ') + 1)..]
                        : "Manager",
                    Email = managerEmail,
                    DepartmentId = department.Id,
                    ManagerId = null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                context.Employees.Add(managerEmployee);

                await context.SaveChangesAsync();
            }
            else
            {
                managerEmployee =
                    await context.Employees
                        .FirstOrDefaultAsync(e =>
                            e.Id == managerUser.Id)
                    ?? throw new InvalidOperationException(
                        $"Identity user '{managerEmail}' exists, " +
                        "but its Employee record does not exist.");
            }

            await EnsureRoleAsync(
                userManager,
                managerUser,
                ManagerRole);

            var employeeDefinitions = new[]
            {
                new
                {
                    Number = 1,
                    FirstName = "John",
                    LastName = $"{codePrefix} Employee"
                },
                new
                {
                    Number = 2,
                    FirstName = "Jane",
                    LastName = $"{codePrefix} Employee"
                },
                new
                {
                    Number = 3,
                    FirstName = "Michael",
                    LastName = $"{codePrefix} Employee"
                }
            };

            foreach (var definition in employeeDefinitions)
            {
                var employeeEmail =
                    $"{codePrefix.ToLowerInvariant()}.employee" +
                    $"{definition.Number}@ems.com";

                var employeeUser =
                    await userManager.FindByEmailAsync(
                        employeeEmail);

                Employee? employee;

                if (employeeUser is not null)
                {
                    employee =
                        await context.Employees
                            .FirstOrDefaultAsync(e =>
                                e.Id == employeeUser.Id);

                    if (employee is null)
                    {
                        throw new InvalidOperationException(
                            $"Identity user '{employeeEmail}' exists, " +
                            "but its Employee record does not exist.");
                    }

                    employee.DepartmentId = department.Id;
                    employee.ManagerId = managerEmployee.Id;
                    employee.IsActive = true;

                    await context.SaveChangesAsync();

                    await EnsureRoleAsync(
                        userManager,
                        employeeUser,
                        EmployeeRole);

                    continue;
                }

                employeeUser = new ApplicationUser
                {
                    UserName = employeeEmail,
                    Email = employeeEmail,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false
                };

                var employeeResult =
                    await userManager.CreateAsync(
                        employeeUser,
                        "Employee@12345");

                if (!employeeResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        employeeResult.Errors.Select(
                            e => e.Description));

                    throw new InvalidOperationException(
                        $"Failed to create employee " +
                        $"'{employeeEmail}': {errors}");
                }

                employee = new Employee
                {
                    Id = employeeUser.Id,
                    EmployeeCode =
                        $"EMP-{codePrefix}-{definition.Number:000}",
                    FirstName = definition.FirstName,
                    LastName = definition.LastName,
                    Email = employeeEmail,
                    DepartmentId = department.Id,
                    ManagerId = managerEmployee.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                context.Employees.Add(employee);

                await context.SaveChangesAsync();

                await EnsureRoleAsync(
                    userManager,
                    employeeUser,
                    EmployeeRole);
            }
        }

        private static async Task EnsureRoleAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string role)
        {
            if (await userManager.IsInRoleAsync(user, role))
            {
                return;
            }

            var result =
                await userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to assign role '{role}' " +
                    $"to user '{user.Email}': {errors}");
            }
        }
    }
}