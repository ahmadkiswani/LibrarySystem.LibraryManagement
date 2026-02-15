using LibrarySystem.Common.DTOs.Library.Authors;
using LibrarySystem.Common.DTOs.Library.Helpers;

namespace LibrarySystem.Services.Interfaces
{
    public interface IAuthorService
    {
        Task AddAuthor(int userId, AuthorCreateDto dto);
        Task EditAuthor(int userId, int id, AuthorUpdateDto dto);
        Task DeleteAuthor(int userId, int id);

        Task<List<AuthorListDto>> GetAllAuthors();
        Task<AuthorDetailsDto> GetAuthorById(int id);
        Task<PagedResultDto<AuthorListDto>> Search(AuthorSearchDto dto);
    }
}
