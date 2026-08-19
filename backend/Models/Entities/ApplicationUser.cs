using Microsoft.AspNetCore.Identity;

namespace backend.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Employee? Employee { get; set; }
    }
}
