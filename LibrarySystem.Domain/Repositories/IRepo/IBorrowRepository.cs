using LibrarySystem.Common.DTOs.Library.Borrows;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using LibrarySystem.Common.DTOs;

namespace LibrarySystem.Domain.Repositories
{
    public interface IBorrowRepository : IGenericRepository<Borrow>
    {
        Task<Borrow> BorrowBookAsync(BorrowCreateDto dto);
        Task<Borrow> ReturnBookAsync(BorrowReturnDto dto);
        Task<Borrow?> MarkOverdueAsync(int borrowId);

        Task<List<Borrow>> SearchAsync(BorrowSearchDto dto);
        Task<List<int>> GetOverdueBorrowIdsAsync(DateTime now);
        Task<DateTime?> GetLastBorrowDateByBookAsync(int bookId);
    }
}
