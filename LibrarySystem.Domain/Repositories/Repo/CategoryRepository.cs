using LibrarySystem.Common.DTOs.Library.Categories;
using LibrarySystem.Domain.Data;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class CategoryRepository
        : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(LibraryDbContext context)
            : base(context)
        {
        }

        public async Task AddCategoryAsync(CategoryCreateDto dto)
        {
            if (await ExistsByNameAsync(dto.Name))
                throw new Exception("Category already exists");

            var category = new Category
            {
                Name = dto.Name
            };

            await AddAsync(category);
        }

        public async Task UpdateCategoryAsync(int id, CategoryUpdateDto dto)
        {
            var category = await GetByIdAsync(id)
                ?? throw new Exception("Category not found");

            category.Name = dto.Name;
            await UpdateAsync(category);
        }

        public async Task SoftDeleteByIdAsync(int id)
        {
            var category = await GetByIdAsync(id)
                ?? throw new Exception("Category not found");

            await SoftDeleteAsync(category);
        }

        public async Task<List<CategoryListDto>> GetAllListAsync()
        {
            return await GetQueryable()
                .Select(c => new CategoryListDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<Category> GetRequiredByIdAsync(int id)
        {
            return await GetByIdAsync(id)
                ?? throw new Exception("Category not found");
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(c => c.Name == name);
        }
    }
}
