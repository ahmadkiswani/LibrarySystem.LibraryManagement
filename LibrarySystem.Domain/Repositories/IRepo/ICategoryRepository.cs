using LibrarySystem.Common.DTOs.Library.Categories;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Repositories.IRepo
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task AddCategoryAsync(CategoryCreateDto dto);
        Task UpdateCategoryAsync(int id, CategoryUpdateDto dto);
        Task SoftDeleteByIdAsync(int id);

        Task<List<CategoryListDto>> GetAllListAsync();
        Task<Category> GetRequiredByIdAsync(int id);

        Task<bool> ExistsByNameAsync(string name);
    }
}
