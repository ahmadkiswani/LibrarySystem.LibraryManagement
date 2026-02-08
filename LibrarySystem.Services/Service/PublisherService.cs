using LibrarySystem.Common.DTOs.Library.Publishers;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Services.Interfaces;
using System.Linq;

namespace LibrarySystem.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IPublisherRepository _publisherRepo;

        public PublisherService(IPublisherRepository publisherRepo)
        {
            _publisherRepo = publisherRepo;
        }

        public Task AddPublisher(PublisherCreateDto dto)
            => _publisherRepo.AddPublisherAsync(dto);

        public Task EditPublisher(int id, PublisherUpdateDto dto)
            => _publisherRepo.UpdatePublisherAsync(id, dto);

        public Task DeletePublisher(int id)
            => _publisherRepo.SoftDeleteByIdAsync(id);

        public Task<List<PublisherListDto>> ListPublishers()
            => _publisherRepo.GetAllListAsync();

        public async Task<List<PublisherListDto>> Search(PublisherSearchDto dto)
        {
            int page = dto.Page <= 0 ? 1 : dto.Page;
            int pageSize = dto.PageSize <= 0 || dto.PageSize > 200 ? 10 : dto.PageSize;

            var publishers = await _publisherRepo.SearchAsync(
                dto.Text,
                dto.Number,
                page,
                pageSize);

            return publishers
                .Select(p => new PublisherListDto
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToList();
        }
    }
}
