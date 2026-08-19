using backend.Common;
using backend.Models.DTOs.Employee;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/v1/employee")]
    [Authorize(Roles = "Admin")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(
            IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost]
        public async Task<ActionResult<
            ApiResponse<EmployeeResponse>>> Create(
            [FromBody] CreateEmployeeRequest request)
        {
            var response =
                await _employeeService.CreateAsync(request);

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
            ApiResponse<PagedResponse<EmployeeResponse>>>> GetAll(
            [FromQuery] PaginationRequest pagination,
            [FromQuery] string? search,
            [FromQuery] int? departmentId)
        {
            var response =
                await _employeeService.GetAllAsync(
                    pagination,
                    search,
                    departmentId);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<
            ApiResponse<EmployeeResponse>>> GetById(
            string id)
        {
            var response =
                await _employeeService.GetByIdAsync(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<
            ApiResponse<EmployeeResponse>>> Update(
            string id,
            [FromBody] UpdateEmployeeRequest request)
        {
            var response =
                await _employeeService.UpdateAsync(
                    id,
                    request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<
            ApiResponse<object>>> Delete(
            string id)
        {
            var response =
                await _employeeService.DeleteAsync(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
