using LibrarySystem.Common.DTOs.Library.BookCopies;
using LibrarySystem.Common.Repositories;
using LibrarySystem.Domain.Helper;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class BookCopyRepository : IBookCopyRepository
    {
        private readonly IRepository<BookCopy> _copyRepo;

        public BookCopyRepository(IRepository<BookCopy> copyRepo)
        {
            _copyRepo = copyRepo;
        }

        public async Task AddCopyAsync(BookCopyCreateDto dto, Book book)
        {
            string code;
            int tries = 0;

            do
            {
                code = Guid.NewGuid().ToString("N")[..8].ToUpper();
                tries++;
                if (tries > 5)
                    throw new Exception("Failed to generate unique CopyCode");
            }
            while (await _copyRepo.GetQueryable().AnyAsync(x => x.CopyCode == code));

            var copy = new BookCopy
            {
                BookId = dto.BookId,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                PublisherId = book.PublisherId,
                CopyCode = code,
                IsAvailable = true
            };

            AuditHelper.OnCreate(copy);

            await _copyRepo.AddAsync(copy);
            await _copyRepo.SaveAsync();
        }

        public async Task DeleteCopyAsync(int copyId)
        {
            var copy = await _copyRepo.GetFirstAsync(x => x.Id == copyId)
                ?? throw new Exception("Copy not found");

            if (!copy.IsAvailable)
                throw new Exception("Cannot delete a borrowed copy");

            AuditHelper.OnSoftDelete(copy, null);

            _copyRepo.SoftDelete(copy);
            await _copyRepo.SaveAsync();
        }

        public async Task<BookCopy?> GetByIdWithBookIdAsync(int copyId)
        {
            return await _copyRepo
                .GetQueryable()
                .AsNoTracking()
                .Where(x => x.Id == copyId)
                .Select(x => new BookCopy
                {
                    Id = x.Id,
                    BookId = x.BookId,
                    IsAvailable = x.IsAvailable
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<BookCopyListDto>> GetAllListAsync()
        {
            return await _copyRepo
                .GetQueryable()
                .AsNoTracking()
                .Select(c => new BookCopyListDto
                {
                    Id = c.Id,
                    BookId = c.BookId,
                    CopyCode = c.CopyCode,
                    IsAvailable = c.IsAvailable
                })
                .ToListAsync();
        }

        public async Task<List<BookCopyListDto>> GetListByBookIdAsync(int bookId)
        {
            return await _copyRepo
                .GetQueryable()
                .AsNoTracking()
                .Where(c => c.BookId == bookId)
                .Select(c => new BookCopyListDto
                {
                    Id = c.Id,
                    BookId = c.BookId,
                    CopyCode = c.CopyCode,
                    IsAvailable = c.IsAvailable
                })
                .ToListAsync();
        }

        public async Task<BookCopy> GetRequiredCopyAsync(int id)
        {
            return await _copyRepo.GetFirstAsync(x => x.Id == id)
                ?? throw new Exception("Copy not found");
        }

        public Task<int> CountByBookAsync(int bookId)
            => _copyRepo.GetQueryable().AsNoTracking()
                .CountAsync(c => c.BookId == bookId);

        public Task<int> CountAvailableAsync(int bookId)
            => _copyRepo.GetQueryable().AsNoTracking()
                .CountAsync(c => c.BookId == bookId && c.IsAvailable);

        public Task<int> CountBorrowedAsync(int bookId)
            => _copyRepo.GetQueryable().AsNoTracking()
                .CountAsync(c => c.BookId == bookId && !c.IsAvailable);

        public async Task<BookCopy?> GetFirstAvailableCopyByBookAsync(int bookId)
        {
            return await _copyRepo
                .GetQueryable()
                .Where(c => c.BookId == bookId && c.IsAvailable)
                .OrderBy(c => c.Id)
                .FirstOrDefaultAsync();
        }
    }
}
