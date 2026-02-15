using LibrarySystem.Common.DTOs.Library.Categories;
using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Common.Events;
using LibrarySystem.Common.Helpers;
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

        public async Task<PagedResultDto<CategoryListDto>> Search(CategorySearchDto dto)
        {
            int page = dto.Page <= 0 ? 1 : dto.Page;
            int pageSize = dto.PageSize <= 0 || dto.PageSize > 200 ? 10 : dto.PageSize;
            AppHelper.NormalizePage(ref page, ref pageSize, 10, 200);

            var categories = await _categoryRepo.SearchAsync(
                dto.Text,
                dto.Number,
                page,
                pageSize);

            var totalCount = await _categoryRepo.CountForSearchAsync(dto.Text, dto.Number);
            var info = AppHelper.BuildPagingInfo(totalCount, page, pageSize);

            return new PagedResultDto<CategoryListDto>
            {
                Data = categories.Select(c => new CategoryListDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList(),
                TotalCount = (int)info.TotalCount,
                Page = info.Page,
                PageSize = info.PageSize,
                TotalPages = info.TotalPages,
                HasNextPage = info.HasNextPage
            };
        }
    }
}
