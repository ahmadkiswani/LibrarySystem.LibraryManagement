using LibrarySystem.Common.DTOs.Library.BookCopies;
using LibrarySystem.Domain.Data;
using LibrarySystem.Domain.Helper;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class BookCopyRepository
        : GenericRepository<BookCopy>, IBookCopyRepository
    {
        public BookCopyRepository(LibraryDbContext context)
            : base(context)
        {
        }

        public async Task AddCopyAsync(BookCopyCreateDto dto, Book book)
        {
            var copy = new BookCopy
            {
                BookId = dto.BookId,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                PublisherId = book.PublisherId,
                CopyCode = Guid.NewGuid().ToString()[..8],
                IsAvailable = true
            };

            AuditHelper.OnCreate(copy);
            await AddAsync(copy);
        }

        public async Task SoftDeleteByIdAsync(int copyId)
        {
            var copy = await GetByIdAsync(copyId);
            if (copy == null)
                throw new Exception("Copy not found");

            AuditHelper.OnSoftDelete(copy, copyId);
            await SoftDeleteAsync(copy);
        }

        public async Task<List<BookCopyListDto>> GetAllListAsync()
        {
            return await GetQueryable()
                .Select(c => new BookCopyListDto
                {
                    Id = c.Id,
                    BookId = c.BookId,
                    IsAvailable = c.IsAvailable
                })
                .ToListAsync();
        }

        public async Task<BookCopy> GetRequiredCopyAsync(int id)
        {
            var copy = await GetByIdAsync(id);
            if (copy == null)
                throw new Exception("Copy not found");

            return copy;
        }

        public async Task<int> CountByBookAsync(int bookId)
            => await _dbSet.CountAsync(c => c.BookId == bookId);

        public async Task<int> CountAvailableAsync(int bookId)
            => await _dbSet.CountAsync(c => c.BookId == bookId && c.IsAvailable);

        public async Task<int> CountBorrowedAsync(int bookId)
            => await _dbSet.CountAsync(c => c.BookId == bookId && !c.IsAvailable);

        public async Task<bool> IsCopyAvailableAsync(int copyId)
            => await _dbSet.AnyAsync(c => c.Id == copyId && c.IsAvailable);
    }
}
