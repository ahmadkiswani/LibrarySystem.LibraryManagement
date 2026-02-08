using LibrarySystem.Common.DTOs.Library.Categories;
using LibrarySystem.Common.Events;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Services.Interfaces;
using MassTransit;

namespace LibrarySystem.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IPublishEndpoint _publish;

        public CategoryService(
            ICategoryRepository categoryRepo,
            IPublishEndpoint publish)
        {
            _categoryRepo = categoryRepo;
            _publish = publish;
        }

        public async Task AddCategory(CategoryCreateDto dto)
        {
            await _categoryRepo.AddCategoryAsync(dto);

            await _publish.Publish(new CategoryUpsertedEvent
            {
                EventId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow,
                CategoryName = dto.Name
            }, ctx => ctx.SetRoutingKey("category.upserted"));
        }

        public async Task EditCategory(int id, CategoryUpdateDto dto)
        {
            var category = await _categoryRepo.GetRequiredByIdAsync(id);

            await _categoryRepo.UpdateCategoryAsync(id, dto);

            await _publish.Publish(new CategoryUpsertedEvent
            {
                EventId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow,
                CategoryId = category.Id,
                CategoryName = dto.Name
            }, ctx => ctx.SetRoutingKey("category.upserted"));
        }

        public Task DeleteCategory(int id)
            => _categoryRepo.SoftDeleteByIdAsync(id);

        public Task<List<CategoryListDto>> ListCategories()
            => _categoryRepo.GetAllListAsync();

        public async Task<List<CategoryListDto>> Search(CategorySearchDto dto)
        {
            int page = dto.Page <= 0 ? 1 : dto.Page;
            int pageSize = dto.PageSize <= 0 || dto.PageSize > 200 ? 10 : dto.PageSize;

            var categories = await _categoryRepo.SearchAsync(
                dto.Text,
                dto.Number,
                page,
                pageSize);

            return categories
                .Select(c => new CategoryListDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();
        }
    }
}
