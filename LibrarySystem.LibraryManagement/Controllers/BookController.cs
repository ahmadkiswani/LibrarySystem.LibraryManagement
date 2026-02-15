using LibrarySystem.Common.Helpers;
using LibrarySystem.Common.DTOs.Library.Books;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly IBookService _service;

        public BookController(IBookService service)
        {
            _service = service;
        }

        [Authorize(Policy = "BookCreate")]
        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] BookCreateDto dto)
        {
            var validation = AppHelper.ValidateDto(dto);
            if (!validation.IsValid)
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });

            var id = await _service.AddBook(dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Book added successfully",
                Data = new { BookId = id }
            });
        }

        [Authorize(Policy = "BookView")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _service.GetAllBooks();

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Books fetched successfully",
                Data = books
            });
        }

        [Authorize(Policy = "BookView")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _service.GetBookById(id);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Book fetched successfully",
                Data = book
            });
        }

        [Authorize(Policy = "BookView")]
        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var result = await _service.GetBookDetails(id);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Book details fetched successfully",
                Data = result
            });
        }

        [Authorize(Policy = "BookUpdate")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] BookUpdateDto dto)
        {
            var validation = AppHelper.ValidateDto(dto);
            if (!validation.IsValid)
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });

            await _service.EditBook(id, dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Book updated successfully"
            });
        }

        [Authorize(Policy = "BookDelete")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteBook(id);
                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Book deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Policy = "BookView")]
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] BookSearchDto dto)
        {
            var validation = AppHelper.ValidateDto(dto);
            if (!validation.IsValid)
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });

            var result = await _service.SearchBooks(dto);

            return Ok(new BaseResponse<BookSearchResultDto>
            {
                Success = true,
                Message = "Books search result",
                Data = result
            });
        }
    }
}
