using LibrarySystem.Common.DTOs.Library.BookCopies;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Repositories.IRepo
{
    public interface IBookCopyRepository
    {
        Task AddCopyAsync(BookCopyCreateDto dto, Book book);
        Task DeleteCopyAsync(int copyId);

        Task<BookCopy?> GetByIdWithBookIdAsync(int copyId);
        Task<BookCopy> GetRequiredCopyAsync(int id);

        Task<List<BookCopyListDto>> GetAllListAsync();
        Task<List<BookCopyListDto>> GetListByBookIdAsync(int bookId);

        Task<int> CountByBookAsync(int bookId);
        Task<int> CountAvailableAsync(int bookId);
        Task<int> CountBorrowedAsync(int bookId);
        Task<BookCopy?> GetFirstAvailableCopyByBookAsync(int bookId);
    }
}
