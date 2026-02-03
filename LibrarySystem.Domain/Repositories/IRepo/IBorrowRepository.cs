using LibrarySystem.Common.DTOs.Library.Borrows;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Repositories.IRepo
{
    public interface IBorrowRepository
    {
        Task BorrowAsync(Borrow borrow, BookCopy copy);
        Task ReturnAsync(Borrow borrow, BookCopy copy);

        Task<int> CountActiveBorrowsAsync(int userId);
        Task<BookCopy> GetCopyForBorrowAsync(int copyId);
        Task<Borrow> GetBorrowForReturnAsync(int borrowId);

        Task<List<Borrow>> SearchAsync(BorrowSearchDto dto);
        Task<List<int>> GetOverdueBorrowIdsAsync(DateTime now);
        Task<DateTime?> GetLastBorrowDateByBookAsync(int bookId);

        Task<List<Borrow>> GetBorrowsByIdsAsync(List<int> ids);
        Task SaveAsync();
    }
}
