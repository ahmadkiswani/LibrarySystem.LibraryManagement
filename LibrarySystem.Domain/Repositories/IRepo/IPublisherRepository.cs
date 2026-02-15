using LibrarySystem.Common.DTOs.Library.Publishers;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Repositories.IRepo
{
    public interface IPublisherRepository
    {
        Task AddPublisherAsync(PublisherCreateDto dto);
        Task UpdatePublisherAsync(int id, PublisherUpdateDto dto);
        Task SoftDeleteByIdAsync(int id);

        Task<List<PublisherListDto>> GetAllListAsync();
        Task<Publisher> GetRequiredByIdAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task<List<Publisher>> SearchAsync(string? text, int? number, int page, int pageSize);
        Task<int> CountForSearchAsync(string? text, int? number);
    }
}
