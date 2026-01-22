using LibrarySystem.Common.DTOs.Library.BookCopies;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Repositories.IRepo
{
    public interface IBookCopyRepository : IGenericRepository<BookCopy>
    {
        Task AddCopyAsync(BookCopyCreateDto dto, Book book);
        Task SoftDeleteByIdAsync(int copyId);

        Task<List<BookCopyListDto>> GetAllListAsync();
        Task<BookCopy> GetRequiredCopyAsync(int id);

        Task<int> CountByBookAsync(int bookId);
        Task<int> CountAvailableAsync(int bookId);
        Task<int> CountBorrowedAsync(int bookId);
        Task<bool> IsCopyAvailableAsync(int copyId);
    }
}
