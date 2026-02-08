using LibrarySystem.API.Helpers;
using LibrarySystem.API.Models;
using LibrarySystem.Common.DTOs.Library.Borrows;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BorrowController : ControllerBase
    {
        private readonly IBorrowService _service;
        private readonly ICurrentUserContext _currentUser;

        public BorrowController(
            IBorrowService service,
            ICurrentUserContext currentUser)
        {
            _service = service;
            _currentUser = currentUser;
        }

        [Authorize(Policy = "BorrowCreate")]
        [HttpPost]
        public async Task<IActionResult> BorrowBook([FromBody] BorrowCreateDto dto)
        {
            var validation = ValidationHelper.ValidateDto(dto);
            if (!validation.IsValid)
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });

            await _service.BorrowBook(_currentUser.ExternalUserId, dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Borrow request submitted"
            });
        }

        [Authorize(Policy = "BorrowCreate")]
        [HttpPost("request/{bookId:int}")]
        public async Task<IActionResult> RequestBorrowByBook(int bookId)
        {
            await _service.RequestBorrowByBook(_currentUser.ExternalUserId, bookId);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Borrow request submitted. Waiting for approval."
            });
        }

        [Authorize(Policy = "BorrowView")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBorrows()
        {
            var result = await _service.GetMyBorrows(_currentUser.ExternalUserId);
            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "My borrows",
                Data = result
            });
        }

        [Authorize(Policy = "BorrowView")]
        [HttpGet("my-pending")]
        public async Task<IActionResult> GetMyPendingBorrows()
        {
            var borrows = await _service.GetMyPendingBorrows(_currentUser.ExternalUserId);
            var list = borrows.Select(b => new PendingBorrowItemDto
            {
                Id = b.Id,
                UserId = b.UserId,
                UserName = b.User?.UserName,
                BookTitle = b.BookCopy?.Book?.Title,
                BookCopyId = b.BookCopyId,
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate
            }).ToList();
            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "My pending requests",
                Data = list
            });
        }

        [Authorize(Policy = "UserManage")]
        [HttpGet("by-user/{externalUserId:int}")]
        public async Task<IActionResult> GetBorrowsByUser(int externalUserId)
        {
            var result = await _service.GetBorrowsByExternalUserId(externalUserId);
            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "User borrows",
                Data = result
            });
        }

        [Authorize(Policy = "BorrowApprove")]
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveBorrows()
        {
            var result = await _service.GetActiveBorrowsForAdmin();
            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Active loans",
                Data = result
            });
        }

        [Authorize(Policy = "BorrowApprove")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingBorrows()
        {
            var borrows = await _service.GetPendingBorrows();
            var list = borrows.Select(b => new PendingBorrowItemDto
            {
                Id = b.Id,
                UserId = b.UserId,
                UserName = b.User?.UserName,
                BookTitle = b.BookCopy?.Book?.Title,
                BookCopyId = b.BookCopyId,
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate
            }).ToList();
            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Pending borrows",
                Data = list
            });
        }

        [Authorize(Policy = "BorrowApprove")]
        [HttpPut("approve/{id:int}")]
        public async Task<IActionResult> ApproveBorrow(int id)
        {
            await _service.ApproveBorrow(id);
            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Borrow approved"
            });
        }

        [Authorize(Policy = "BorrowApprove")]
        [HttpPut("reject/{id:int}")]
        public async Task<IActionResult> RejectBorrow(int id)
        {
            await _service.RejectBorrow(id);
            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Borrow rejected"
            });
        }

        [Authorize(Policy = "BorrowReturn")]
        [HttpPut("return")]
        public async Task<IActionResult> ReturnBook([FromBody] BorrowReturnDto dto)
        {
            var validation = ValidationHelper.ValidateDto(dto);
            if (!validation.IsValid)
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });

            await _service.ReturnBook(dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Book returned successfully"
            });
        }

        [Authorize(Policy = "BorrowView")]
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
