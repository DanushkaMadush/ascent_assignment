using backend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace backend.Security
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtService(
            IOptions<JwtSettings> settings,
            UserManager<ApplicationUser> userManager)
        {
            _settings = settings.Value;
            _userManager = userManager;
        }

        public async Task<string> GenerateAccessTokenAsync(
            ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            if (user.Employee is not null)
            {
                claims.Add(
                    new Claim("employeeId", user.Employee.Id));

                claims.Add(
                    new Claim("employeeCode", user.Employee.EmployeeCode));

                claims.Add(
                    new Claim(
                        "firstName",
                        user.Employee.FirstName));

                claims.Add(
                    new Claim(
                        "lastName",
                        user.Employee.LastName));
            }

            foreach (var role in roles)
            {
                claims.Add(
                    new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddMinutes(
                _settings.ExpiresInMinutes);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }

        public string HashRefreshToken(string refreshToken)
        {
            var bytes = SHA256.HashData(
                Encoding.UTF8.GetBytes(refreshToken));

            return Convert.ToBase64String(bytes);
        }

        public DateTime GetAccessTokenExpiry()
        {
            return DateTime.UtcNow.AddMinutes(
                _settings.ExpiresInMinutes);
        }
    }
}
