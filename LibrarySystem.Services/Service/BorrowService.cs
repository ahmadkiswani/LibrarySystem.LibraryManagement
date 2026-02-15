using LibrarySystem.Common.DTOs.Library.Borrows;
using LibrarySystem.Common.DTOs.Library;
using LibrarySystem.Common.Events;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using LibrarySystem.Services.Interfaces;
using MassTransit;
namespace LibrarySystem.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly IBorrowRepository _borrowRepo;
        private readonly IUserRepository _userRepo;
        private readonly IBookCopyRepository _copyRepo;
        private readonly IBookRepository _bookRepo;
        private readonly IPublishEndpoint _publish;

        public BorrowService(
            IBorrowRepository borrowRepo,
            IUserRepository userRepo,
            IBookCopyRepository copyRepo,
            IBookRepository bookRepo,
            IPublishEndpoint publish)
        {
            _borrowRepo = borrowRepo;
            _userRepo = userRepo;
            _copyRepo = copyRepo;
            _bookRepo = bookRepo;
            _publish = publish;
        }

        public async Task BorrowBook(int externalUserId, BorrowCreateDto dto)
        {
            var user = await _userRepo.GetByExternalIdAsync(externalUserId)
                ?? throw new Exception("User not found (not synced)");

            if (user.IsDeleted)
                throw new Exception("User is not active");

            var copy = await _borrowRepo.GetCopyForBorrowAsync(dto.BookCopyId);

            if (!copy.IsAvailable)
                throw new Exception("Copy is not available");

            int active = await _borrowRepo.CountActiveBorrowsAsync(user.Id);
            if (active >= 5)
                throw new Exception("User cannot borrow more than 5 books");

            var borrow = new Borrow
            {
                UserId = user.Id,
                BookCopyId = copy.Id,
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(5),
                Status = BorrowStatus.Pending,
                OverdueDays = 0
            };

            await _borrowRepo.BorrowAsync(borrow, copy);

            await _publish.Publish(new BorrowCreatedEvent
            {
                EventId = Guid.NewGuid(),
                OccurredAt = borrow.BorrowDate,
                BorrowId = borrow.Id,
                UserId = user.Id,
                UserName = user.UserName,
                BookCopyId = borrow.BookCopyId,
                CategoryId = copy.Book.CategoryId,
                CategoryName = copy.Book.Category.Name
            }, ctx => ctx.SetRoutingKey("borrow.created"));
        }

        public async Task RequestBorrowByBook(int externalUserId, int bookId)
        {
            var user = await _userRepo.GetByExternalIdAsync(externalUserId)
                ?? throw new Exception("User not found (not synced)");
            if (user.IsDeleted)
                throw new Exception("User is not active");

            int availableCount = await _copyRepo.CountAvailableAsync(bookId);
            if (availableCount <= 0)
                throw new Exception("No available copy for this book. All copies are currently borrowed.");

            var copy = await _copyRepo.GetFirstAvailableCopyByBookAsync(bookId)
                ?? throw new Exception("No available copy for this book");

            int active = await _borrowRepo.CountActiveBorrowsAsync(user.Id);
            if (active >= 5)
                throw new Exception("User cannot borrow more than 5 books");

            var borrow = new Borrow
            {
                UserId = user.Id,
                BookCopyId = copy.Id,
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(5),
                Status = BorrowStatus.Pending,
                OverdueDays = 0
            };
            await _borrowRepo.BorrowAsync(borrow, copy);
        }

        public async Task ApproveBorrow(int borrowId)
        {
            var borrow = await _borrowRepo.GetBorrowWithCopyAndBookAsync(borrowId)
                ?? throw new Exception("Borrow not found");
            if (borrow.Status != BorrowStatus.Pending)
                throw new Exception("Only pending borrows can be approved");
            borrow.Status = BorrowStatus.Borrowed;
            await _borrowRepo.UpdateAsync(borrow);

            await _publish.Publish(new BorrowCreatedEvent
            {
                EventId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow,
                BorrowId = borrow.Id,
                UserId = borrow.UserId,
                UserName = borrow.User?.UserName,
                BookCopyId = borrow.BookCopyId,
                CategoryId = borrow.BookCopy?.Book?.CategoryId ?? 0,
                CategoryName = borrow.BookCopy?.Book?.Category?.Name
            }, ctx => ctx.SetRoutingKey("borrow.created"));
        }

        public async Task RejectBorrow(int borrowId)
        {
            var borrow = await _borrowRepo.GetBorrowWithCopyAndBookAsync(borrowId)
                ?? throw new Exception("Borrow not found");
            if (borrow.Status != BorrowStatus.Pending)
                throw new Exception("Only pending borrows can be rejected");
            borrow.Status = BorrowStatus.Rejected;
            await _borrowRepo.ReleaseCopyAsync(borrow, borrow.BookCopy);
        }

        public async Task ReturnBook(BorrowReturnDto dto)
        {
            var borrow = await _borrowRepo.GetBorrowForReturnAsync(dto.Id);

            if (borrow.Status == BorrowStatus.Returned)
                throw new Exception("Already returned");
            if (borrow.Status != BorrowStatus.Borrowed && borrow.Status != BorrowStatus.Overdue)
                throw new Exception("Only borrowed or overdue books can be returned");

            borrow.ReturnDate = DateTime.UtcNow;
            borrow.Status = BorrowStatus.Returned;
            var copy = await _borrowRepo.GetCopyForBorrowAsync(borrow.BookCopyId);

            await _borrowRepo.ReturnAsync(borrow, copy);

            await _publish.Publish(new BorrowReturnedEvent
            {
                EventId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow,
                BorrowId = borrow.Id,
                UserId = borrow.UserId,
                ReturnedAt = DateTime.UtcNow,
                WasOverdue = false
            }, ctx => ctx.SetRoutingKey("borrow.returned"));
        }

        public Task<List<Borrow>> Search(BorrowSearchDto dto)
            => _borrowRepo.SearchAsync(dto);

        public async Task<List<BorrowListDto>> GetMyBorrows(int externalUserId)
        {
            var user = await _userRepo.GetByExternalIdAsync(externalUserId);
            if (user == null) return new List<BorrowListDto>();
            var borrows = await _borrowRepo.GetBorrowsByUserIdWithBookAsync(user.Id);
            return borrows.Select(b => new BorrowListDto
            {
                Id = b.Id,
                UserId = b.UserId,
                BookCopyId = b.BookCopyId,
                CopyCode = b.BookCopy?.CopyCode,
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                ReturnDate = b.ReturnDate,
                Status = (int)b.Status,
                OverdueDays = b.OverdueDays,
                BookTitle = b.BookCopy?.Book?.Title
            }).ToList();
        }

        public async Task<List<BorrowListDto>> GetBorrowsByExternalUserId(int externalUserId)
        {
            var user = await _userRepo.GetByExternalIdAsync(externalUserId);
            if (user == null) return new List<BorrowListDto>();
            var borrows = await _borrowRepo.GetBorrowsByUserIdWithBookAsync(user.Id);
            return borrows.Select(b => new BorrowListDto
            {
                Id = b.Id,
                UserId = b.UserId,
                BookCopyId = b.BookCopyId,
                CopyCode = b.BookCopy?.CopyCode,
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                ReturnDate = b.ReturnDate,
                Status = (int)b.Status,
                OverdueDays = b.OverdueDays,
                BookTitle = b.BookCopy?.Book?.Title
            }).ToList();
        }

        public async Task<List<ActiveBorrowAdminDto>> GetActiveBorrowsForAdmin()
        {
            var borrows = await _borrowRepo.GetActiveBorrowsWithUserAndCopyAsync();
            return borrows.Select(b => new ActiveBorrowAdminDto
            {
                Id = b.Id,
                UserName = b.User?.UserName ,
                BookTitle = b.BookCopy?.Book?.Title ,
                CategoryName = b.BookCopy?.Book?.Category?.Name ,
                CopyCode = b.BookCopy?.CopyCode,
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                Status = (int)b.Status,
                StatusText = b.Status.ToString()
            }).ToList();
        }

        public Task<List<Borrow>> GetPendingBorrows()
            => _borrowRepo.GetPendingBorrowsAsync();

        public async Task<List<Borrow>> GetMyPendingBorrows(int externalUserId)
        {
            var user = await _userRepo.GetByExternalIdAsync(externalUserId);
            if (user == null)
                return new List<Borrow>();
            return await _borrowRepo.GetPendingBorrowsByUserIdAsync(user.Id);
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var totalBooks = await _bookRepo.CountAsync();
            var activeUsers = await _borrowRepo.CountDistinctActiveBorrowersAsync();
            var borrowedToday = await _borrowRepo.CountBorrowsCreatedTodayAsync(today);
            var overdue = await _borrowRepo.CountOverdueAsync(now);
            return new DashboardStatsDto
            {
                TotalBooks = totalBooks,
                ActiveUsers = activeUsers,
                BorrowedToday = borrowedToday,
                Overdue = overdue
            };
        }

        public async Task ProcessOverdueBorrowsAsync(DateTime now)
        {
            var overdueIds = await _borrowRepo.GetOverdueBorrowIdsAsync(now);

            if (!overdueIds.Any())
                return;

            var borrows = await _borrowRepo.GetBorrowsByIdsAsync(overdueIds);

            foreach (var borrow in borrows)
            {
                borrow.Status = BorrowStatus.Overdue;
                borrow.OverdueDays =
                    (int)(now.Date - borrow.DueDate.Date).TotalDays;
            }

            await _borrowRepo.SaveAsync();

            foreach (var borrow in borrows)
            {
                await _publish.Publish(new BorrowOverdueEvent
                {
                    EventId = Guid.NewGuid(),
                    OccurredAt = now,
                    BorrowId = borrow.Id,
                    UserId = borrow.UserId,
                    UserName = borrow.User?.UserName ?? "",
                    DaysOverdue = borrow.OverdueDays ?? 0
                }, ctx => ctx.SetRoutingKey("borrow.overdue"));
            }
        }

        public async Task AutoReturnBorrowsForExternalUserAsync(int externalUserId)
        {
            var user = await _userRepo.GetByExternalIdAsync(externalUserId);
            if (user == null)
                return;

            var borrows = await _borrowRepo.GetBorrowsByUserIdWithBookAsync(user.Id);
            var now = DateTime.UtcNow;

            foreach (var borrow in borrows
                         .Where(b => b.Status == BorrowStatus.Borrowed || b.Status == BorrowStatus.Overdue)
                         .ToList())
            {
                if (borrow.BookCopy == null)
                    continue;

                borrow.Status = BorrowStatus.Returned;
                borrow.ReturnDate = now;

                await _borrowRepo.ReleaseCopyAsync(borrow, borrow.BookCopy);
            }
        }

    }
}