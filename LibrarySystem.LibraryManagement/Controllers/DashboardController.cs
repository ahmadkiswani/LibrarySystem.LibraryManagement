using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IBorrowService _borrowService;

        public DashboardController(IBorrowService borrowService)
        {
            _borrowService = borrowService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var stats = await _borrowService.GetDashboardStatsAsync();
            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Dashboard overview",
                Data = stats
            });
        }
    }
}
