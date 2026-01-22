using LibrarySystem.Common.DTOs.Library.Books;
using LibrarySystem.Domain.Data;
using LibrarySystem.Domain.Helper;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class BookRepository
        : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(LibraryDbContext context)
            : base(context)
        {
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
            await AddAsync(book);
            return book.Id;
        }

        public async Task UpdateBookAsync(int id, BookUpdateDto dto)
        {
            var book = await GetByIdAsync(id);
            if (book == null)
                throw new Exception("Book not found");

            book.Title = dto.Title;
            book.Version = dto.Version;
            book.PublishDate = dto.PublishDate;
            book.AuthorId = dto.AuthorId;
            book.CategoryId = dto.CategoryId;
            book.PublisherId = dto.PublisherId;

            AuditHelper.OnUpdate(book, id);
            await UpdateAsync(book);
        }

        public async Task SoftDeleteByIdAsync(int id)
        {
            var book = await GetByIdAsync(id);
            if (book == null)
                throw new Exception("Book not found");

            AuditHelper.OnSoftDelete(book, id);
            await SoftDeleteAsync(book);
        }

        public async Task<Book> GetRequiredByIdAsync(int id)
        {
            return await GetByIdAsync(id)
                ?? throw new Exception("Book not found");
        }

        public async Task<bool> ExistsAsync(BookCreateDto dto)
        {
            return await _dbSet.AnyAsync(b =>
                b.Title == dto.Title &&
                b.Version == dto.Version &&
                b.AuthorId == dto.AuthorId);
        }

        public async Task<Book?> GetDetailsAsync(int id)
        {
            return await _dbSet
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<BookListDto>> SearchAsync(BookSearchDto dto)
        {
            int page = dto.Page > 0 ? dto.Page.Value : 1;
            int pageSize = dto.PageSize > 0 ? dto.PageSize.Value : 10;

            int? author = dto.AuthorId > 0 ? dto.AuthorId : null;
            int? category = dto.CategoryId > 0 ? dto.CategoryId : null;
            int? publisher = dto.PublisherId > 0 ? dto.PublisherId : null;

            var bookCopies = _context.Set<BookCopy>();

            return await _dbSet
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Where(b =>
                    (string.IsNullOrWhiteSpace(dto.Title) ||
                     b.Title.Contains(dto.Title))
                    && (author == null || b.AuthorId == author)
                    && (category == null || b.CategoryId == category)
                    && (publisher == null || b.PublisherId == publisher)
                )
                .Select(b => new BookListDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Author.AuthorName,
                    CategoryName = b.Category.Name,
                    PublisherName = b.Publisher != null ? b.Publisher.Name : string.Empty,
                    TotalCopies = bookCopies.Count(c => c.BookId == b.Id),
                    AvailableCopies = bookCopies.Count(c =>
                        c.BookId == b.Id && c.IsAvailable)
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
