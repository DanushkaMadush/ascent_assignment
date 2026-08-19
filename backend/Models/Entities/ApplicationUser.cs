using Microsoft.AspNetCore.Identity;

namespace backend.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Employee? Employee { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
