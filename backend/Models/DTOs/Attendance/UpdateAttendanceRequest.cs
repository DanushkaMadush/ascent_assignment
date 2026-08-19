using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Attendance
{
    public class UpdateAttendanceRequest
    {
        [Required]
        public DateTime CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        [Required]
        public DateTime WorkDate { get; set; }
    }
}
