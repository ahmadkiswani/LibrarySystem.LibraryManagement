using LibrarySystem.Common.DTOs.Library.Publishers;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Services.Interfaces;

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
    }
}
