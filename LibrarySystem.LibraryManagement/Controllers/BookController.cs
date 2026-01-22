using LibrarySystem.API.Helpers;
using LibrarySystem.Common.DTOs.Library.Books;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var validation = ValidationHelper.ValidateDto(dto);
            if (!validation.IsValid)
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });
            try
            {
                // Provide a value for the required 'id' parameter.
                // If you have a specific logic for this id, replace '0' with the appropriate value.
                var id = await _service.AddBook(dto);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Book added successfully",
                    Data = new { BookId = id }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                });
            }
        }
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var book = await _service.GetBookById(id);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Book fetched successfully",
                    Data = book
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
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            try
            {
                var result = await _service.GetBookDetails(id);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Book details fetched successfully",
                    Data = result
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
        public async Task<IActionResult> Edit(int id, [FromBody] BookUpdateDto dto)
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
                await _service.EditBook(id, dto);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Book updated successfully"
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
                await _service.DeleteBook(id);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Book deleted successfully"
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
        [HttpPost("search")]

        public async Task<IActionResult> Search([FromBody] BookSearchDto dto)
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

            var result = await _service.SearchBooks(dto);

            var errors = new List<string>();
            if (result == null || result.Count == 0)
            {
                errors.Add("No books found with the given Data.");
            }

            return Ok(new BaseResponse<object>
            {
                Success = errors.Count == 0,
                Message = errors.Count == 0 ? "Books search result" : "Search returned no results",
                Data = result,
                Errors = errors
            });
        }
    }

};