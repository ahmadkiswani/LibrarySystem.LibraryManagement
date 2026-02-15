using LibrarySystem.Common.DTOs.Library.Borrows;
using LibrarySystem.Common.Helpers;
using LibrarySystem.Common.Repositories;
using LibrarySystem.Domain.Data;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class BorrowRepository : IBorrowRepository
    {
        private readonly IRepository<Borrow> _borrowRepo;
        private readonly IRepository<BookCopy> _copyRepo;
        private readonly LibraryDbContext _context;

        public BorrowRepository(
            IRepository<Borrow> borrowRepo,
            IRepository<BookCopy> copyRepo,
            LibraryDbContext context)
        {
            _borrowRepo = borrowRepo;
            _copyRepo = copyRepo;
            _context = context;
        }

        public Task<int> CountActiveBorrowsAsync(int userId)
        {
            return _borrowRepo
                .GetQueryable()
                .CountAsync(b =>
                    b.UserId == userId &&
                    b.Status == BorrowStatus.Borrowed);
        }

        public async Task<BookCopy> GetCopyForBorrowAsync(int copyId)
        {
            return await _context.BookCopies
                .Include(c => c.Book)
                    .ThenInclude(b => b.Category)
                .FirstOrDefaultAsync(c => c.Id == copyId)
                ?? throw new Exception("Copy not found");
        }

        public async Task<Borrow> GetBorrowForReturnAsync(int borrowId)
        {
            return await _borrowRepo
                .GetFirstAsync(b => b.Id == borrowId)
                ?? throw new Exception("Borrow not found");
        }

        public async Task BorrowAsync(Borrow borrow, BookCopy copy)
        {
            copy.IsAvailable = false;

            await _borrowRepo.AddAsync(borrow);
            await _copyRepo.UpdateAsync(copy);

            await _borrowRepo.SaveAsync();
        }

        public async Task ReturnAsync(Borrow borrow, BookCopy copy)
        {
            copy.IsAvailable = true;

            await _borrowRepo.UpdateAsync(borrow);
            await _copyRepo.UpdateAsync(copy);

            await _borrowRepo.SaveAsync();
        }

        public async Task<List<Borrow>> SearchAsync(BorrowSearchDto dto)
        {
            var query = _borrowRepo
                .GetQueryable()
                .AsNoTracking()
                .Where(b => !dto.Number.HasValue || b.Id == dto.Number.Value)
                .Where(b => !dto.UserId.HasValue || b.UserId == dto.UserId.Value)
                .Where(b => !dto.BookCopyId.HasValue || b.BookCopyId == dto.BookCopyId.Value)
                .Where(b =>
                    !dto.Returned.HasValue ||
                    (dto.Returned.Value
                        ? b.Status == BorrowStatus.Returned
                        : b.Status != BorrowStatus.Returned));

            int page = dto.Page > 0 ? dto.Page : 1;
            int pageSize = dto.PageSize > 0 ? dto.PageSize : 10;
            AppHelper.NormalizePage(ref page, ref pageSize, 10, 200);

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<List<int>> GetOverdueBorrowIdsAsync(DateTime now)
        {
            return _borrowRepo
                .GetQueryable()
                .AsNoTracking()
                .Where(b =>
                    b.Status == BorrowStatus.Borrowed &&
                    b.DueDate < now)
                .Select(b => b.Id)
                .ToListAsync();
        }

        public Task<DateTime?> GetLastBorrowDateByBookAsync(int bookId)
        {
            return _borrowRepo
                .GetQueryable()
                .AsNoTracking()
                .Where(b => b.BookCopy.BookId == bookId)
                .MaxAsync(b => (DateTime?)b.BorrowDate);
        }


        public Task<List<Borrow>> GetBorrowsByIdsAsync(List<int> ids)
        {
            return _borrowRepo
                .GetQueryable()
                .Include(b => b.User)
                .Where(b => ids.Contains(b.Id))
                .ToListAsync();
        }

        public Task SaveAsync()
        {
            return _borrowRepo.SaveAsync();
        }

        public async Task<Borrow?> GetBorrowWithCopyAndBookAsync(int borrowId)
        {
            return await _borrowRepo
                .GetQueryable()
                .Include(b => b.User)
                .Include(b => b.BookCopy)
                    .ThenInclude(c => c.Book)
                    .ThenInclude(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == borrowId);
        }

        public async Task<List<Borrow>> GetPendingBorrowsAsync()
        {
            return await _borrowRepo
                .GetQueryable()
                .Include(b => b.User)
                .Include(b => b.BookCopy)
                    .ThenInclude(c => c.Book)
                .Where(b => b.Status == BorrowStatus.Pending)
                .OrderBy(b => b.BorrowDate)
                .ToListAsync();
        }

        public async Task<List<Borrow>> GetPendingBorrowsByUserIdAsync(int userId)
        {
            return await _borrowRepo
                .GetQueryable()
                .Include(b => b.User)
                .Include(b => b.BookCopy)
                    .ThenInclude(c => c.Book)
                .Where(b => b.Status == BorrowStatus.Pending && b.UserId == userId)
                .OrderBy(b => b.BorrowDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(Borrow borrow)
        {
            await _borrowRepo.UpdateAsync(borrow);
            await _borrowRepo.SaveAsync();
        }

        public async Task ReleaseCopyAsync(Borrow borrow, BookCopy copy)
        {
            copy.IsAvailable = true;
            await _borrowRepo.UpdateAsync(borrow);
            await _copyRepo.UpdateAsync(copy);
            await _borrowRepo.SaveAsync();
        }

        public async Task<List<Borrow>> GetBorrowsByUserIdWithBookAsync(int userId)
        {
            return await _borrowRepo
                .GetQueryable()
                .Include(b => b.BookCopy)
                    .ThenInclude(c => c.Book)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();
        }

        public async Task<List<Borrow>> GetActiveBorrowsWithUserAndCopyAsync()
        {
            return await _borrowRepo
                .GetQueryable()
                .Include(b => b.User)
                .Include(b => b.BookCopy)
                    .ThenInclude(c => c.Book)
                        .ThenInclude(book => book.Category)
                .Where(b => b.Status == BorrowStatus.Borrowed || b.Status == BorrowStatus.Overdue)
                .OrderBy(b => b.DueDate)
                .ToListAsync();
        }

        public Task<int> CountDistinctActiveBorrowersAsync()
        {
            return _borrowRepo
                .GetQueryable()
                .AsNoTracking()
                .Where(b => b.Status == BorrowStatus.Borrowed)
                .Select(b => b.UserId)
                .Distinct()
                .CountAsync();
        }

        public Task<int> CountBorrowsCreatedTodayAsync(DateTime todayUtc)
        {
            var start = todayUtc.Date;
            var end = start.AddDays(1);
            return _borrowRepo
                .GetQueryable()
                .AsNoTracking()
                .Where(b => b.BorrowDate >= start && b.BorrowDate < end)
                .CountAsync();
        }

        public Task<int> CountOverdueAsync(DateTime nowUtc)
        {
            return _borrowRepo
                .GetQueryable()
                .AsNoTracking()
                .Where(b => b.ReturnDate == null &&
                    ((b.Status == BorrowStatus.Borrowed && b.DueDate < nowUtc) || b.Status == BorrowStatus.Overdue))
                .CountAsync();
        }
    }
}
