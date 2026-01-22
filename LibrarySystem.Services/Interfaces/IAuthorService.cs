using LibrarySystem.Common.DTOs.Library.Authors;
namespace LibrarySystem.Services.Interfaces
{
    public interface IAuthorService
    {
        Task AddAuthor(AuthorCreateDto dto);
        Task<List<AuthorListDto>> GetAllAuthors();
        Task<AuthorDetailsDto> GetAuthorById(int id);
        Task EditAuthor(int id, AuthorUpdateDto dto);
        Task DeleteAuthor(int id);
        Task<List<AuthorListDto>> Search(AuthorSearchDto dto);
    }
}