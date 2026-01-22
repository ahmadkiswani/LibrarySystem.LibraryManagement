using LibrarySystem.Common.DTOs.Library.Borrows;
using LibrarySystem.Common.Events;
using LibrarySystem.Domain.Repositories;
using LibrarySystem.Entities.Models;
using LibrarySystem.Services.Interfaces;
using LibrarySystem.Common.DTOs;
using MassTransit;

namespace LibrarySystem.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly IBorrowRepository _borrowRepo;
        private readonly IPublishEndpoint _publish;

        public BorrowService(
            IBorrowRepository borrowRepo,
            IPublishEndpoint publish)
        {
            _borrowRepo = borrowRepo;
            _publish = publish;
        }

        public async Task BorrowBook(BorrowCreateDto dto)
        {
            var borrow = await _borrowRepo.BorrowBookAsync(dto);

            await _publish.Publish(new BorrowCreatedEvent
            {
                EventId = Guid.NewGuid(),
                OccurredAt = borrow.BorrowDate,
                BorrowId = borrow.Id,
                UserId = borrow.UserId,
                BookCopyId = borrow.BookCopyId
            }, ctx => ctx.SetRoutingKey("borrow.created"));
        }

        public async Task ReturnBook(BorrowReturnDto dto)
        {
            var borrow = await _borrowRepo.ReturnBookAsync(dto);

            await _publish.Publish(new BorrowReturnedEvent
            {
                EventId = Guid.NewGuid(),
                OccurredAt = borrow.ReturnDate!.Value,
                BorrowId = borrow.Id,
                UserId = borrow.UserId,
                ReturnedAt = borrow.ReturnDate.Value,
                WasOverdue = false
            }, ctx => ctx.SetRoutingKey("borrow.returned"));
        }

        public async Task MarkOverdueAsync(int borrowId)
        {
            var borrow = await _borrowRepo.MarkOverdueAsync(borrowId);
            if (borrow == null) return;

            await _publish.Publish(new BorrowOverdueEvent
            {
                EventId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow,
                BorrowId = borrow.Id,
                UserId = borrow.UserId,
                DaysOverdue = borrow.OverdueDays ?? 0
            }, ctx => ctx.SetRoutingKey("borrow.overdue"));
        }

        public Task<List<Borrow>> Search(BorrowSearchDto dto)
            => _borrowRepo.SearchAsync(dto);
        public Task<List<int>> GetOverdueBorrowIdsAsync(DateTime now)
        {
            return _borrowRepo.GetOverdueBorrowIdsAsync(now);
        }
    }

}
