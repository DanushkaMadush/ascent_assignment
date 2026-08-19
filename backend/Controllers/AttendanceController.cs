using backend.Common;
using backend.Models.DTOs.Attendance;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/v1/attendance")]
    [Authorize(Roles = "Admin,Employee")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(
            IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost("check-in")]
        public async Task<ActionResult<
            ApiResponse<AttendanceResponse>>> CheckIn(
            [FromBody] CheckInRequest request)
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
                await _attendanceService.CheckInAsync(
                    request,
                    userId,
                    isAdmin);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("check-out")]
        public async Task<ActionResult<
            ApiResponse<AttendanceResponse>>> CheckOut(
            [FromBody] CheckOutRequest request)
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
                await _attendanceService.CheckOutAsync(
                    request,
                    userId,
                    isAdmin);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<
            ApiResponse<PagedResponse<AttendanceResponse>>>> GetAll(
            [FromQuery] PaginationRequest pagination,
            [FromQuery] string? employeeId,
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
                await _attendanceService.GetAllAsync(
                    pagination,
                    employeeId,
                    startDate,
                    endDate,
                    userId,
                    isAdmin);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<
            ApiResponse<AttendanceResponse>>> GetById(
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
                await _attendanceService.GetByIdAsync(
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<
            ApiResponse<AttendanceResponse>>> Update(
            int id,
            [FromBody] UpdateAttendanceRequest request)
        {
            var response =
                await _attendanceService.UpdateAsync(
                    id,
                    request);

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
