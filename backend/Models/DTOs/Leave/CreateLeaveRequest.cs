using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Leave
{
    public class CreateLeaveRequest
    {
        [Required]
        public string LeaveType { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [MaxLength(1000)]
        public string? Reason { get; set; }
    }
}
