using LibrarySystem.API.Helpers;
using LibrarySystem.Common.DTOs.Library.BookCopies;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookCopyController : ControllerBase
    {
        private readonly IBookCopyService _service;

        public BookCopyController(IBookCopyService service)
        {
            _service = service;
        }

        [Authorize(Policy = "BookCreate")]
        [HttpPost]
        public async Task<IActionResult> AddCopy([FromBody] BookCopyCreateDto dto)
        {
            var validation = ValidationHelper.ValidateDto(dto);
            if (!validation.IsValid)
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });

            await _service.AddBookCopy(dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Copy added successfully"
            });
        }

        [Authorize(Policy = "BookView")]
        [HttpGet("by-book/{bookId:int}")]
        public async Task<IActionResult> GetByBookId(int bookId)
        {
            var copies = await _service.GetCopiesByBookIdAsync(bookId);
            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Copies fetched",
                Data = copies
            });
        }

        [Authorize(Policy = "BookView")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.ListBookCopies();

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Copies fetched successfully",
                Data = result
            });
        }

        [Authorize(Policy = "BookView")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCopy(int id)
        {
            var copy = await _service.GetSpecificCopy(id);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Copy fetched successfully",
                Data = copy
            });
        }

        [Authorize(Policy = "BookDelete")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteBookCopy(id);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Copy deleted successfully"
            });
        }

        [Authorize(Policy = "BookView")]
        [HttpGet("total/{bookId:int}")]
        public async Task<IActionResult> GetTotalCopies(int bookId)
        {
            var count = await _service.GetAllCopiesCount(bookId);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Total copies fetched",
                Data = count
            });
        }

        [Authorize(Policy = "BookView")]
        [HttpGet("available/{bookId:int}")]
        public async Task<IActionResult> GetAvailableCopies(int bookId)
        {
            var count = await _service.GetAvailableCount(bookId);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Available copies fetched",
                Data = count
            });
        }

        [Authorize(Policy = "BookView")]
        [HttpGet("borrowed/{bookId:int}")]
        public async Task<IActionResult> GetBorrowedCopies(int bookId)
        {
            var count = await _service.GetBorrowedCount(bookId);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Borrowed copies fetched",
                Data = count
            });
        }
    }
}
