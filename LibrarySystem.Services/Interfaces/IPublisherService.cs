
using LibrarySystem.Common.DTOs.Library.Publishers;

namespace LibrarySystem.Services.Interfaces
{
    public interface IPublisherService
    {
        Task AddPublisher(PublisherCreateDto dto);
        Task EditPublisher(int id, PublisherUpdateDto dto);
        Task DeletePublisher(int id);
        Task<List<PublisherListDto>> ListPublishers();
        Task<List<PublisherListDto>> Search(PublisherSearchDto dto);
    }
}
