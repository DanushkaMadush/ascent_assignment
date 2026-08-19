using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Department
{
    public class CreateDepartmentRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
