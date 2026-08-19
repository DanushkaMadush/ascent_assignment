namespace backend.Models.DTOs.Leave
{
    public class LeaveResponse
    {
        public int Id { get; set; }

        public string EmployeeId { get; set; } = null!;

        public string EmployeeName { get; set; } = null!;

        public string LeaveType { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Reason { get; set; }

        public string Status { get; set; } = null!;

        public string? ApprovedById { get; set; }

        public string? ApprovedByName { get; set; }

        public DateTime AppliedOn { get; set; }
    }
}
