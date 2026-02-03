using LibrarySystem.Common.DTOs.Library.Borrows;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Services.Interfaces
{
    public interface IBorrowService
    {
        Task BorrowBook(int externalUserId, BorrowCreateDto dto);
        Task ReturnBook(BorrowReturnDto dto);
        Task<List<Borrow>> Search(BorrowSearchDto dto);
        Task ProcessOverdueBorrowsAsync(DateTime now);
    }
}
