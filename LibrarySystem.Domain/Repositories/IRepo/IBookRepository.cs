using LibrarySystem.Common.DTOs.Library.Books;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Repositories.IRepo
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<int> AddBookAsync(BookCreateDto dto);
        Task UpdateBookAsync(int id, BookUpdateDto dto);
        Task SoftDeleteByIdAsync(int id);

        Task<Book> GetRequiredByIdAsync(int id);
        Task<Book?> GetDetailsAsync(int id);
        Task<List<BookListDto>> SearchAsync(BookSearchDto dto);
        Task<bool> ExistsAsync(BookCreateDto dto);
    }
}
