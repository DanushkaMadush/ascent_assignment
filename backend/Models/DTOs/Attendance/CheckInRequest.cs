using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Attendance
{
    public class CheckInRequest
    {
        public string? EmployeeId { get; set; }

        [MaxLength(30)]
        public string? DeviceType { get; set; }
    }
}
