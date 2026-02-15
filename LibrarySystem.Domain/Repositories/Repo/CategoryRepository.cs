using LibrarySystem.Common.DTOs.Library.Categories;
using LibrarySystem.Common.Repositories;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IRepository<Category> _repo;

        public CategoryRepository(IRepository<Category> repo)
        {
            _repo = repo;
        }

        public async Task AddCategoryAsync(CategoryCreateDto dto)
        {
            if (await ExistsByNameAsync(dto.Name))
                throw new Exception("Category already exists");

            await _repo.AddAsync(new Category { Name = dto.Name });
            await _repo.SaveAsync();
        }

        public async Task UpdateCategoryAsync(int id, CategoryUpdateDto dto)
        {
            var category = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Category not found");

            category.Name = dto.Name;
            await _repo.UpdateAsync(category);
            await _repo.SaveAsync();
        }

        public async Task SoftDeleteByIdAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Category not found");

            _repo.SoftDelete(category);
            await _repo.SaveAsync();
        }

        public Task<List<CategoryListDto>> GetAllListAsync()
            => _repo.GetQueryable()
                .AsNoTracking()
                .Select(c => new CategoryListDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

        public Task<bool> ExistsByNameAsync(string name)
            => _repo.GetQueryable().AnyAsync(c => c.Name == name);

        public async Task<Category> GetRequiredByIdAsync(int id)
            => await _repo.GetByIdAsync(id)
                ?? throw new Exception("Category not found");

        public Task<bool> ExistsAsync(int id)
            => _repo.GetQueryable().AnyAsync(c => c.Id == id);

        public async Task<List<Category>> SearchAsync(
            string? text,
            int? number,
            int page,
            int pageSize)
        {
            var query = _repo
                .GetQueryable()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(text))
                query = query.Where(c => c.Name.Contains(text));

            if (number.HasValue)
                query = query.Where(c => c.Id == number.Value);

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountForSearchAsync(string? text, int? number)
        {
            var query = _repo.GetQueryable().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(text))
                query = query.Where(c => c.Name.Contains(text));

            if (number.HasValue)
                query = query.Where(c => c.Id == number.Value);

            return await query.CountAsync();
        }
    }
}
