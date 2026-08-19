namespace backend.Models.Entities
{
    public class Leave
    {
        public int Id { get; set; }

        public string EmployeeId { get; set; } = null!;

        public string LeaveType { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Reason { get; set; }

        public string Status { get; set; } = "Pending";

        public string? ApprovedById { get; set; }

        public DateTime AppliedOn { get; set; } = DateTime.UtcNow;

        public Employee Employee { get; set; } = null!;

        public Employee? ApprovedBy { get; set; }
    }
}
