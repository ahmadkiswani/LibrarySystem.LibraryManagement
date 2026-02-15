using LibrarySystem.Common.Helpers;
using LibrarySystem.Common.DTOs.Library.Categories;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [Authorize(Policy = "CategoryManage")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CategoryCreateDto dto)
        {
            var validation = AppHelper.ValidateDto(dto);
            if (!validation.IsValid)
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });

            await _service.AddCategory(dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Category added successfully"
            });
        }

        [Authorize(Policy = "BookView")]
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] CategorySearchDto dto)
        {
            var categories = await _service.Search(dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Data = categories
            });
        }

        [Authorize(Policy = "BookView")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _service.ListCategories();

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Data = categories
            });
        }

        [Authorize(Policy = "CategoryManage")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] CategoryUpdateDto dto)
        {
            var validation = AppHelper.ValidateDto(dto);
            if (!validation.IsValid)
                return BadRequest(new BaseResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validation.Errors
                });

            await _service.EditCategory(id, dto);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Category updated successfully"
            });
        }

        [Authorize(Policy = "CategoryManage")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteCategory(id);

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Category deleted successfully"
            });
        }
    }
}
