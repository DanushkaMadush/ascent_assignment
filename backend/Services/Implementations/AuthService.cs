using backend.Common;
using backend.Data;
using backend.Models.DTOs.Auth;
using backend.Models.Entities;
using backend.Security;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            AppDbContext context,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<TokenResponse>> LoginAsync(
            LoginRequest request)
        {
            var user = await _userManager.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null)
            {
                return ApiResponse<TokenResponse>.FailureResponse(
                    "Invalid email or password.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(
                user,
                request.Password);

            if (!passwordValid)
            {
                return ApiResponse<TokenResponse>.FailureResponse(
                    "Invalid email or password.");
            }

            var accessToken =
                await _jwtService.GenerateAccessTokenAsync(user);

            var refreshToken =
                _jwtService.GenerateRefreshToken();

            var refreshTokenHash =
                _jwtService.HashRefreshToken(refreshToken);

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);

            await _context.SaveChangesAsync();

            return ApiResponse<TokenResponse>.SuccessResponse(
                new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                },
                "Login successful.");
        }

        public async Task<ApiResponse<TokenResponse>> RefreshTokenAsync(
            RefreshTokenRequest request)
        {
            var refreshTokenHash =
                _jwtService.HashRefreshToken(
                    request.RefreshToken);

            var storedToken = await _context.RefreshTokens
                .Include(r => r.User)
                .ThenInclude(u => u.Employee)
                .FirstOrDefaultAsync(r =>
                    r.Token == refreshTokenHash);

            if (storedToken is null)
            {
                return ApiResponse<TokenResponse>.FailureResponse(
                    "Invalid refresh token.");
            }

            if (!storedToken.IsActive)
            {
                return ApiResponse<TokenResponse>.FailureResponse(
                    "Refresh token is expired or revoked.");
            }

            // Revoke old refresh token
            storedToken.RevokedAt = DateTime.UtcNow;

            // Generate new tokens
            var accessToken =
                await _jwtService.GenerateAccessTokenAsync(
                    storedToken.User);

            var newRefreshToken =
                _jwtService.GenerateRefreshToken();

            var newRefreshTokenHash =
                _jwtService.HashRefreshToken(
                    newRefreshToken);

            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = storedToken.UserId,
                Token = newRefreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(
                newRefreshTokenEntity);

            await _context.SaveChangesAsync();

            return ApiResponse<TokenResponse>.SuccessResponse(
                new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken
                },
                "Token refreshed successfully.");
        }
    }
}
