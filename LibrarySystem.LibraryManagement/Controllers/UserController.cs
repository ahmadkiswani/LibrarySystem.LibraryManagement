using LibrarySystem.API.Helpers;
using LibrarySystem.Common.DTOs.Library.Helpers;
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
