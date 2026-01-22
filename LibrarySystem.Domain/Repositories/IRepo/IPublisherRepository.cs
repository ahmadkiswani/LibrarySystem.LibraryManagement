using LibrarySystem.Common.DTOs.Library.Publishers;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Repositories.IRepo
{
    public interface IPublisherRepository : IGenericRepository<Publisher>
    {
        Task AddPublisherAsync(PublisherCreateDto dto);
        Task UpdatePublisherAsync(int id, PublisherUpdateDto dto);
        Task SoftDeleteByIdAsync(int id);

        Task<List<PublisherListDto>> GetAllListAsync();
        Task<Publisher> GetRequiredByIdAsync(int id);

        Task<bool> ExistsByNameAsync(string name);
    }
}
