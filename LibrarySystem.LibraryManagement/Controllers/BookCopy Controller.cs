using LibrarySystem.API.Helpers;
using LibrarySystem.Common.DTOs.Library.BookCopies;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookCopyController : ControllerBase
    {
        private readonly IBookCopyService _service;

        public BookCopyController(IBookCopyService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddCopy([FromBody] BookCopyCreateDto dto)
        {
            var validation = ValidationHelper.ValidateDto( dto);
            if (!validation.IsValid)
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });

            try
            {
                await _service.AddBookCopy(dto);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Copy added successfully"
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCopy(int id)
        {
            try
            {
                var copy = await _service.GetSpecificCopy(id);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Copy fetched successfully",
                    Data = copy
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
                await _service.DeleteBookCopy(id);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Copy deleted successfully"
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

        [HttpGet("total/{bookId}")]
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

        [HttpGet("available/{bookId}")]
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

        [HttpGet("borrowed/{bookId}")]
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
