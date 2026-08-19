using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Employee
{
    public class UpdateEmployeeRequest
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [MaxLength(30)]
        public string? PhoneNumber { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public string? ManagerId { get; set; }

        [Required]
        public string Role { get; set; } = "Employee";

        public bool IsActive { get; set; } = true;
    }
}
