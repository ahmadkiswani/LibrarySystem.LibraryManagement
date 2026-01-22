using LibrarySystem.API.Helpers;
using LibrarySystem.Common.DTOs.Library.Authors;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _service;

        public AuthorController(IAuthorService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorCreateDto dto)
        {
            var validation = ValidationHelper.ValidateDto(dto);
            if (!validation.IsValid)
            {
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });
            }

            // You must provide an int id as the second argument.
            // If the id should be generated or is not available from the client, you need to clarify how to obtain it.
            // For now, passing 0 as a placeholder. Replace with actual logic as needed.
            await _service.AddAuthor(dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Author added successfully"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _service.GetAllAuthors();

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Authors fetched successfully",
                Data = authors
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var author = await _service.GetAuthorById(id);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Author fetched successfully",
                    Data = author
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] AuthorUpdateDto dto)
        {
            var validation = ValidationHelper.ValidateDto( dto);

            if (!validation.IsValid)
            {
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });
            }

            try
            {
                await _service.EditAuthor(id, dto);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Author updated successfully"
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

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAuthor(id);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Author deleted successfully"
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
        [Authorize]
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            var userId = int.Parse( User.FindFirstValue(ClaimTypes.NameIdentifier));


            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new
            {
                userId,
                roles
            });
        }
        [Authorize]
        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            return Ok(User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            }));
        }

    }

}
