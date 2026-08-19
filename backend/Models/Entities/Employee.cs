namespace backend.Models.Entities
{
    public class Employee
    {
        public string Id { get; set; } = null!;

        public string EmployeeCode { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public int DepartmentId { get; set; }

        public string? ManagerId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedBy { get; set; }

        public ApplicationUser ApplicationUser { get; set; } = null!;

        public Department Department { get; set; } = null!;

        public Employee? Manager { get; set; }

        public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        public ICollection<Leave> LeaveRequests { get; set; } = new List<Leave>();
    }
}
