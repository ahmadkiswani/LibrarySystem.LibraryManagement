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
    }
}
