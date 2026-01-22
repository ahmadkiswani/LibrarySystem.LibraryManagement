using LibrarySystem.Common.DTOs.Library.Authors;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Repositories.IRepo
{
    public interface IAuthorRepository : IGenericRepository<Author>
    {
        Task AddAuthorAsync(AuthorCreateDto dto);

        Task SoftDeleteByIdAsync(int id);
        Task UpdateNameAsync(int id, string authorName);

        Task<List<AuthorListDto>> GetAllListAsync();
        Task<List<AuthorListDto>> SearchAsync(AuthorSearchDto dto);
        Task<AuthorDetailsDto> GetDetailsAsync(int id);
    }
}
