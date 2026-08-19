using backend.Common;
using backend.Models.DTOs.Auth;

namespace backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<TokenResponse>> LoginAsync(LoginRequest request);

        Task<ApiResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    }
}
