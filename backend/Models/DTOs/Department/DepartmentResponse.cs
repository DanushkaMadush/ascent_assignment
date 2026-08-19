namespace backend.Models.DTOs.Department
{
    public class DepartmentResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int EmployeeCount { get; set; }
    }
}
