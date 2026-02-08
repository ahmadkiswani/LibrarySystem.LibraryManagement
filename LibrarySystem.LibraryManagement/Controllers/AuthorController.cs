using LibrarySystem.API.Helpers;
using LibrarySystem.Common.DTOs.Library.Authors;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _service;
        private readonly ICurrentUserContext _currentUser;

        public AuthorController(
            IAuthorService service,
            ICurrentUserContext currentUser)
        {
            _service = service;
            _currentUser = currentUser;
        }

        [Authorize(Policy = "BookView")]
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] AuthorSearchDto dto)
        {
            var authors = await _service.Search(dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Authors fetched successfully",
                Data = authors
            });
        }

        [Authorize(Policy = "BookCreate")]
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

            await _service.AddAuthor(_currentUser.LocalUserId, dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Author added successfully"
            });
        }

        [Authorize(Policy = "BookView")]
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

        [Authorize(Policy = "BookView")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var author = await _service.GetAuthorById(id);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Author fetched successfully",
                Data = author
            });
        }

        [Authorize(Policy = "BookUpdate")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] AuthorUpdateDto dto)
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

            await _service.EditAuthor(_currentUser.LocalUserId, id, dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Author updated successfully"
            });
        }

        [Authorize(Policy = "BookDelete")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAuthor(_currentUser.LocalUserId, id);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Author deleted successfully"
            });
        }

       
    }
}
