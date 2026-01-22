using LibrarySystem.API.Helpers;
using LibrarySystem.Common.DTOs.Library.Borrows;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowController : ControllerBase
    {
        private readonly IBorrowService _service;

        public BorrowController(IBorrowService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBook([FromBody] BorrowCreateDto dto)
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
                await _service.BorrowBook(dto);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Book borrowed successfully"
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

        [HttpPut("return")]
        public async Task<IActionResult> ReturnBook([FromBody] BorrowReturnDto dto)
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
                await _service.ReturnBook(dto);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Book returned successfully"
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

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] BorrowSearchDto dto)
        {
            var result = await _service.Search(dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Borrow search completed",
                Data = result
            });
        }
    }
}
