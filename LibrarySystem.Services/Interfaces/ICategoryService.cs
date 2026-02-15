using LibrarySystem.Common.DTOs.Library.Categories;
using LibrarySystem.Common.DTOs.Library.Helpers;

namespace LibrarySystem.Services.Interfaces
{
    public interface ICategoryService
    {
        Task AddCategory(CategoryCreateDto dto);
        Task EditCategory(int id, CategoryUpdateDto dto);
        Task DeleteCategory(int id);
        Task<List<CategoryListDto>> ListCategories();
        Task<PagedResultDto<CategoryListDto>> Search(CategorySearchDto dto);
    }
}
