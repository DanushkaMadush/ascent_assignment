namespace backend.Models.Entities
{
    public class Attendance
    {
        public int Id { get; set; }

        public string EmployeeId { get; set; } = null!;

        public DateTime CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public DateTime WorkDate { get; set; }

        public string? Status { get; set; }

        public string? DeviceType { get; set; }

        public Employee Employee { get; set; } = null!;
    }
}
