using LibrarySystem.Common.DTOs.Library.Borrows;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Services.Interfaces
{
    public interface IBorrowService
    {
        Task BorrowBook(BorrowCreateDto dto);
        Task ReturnBook(BorrowReturnDto dto);
        Task<List<Borrow>> Search(BorrowSearchDto dto);
        Task MarkOverdueAsync(int borrowId);
        Task<List<int>> GetOverdueBorrowIdsAsync(DateTime now);
    }
}
