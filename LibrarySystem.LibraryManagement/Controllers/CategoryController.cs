using LibrarySystem.API.Helpers;
using LibrarySystem.Common.DTOs.Library.Categories;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CategoryCreateDto dto)
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
                await _service.AddCategory(dto);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Category added successfully"
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
            var categories = await _service.ListCategories();

            return Ok(new BaseResponse<object>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Data = categories
            });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] CategoryUpdateDto dto)
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
                await _service.EditCategory(id, dto);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Category updated successfully"
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

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteCategory(id);

                return Ok(new BaseResponse<object>
                {
                    Success = true,
                    Message = "Category deleted successfully"
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
    }
}
