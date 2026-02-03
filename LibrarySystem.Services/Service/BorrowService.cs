using LibrarySystem.Common.DTOs.Library.Borrows;
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
        private readonly IPublishEndpoint _publish;

        public BorrowService(
            IBorrowRepository borrowRepo,
            IUserRepository userRepo,
            IPublishEndpoint publish)
        {
            _borrowRepo = borrowRepo;
            _userRepo = userRepo;
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
                Status = BorrowStatus.Borrowed,
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

        public async Task ReturnBook(BorrowReturnDto dto)
        {
            var borrow = await _borrowRepo.GetBorrowForReturnAsync(dto.Id);

            if (borrow.Status == BorrowStatus.Returned)
                throw new Exception("Already returned");

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

                await _publish.Publish(new BorrowOverdueEvent
                {
                    EventId = Guid.NewGuid(),
                    OccurredAt = now,
                    BorrowId = borrow.Id,
                    UserId = borrow.UserId,
                    DaysOverdue = borrow.OverdueDays ?? 0
                }, ctx => ctx.SetRoutingKey("borrow.overdue"));
            }

            await _borrowRepo.SaveAsync();
        }


    }
}
