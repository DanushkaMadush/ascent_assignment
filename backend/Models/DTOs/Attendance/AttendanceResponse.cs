namespace backend.Models.DTOs.Attendance
{
    public class AttendanceResponse
    {
        public int Id { get; set; }

        public string EmployeeId { get; set; } = null!;

        public string EmployeeCode { get; set; } = null!;

        public string EmployeeName { get; set; } = null!;

        public DateTime CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public DateTime WorkDate { get; set; }

        public string? Status { get; set; }

        public string? DeviceType { get; set; }
    }
}
