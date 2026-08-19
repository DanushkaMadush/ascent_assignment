using backend.Common;
using backend.Models.DTOs.Auth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<TokenResponse>>> Login(
            [FromBody] LoginRequest request)
        {
            var response =
                await _authService.LoginAsync(request);

            if (!response.Success)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse<TokenResponse>>> Refresh(
            [FromBody] RefreshTokenRequest request)
        {
            var response =
                await _authService.RefreshTokenAsync(request);

            if (!response.Success)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
    }
}
