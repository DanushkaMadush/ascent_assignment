using backend.Common;
using backend.Models.DTOs.Leave;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/v1/leave")]
    [Authorize(Roles = "Admin,Manager,Employee")]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveController(
            ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<ActionResult<
            ApiResponse<LeaveResponse>>> Create(
            [FromBody] CreateLeaveRequest request)
        {
            var userId = GetAuthenticatedUserId();

            if (userId is null)
            {
                return Unauthorized(
                    ApiResponse<object>.FailureResponse(
                        "Authenticated user ID not found."));
            }

            var response =
                await _leaveService.CreateAsync(
                    request,
                    userId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = response.Data!.Id },
                response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<ActionResult<
            ApiResponse<PagedResponse<LeaveResponse>>>> GetAll(
            [FromQuery] PaginationRequest pagination,
            [FromQuery] string? employeeId,
            [FromQuery] string? status,
            [FromQuery] string? leaveType,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var userId = GetAuthenticatedUserId();

            if (userId is null)
            {
                return Unauthorized(
                    ApiResponse<object>.FailureResponse(
                        "Authenticated user ID not found."));
            }

            var isAdmin = User.IsInRole("Admin");

            var response =
                await _leaveService.GetAllAsync(
                    pagination,
                    employeeId,
                    status,
                    leaveType,
                    startDate,
                    endDate,
                    userId,
                    isAdmin);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<ActionResult<
            ApiResponse<LeaveResponse>>> GetById(
            int id)
        {
            var userId = GetAuthenticatedUserId();

            if (userId is null)
            {
                return Unauthorized(
                    ApiResponse<object>.FailureResponse(
                        "Authenticated user ID not found."));
            }

            var isAdmin = User.IsInRole("Admin");

            var response =
                await _leaveService.GetByIdAsync(
                    id,
                    userId,
                    isAdmin);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<ActionResult<
            ApiResponse<LeaveResponse>>> Update(
            int id,
            [FromBody] UpdateLeaveRequest request)
        {
            var userId = GetAuthenticatedUserId();

            if (userId is null)
            {
                return Unauthorized(
                    ApiResponse<object>.FailureResponse(
                        "Authenticated user ID not found."));
            }

            var isAdmin = User.IsInRole("Admin");

            var response =
                await _leaveService.UpdateAsync(
                    id,
                    request,
                    userId,
                    isAdmin);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<ActionResult<
            ApiResponse<LeaveResponse>>> Cancel(
            int id)
        {
            var userId = GetAuthenticatedUserId();

            if (userId is null)
            {
                return Unauthorized(
                    ApiResponse<object>.FailureResponse(
                        "Authenticated user ID not found."));
            }

            var isAdmin = User.IsInRole("Admin");

            var response =
                await _leaveService.CancelAsync(
                    id,
                    userId,
                    isAdmin);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<
            ApiResponse<LeaveResponse>>> Approve(
            int id)
        {
            var userId = GetAuthenticatedUserId();

            if (userId is null)
            {
                return Unauthorized(
                    ApiResponse<object>.FailureResponse(
                        "Authenticated user ID not found."));
            }

            var isAdmin = User.IsInRole("Admin");

            var response =
                await _leaveService.ApproveAsync(
                    id,
                    userId,
                    isAdmin);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPatch("{id}/reject")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<
            ApiResponse<LeaveResponse>>> Reject(
            int id)
        {
            var userId = GetAuthenticatedUserId();

            if (userId is null)
            {
                return Unauthorized(
                    ApiResponse<object>.FailureResponse(
                        "Authenticated user ID not found."));
            }

            var isAdmin = User.IsInRole("Admin");

            var response =
                await _leaveService.RejectAsync(
                    id,
                    userId,
                    isAdmin);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        private string? GetAuthenticatedUserId()
        {
            return User.FindFirstValue(
                ClaimTypes.NameIdentifier);
        }
    }
}
