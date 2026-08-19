using backend.Models.Entities;

namespace backend.Security
{
    public interface IJwtService
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser user);

        string GenerateRefreshToken();

        string HashRefreshToken(string refreshToken);

        DateTime GetAccessTokenExpiry();
    }
}
