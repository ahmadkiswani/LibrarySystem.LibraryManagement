using LibrarySystem.API.Helpers;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
     
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
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetUserDetails(int id)
        {
            try
            {
                var user = await _service.GetUserDetails(id);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "User details fetched successfully",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return NotFound(new BaseResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }


       

     

    }
}
