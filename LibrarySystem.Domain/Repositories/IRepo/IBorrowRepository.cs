using LibrarySystem.Common.DTOs.Library.Borrows;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Repositories.IRepo
{
    public interface IBorrowRepository
    {
        Task BorrowAsync(Borrow borrow, BookCopy copy);
        Task ReturnAsync(Borrow borrow, BookCopy copy);
        Task UpdateAsync(Borrow borrow);

        Task<int> CountActiveBorrowsAsync(int userId);
        Task<BookCopy> GetCopyForBorrowAsync(int copyId);
        Task<Borrow> GetBorrowForReturnAsync(int borrowId);

        Task<List<Borrow>> SearchAsync(BorrowSearchDto dto);
        Task<List<int>> GetOverdueBorrowIdsAsync(DateTime now);
        Task<DateTime?> GetLastBorrowDateByBookAsync(int bookId);

        Task<List<Borrow>> GetBorrowsByIdsAsync(List<int> ids);
        Task SaveAsync();

        Task<Borrow?> GetBorrowWithCopyAndBookAsync(int borrowId);
        Task<List<Borrow>> GetPendingBorrowsAsync();
        Task<List<Borrow>> GetPendingBorrowsByUserIdAsync(int userId);
        Task ReleaseCopyAsync(Borrow borrow, BookCopy copy);
        Task<List<Borrow>> GetBorrowsByUserIdWithBookAsync(int userId);
        Task<List<Borrow>> GetActiveBorrowsWithUserAndCopyAsync();

        Task<int> CountDistinctActiveBorrowersAsync();
        Task<int> CountBorrowsCreatedTodayAsync(DateTime todayUtc);
        Task<int> CountOverdueAsync(DateTime nowUtc);
    }
}
