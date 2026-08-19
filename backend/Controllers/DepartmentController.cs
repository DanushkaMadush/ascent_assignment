using backend.Common;
using backend.Models.DTOs.Department;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/v1/department")]
    [Authorize(Roles = "Admin")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(
            IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpPost]
        public async Task<ActionResult<
            ApiResponse<DepartmentResponse>>> Create(
            [FromBody] CreateDepartmentRequest request)
        {
            var response =
                await _departmentService.CreateAsync(request);

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
        public async Task<ActionResult<
            ApiResponse<PagedResponse<DepartmentResponse>>>> GetAll(
            [FromQuery] PaginationRequest pagination,
            [FromQuery] string? search)
        {
            var response =
                await _departmentService.GetAllAsync(
                    pagination,
                    search);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<
            ApiResponse<DepartmentResponse>>> GetById(
            int id)
        {
            var response =
                await _departmentService.GetByIdAsync(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<
            ApiResponse<DepartmentResponse>>> Update(
            int id,
            [FromBody] UpdateDepartmentRequest request)
        {
            var response =
                await _departmentService.UpdateAsync(
                    id,
                    request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
