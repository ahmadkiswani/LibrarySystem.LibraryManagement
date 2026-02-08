using LibrarySystem.Common.DTOs.Library.Borrows;
using LibrarySystem.Entities.Models;
using LibrarySystem.Services.Models;

namespace LibrarySystem.Services.Interfaces
{
    public interface IBorrowService
    {
        Task BorrowBook(int externalUserId, BorrowCreateDto dto);
        Task RequestBorrowByBook(int externalUserId, int bookId);
        Task ApproveBorrow(int borrowId);
        Task RejectBorrow(int borrowId);
        Task ReturnBook(BorrowReturnDto dto);
        Task<List<Borrow>> Search(BorrowSearchDto dto);
        Task<List<BorrowListDto>> GetMyBorrows(int externalUserId);
        Task<List<BorrowListDto>> GetBorrowsByExternalUserId(int externalUserId);
        Task<List<ActiveBorrowAdminDto>> GetActiveBorrowsForAdmin();
        Task<List<Borrow>> GetPendingBorrows();
        Task<List<Borrow>> GetMyPendingBorrows(int externalUserId);
        Task ProcessOverdueBorrowsAsync(DateTime now);
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task AutoReturnBorrowsForExternalUserAsync(int externalUserId);
    }
}
