using LibrarySystem.Common.DTOs.Library.Borrows;
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
                .Where(b => ids.Contains(b.Id))
                .ToListAsync();
        }

        public Task SaveAsync()
        {
            return _borrowRepo.SaveAsync();
        }
    }
}
