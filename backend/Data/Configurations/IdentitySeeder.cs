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

            await SeedAdminAsync(
                userManager,
                context);
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
                if (!await roleManager.RoleExistsAsync(role))
                {
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
        }

        private static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            AppDbContext context)
        {
            const string adminEmail = "admin@ascentems.com";
            const string adminPassword = "Admin@12345";

            var adminUser = await userManager.FindByEmailAsync(
                adminEmail);

            if (adminUser is not null)
            {
                if (!await userManager.IsInRoleAsync(
                        adminUser,
                        AdminRole))
                {
                    var roleResult =
                        await userManager.AddToRoleAsync(
                            adminUser,
                            AdminRole);

                    if (!roleResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            "Failed to assign Admin role.");
                    }
                }

                return;
            }

            // Find a department for the seeded Admin.
            var department = await context.Departments
                .FirstOrDefaultAsync(d => d.Name == "Administration");

            if (department is null)
            {
                department = new Department
                {
                    Name = "Administration",
                    Description = "Administration department"
                };

                context.Departments.Add(department);

                await context.SaveChangesAsync();
            }

            // Create Identity user first.
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

            // Create Employee using the SAME Identity Id.
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

            // Assign Admin role.
            var roleAssignmentResult =
                await userManager.AddToRoleAsync(
                    adminUser,
                    AdminRole);

            if (!roleAssignmentResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleAssignmentResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to assign Admin role: {errors}");
            }
        }
    }
}
