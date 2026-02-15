using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Common.DTOs.Library.Users;
using LibrarySystem.Common.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "UserManage")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.ListUsers();

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Users retrieved successfully",
                Data = users
            });
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(
            [FromBody] UserSearchDto dto,
            [FromQuery] string? status = null,
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null)
        {
            // Query string takes precedence (more reliable through gateway / POST)
            if (!string.IsNullOrWhiteSpace(status))
                dto.Status = status.Trim();
            if (page.HasValue && page.Value > 0)
                dto.Page = page.Value;
            if (pageSize.HasValue && pageSize.Value > 0)
                dto.PageSize = pageSize.Value;
            var result = await _service.Search(dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Users retrieved successfully",
                Data = result
            });
        }

        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> GetUserDetails(int id)
        {
            var user = await _service.GetUserDetails(id);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "User details fetched successfully",
                Data = user
            });
        }
    }
}
