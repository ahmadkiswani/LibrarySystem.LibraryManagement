using LibrarySystem.Common.DTOs.Library.Borrows;
using LibrarySystem.Domain.Data;
using LibrarySystem.Domain.Repositories;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using LibrarySystem.Common.DTOs;
using Microsoft.EntityFrameworkCore;
using System;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class BorrowRepository : GenericRepository<Borrow>, IBorrowRepository
    {
        private readonly IGenericRepository<BookCopy> _copyRepo;
        private readonly IGenericRepository<User> _userRepo;

        public BorrowRepository(
            LibraryDbContext context,
            IGenericRepository<BookCopy> copyRepo,
            IGenericRepository<User> userRepo)
            : base(context)
        {
            _copyRepo = copyRepo;
            _userRepo = userRepo;
        }

        public async Task<Borrow> BorrowBookAsync(BorrowCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepo.GetByIdAsync(dto.UserId)
                    ?? throw new Exception("User not found");

                if (user.IsDeleted)
                    throw new Exception("User is not active");

                var copy = await _copyRepo.GetQueryable()
                    .Include(c => c.Book)
                        .ThenInclude(b => b.Category)
                    .FirstOrDefaultAsync(c => c.Id == dto.BookCopyId)
                    ?? throw new Exception("Copy not found");

                if (!copy.IsAvailable)
                    throw new Exception("Copy is not available");

                int activeBorrows = await GetQueryable()
                    .CountAsync(b =>
                        b.UserId == dto.UserId &&
                        b.Status == BorrowStatus.Borrowed);

                if (activeBorrows >= 5)
                    throw new Exception("User cannot borrow more than 5 books");

                copy.IsAvailable = false;
                await _copyRepo.UpdateAsync(copy);

                var now = DateTime.UtcNow;

                var borrow = new Borrow
                {
                    UserId = dto.UserId,
                    BookCopyId = dto.BookCopyId,
                    BorrowDate = now,
                    DueDate = now.AddDays(5),
                    Status = BorrowStatus.Borrowed,
                    OverdueDays = 0
                };

                await AddAsync(borrow);

                await transaction.CommitAsync();
                return borrow;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Borrow> ReturnBookAsync(BorrowReturnDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var borrow = await GetByIdAsync(dto.Id)
                    ?? throw new Exception("Borrow not found");

                if (borrow.Status == BorrowStatus.Returned)
                    throw new Exception("Already returned");

                var now = DateTime.UtcNow;

                borrow.ReturnDate = now;
                borrow.Status = BorrowStatus.Returned;
                borrow.OverdueDays = 0;

                await UpdateAsync(borrow);

                var copy = await _copyRepo.GetByIdAsync(borrow.BookCopyId);
                if (copy != null)
                {
                    copy.IsAvailable = true;
                    await _copyRepo.UpdateAsync(copy);
                }

                await transaction.CommitAsync();
                return borrow;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Borrow?> MarkOverdueAsync(int borrowId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var borrow = await GetByIdAsync(borrowId);
                if (borrow == null || borrow.Status != BorrowStatus.Borrowed)
                    return null;

                var now = DateTime.UtcNow;
                if (borrow.DueDate >= now)
                    return null;

                borrow.Status = BorrowStatus.Overdue;
                borrow.OverdueDays = (now - borrow.DueDate).Days;

                await UpdateAsync(borrow);
                await transaction.CommitAsync();
                return borrow;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Borrow>> SearchAsync(BorrowSearchDto dto)
        {
            var query = GetQueryable()
                .Where(b => !dto.Number.HasValue || b.Id == dto.Number.Value)
                .Where(b => !dto.UserId.HasValue || b.UserId == dto.UserId.Value)
                .Where(b => !dto.BookCopyId.HasValue || b.BookCopyId == dto.BookCopyId.Value)
                .Where(b =>
                    !dto.Returned.HasValue ||
                    (dto.Returned.Value
                        ? b.Status == BorrowStatus.Returned
                        : b.Status != BorrowStatus.Returned)
                );

            int page = dto.Page > 0 ? dto.Page : 1;
            int pageSize = dto.PageSize > 0 ? dto.PageSize : 10;

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<List<int>> GetOverdueBorrowIdsAsync(DateTime now)
        {
            return await GetQueryable()
                .Where(b => b.Status == BorrowStatus.Borrowed && b.DueDate < now)
                .Select(b => b.Id)
                .ToListAsync();
        }

        public async Task<DateTime?> GetLastBorrowDateByBookAsync(int bookId)
        {
            return await GetQueryable()
                .Where(b => b.BookCopy.BookId == bookId)
                .MaxAsync(b => (DateTime?)b.BorrowDate);
        }

    }
}
