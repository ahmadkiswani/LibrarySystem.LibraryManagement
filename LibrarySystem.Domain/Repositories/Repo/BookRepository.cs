using LibrarySystem.Common.DTOs.Library.Books;
using LibrarySystem.Domain.Data;
using LibrarySystem.Domain.Helper;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;

        public BookRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddBookAsync(BookCreateDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                Version = dto.Version,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                PublisherId = dto.PublisherId
            };

            AuditHelper.OnCreate(book);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return book.Id;
        }

        public async Task UpdateBookAsync(int id, BookUpdateDto dto)
        {
            var book = await _context.Books.FindAsync(id)
                ?? throw new Exception("Book not found");

            book.Title = dto.Title;
            book.Version = dto.Version;
            book.PublishDate = dto.PublishDate;
            book.AuthorId = dto.AuthorId;
            book.CategoryId = dto.CategoryId;
            book.PublisherId = dto.PublisherId;

            AuditHelper.OnUpdate(book, id);

            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteByIdAsync(int id)
        {
            var book = await _context.Books.FindAsync(id)
                ?? throw new Exception("Book not found");

            AuditHelper.OnSoftDelete(book, id);

            await _context.SaveChangesAsync();
        }

        public async Task<Book> GetRequiredByIdAsync(int id)
        {
            return await _context.Books
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id)
                ?? throw new Exception("Book not found");
        }

        public async Task<bool> ExistsAsync(BookCreateDto dto)
        {
            return await _context.Books
                .AsNoTracking()
                .AnyAsync(b =>
                    b.Title == dto.Title &&
                    b.Version == dto.Version &&
                    b.AuthorId == dto.AuthorId);
        }

        public async Task<Book?> GetDetailsAsync(int id)
        {
            return await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public Task<int> CountAsync()
        {
            return _context.Books.CountAsync(b => !b.IsDeleted);
        }

        public async Task<List<BookListDto>> SearchAsync(BookSearchDto dto)
        {
            int page = dto.Page > 0 ? dto.Page.Value : 1;
            int pageSize = dto.PageSize > 0 ? dto.PageSize.Value : 10;

            var query = _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .AsQueryable();

            bool hasFilter =
                !string.IsNullOrWhiteSpace(dto.Title) ||
                dto.AuthorId > 0 ||
                dto.CategoryId > 0 ||
                dto.PublisherId > 0;

            if (hasFilter)
            {
                query = query.Where(b =>
                    (!string.IsNullOrWhiteSpace(dto.Title) && b.Title.Contains(dto.Title)) ||
                    (dto.AuthorId > 0 && b.AuthorId == dto.AuthorId) ||
                    (dto.CategoryId > 0 && b.CategoryId == dto.CategoryId) ||
                    (dto.PublisherId > 0 && b.PublisherId == dto.PublisherId)
                );
            }

            return await query
                .OrderBy(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookListDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Author.AuthorName,
                    CategoryName = b.Category.Name,
                    PublisherName = b.Publisher != null ? b.Publisher.Name : string.Empty,
                    TotalCopies = _context.BookCopies.Count(c => c.BookId == b.Id && !c.IsDeleted),
                    AvailableCopies = _context.BookCopies.Count(c => c.BookId == b.Id && c.IsAvailable && !c.IsDeleted)
                })
                .ToListAsync();
        }

        public async Task<int> CountForSearchAsync(BookSearchDto dto)
        {
            var query = _context.Books
                .AsNoTracking()
                .AsQueryable();

            bool hasFilter =
                !string.IsNullOrWhiteSpace(dto.Title) ||
                (dto.AuthorId ?? 0) > 0 ||
                (dto.CategoryId ?? 0) > 0 ||
                (dto.PublisherId ?? 0) > 0;

            if (hasFilter)
            {
                query = query.Where(b =>
                    (!string.IsNullOrWhiteSpace(dto.Title) && b.Title.Contains(dto.Title!)) ||
                    ((dto.AuthorId ?? 0) > 0 && b.AuthorId == dto.AuthorId) ||
                    ((dto.CategoryId ?? 0) > 0 && b.CategoryId == dto.CategoryId) ||
                    ((dto.PublisherId ?? 0) > 0 && b.PublisherId == dto.PublisherId)
                );
            }

            return await query.CountAsync();
        }

        public async Task IncrementCopiesAsync(int bookId)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == bookId)
                ?? throw new Exception("Book not found");

            book.TotalCopies += 1;

            await _context.SaveChangesAsync();
        }

        public async Task DecrementCopiesAsync(int bookId)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == bookId)
                ?? throw new Exception("Book not found");

            if (book.TotalCopies <= 0)
                return;

            book.TotalCopies -= 1;

            await _context.SaveChangesAsync();
        }

    }
}
