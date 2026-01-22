using LibrarySystem.Common.DTOs.Library.BookCopies;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Services.Interfaces
{
    public interface IBookCopyService
    {
        Task AddBookCopy(BookCopyCreateDto dto);
        Task DeleteBookCopy(int id);
        Task<List<BookCopyListDto>> ListBookCopies();
        Task<BookCopy> GetSpecificCopy(int id);
        Task<int> GetAllCopiesCount(int bookId);
        Task<int> GetAvailableCount(int bookId);
        Task<int> GetBorrowedCount(int bookId);
    }
}
