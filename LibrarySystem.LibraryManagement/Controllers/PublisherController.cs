using LibrarySystem.API.Helpers;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Common.DTOs.Library.Publishers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PublisherController : ControllerBase
    {
        private readonly IPublisherService _service;

        public PublisherController(IPublisherService service)
        {
            _service = service;
        }

        [Authorize(Policy = "CategoryManage")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PublisherCreateDto dto)
        {
            var validation = ValidationHelper.ValidateDto(dto);
            if (!validation.IsValid)
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });

            await _service.AddPublisher(dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Publisher added successfully"
            });
        }

        [Authorize(Policy = "BookView")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var publishers = await _service.ListPublishers();

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Publishers retrieved successfully",
                Data = publishers
            });
        }

        [Authorize(Policy = "CategoryManage")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] PublisherUpdateDto dto)
        {
            var validation = ValidationHelper.ValidateDto(dto);
            if (!validation.IsValid)
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });

            await _service.EditPublisher(id, dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Publisher updated successfully"
            });
        }

        [Authorize(Policy = "CategoryManage")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeletePublisher(id);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Publisher deleted successfully"
            });
        }
    }
}
