using LibrarySystem.Common.DTOs.Library.Books;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Services.Interfaces;

namespace LibrarySystem.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepo;
        private readonly IAuthorRepository _authorRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IPublisherRepository _publisherRepo;
        private readonly IBookCopyRepository _copyRepo;
        private readonly IBorrowRepository _borrowRepo;

        public BookService(
            IBookRepository bookRepo,
            IAuthorRepository authorRepo,
            ICategoryRepository categoryRepo,
            IPublisherRepository publisherRepo,
            IBookCopyRepository copyRepo,
            IBorrowRepository borrowRepo)
        {
            _bookRepo = bookRepo;
            _authorRepo = authorRepo;
            _categoryRepo = categoryRepo;
            _publisherRepo = publisherRepo;
            _copyRepo = copyRepo;
            _borrowRepo = borrowRepo;
        }

        public async Task<int> AddBook(BookCreateDto dto)
        {
            if (!await _authorRepo.ExistsAsync(dto.AuthorId))
                throw new Exception("Author does not exist");

            if (!await _categoryRepo.ExistsAsync(dto.CategoryId))
                throw new Exception("Category does not exist");

            if (!await _publisherRepo.ExistsAsync(dto.PublisherId))
                throw new Exception("Publisher does not exist");

            if (await _bookRepo.ExistsAsync(dto))
                throw new Exception("Book already exists");

            return await _bookRepo.AddBookAsync(dto);
        }

        public Task EditBook(int id, BookUpdateDto dto)
            => _bookRepo.UpdateBookAsync(id, dto);

        public async Task DeleteBook(int id)
        {
            int borrowedCount = await _copyRepo.CountBorrowedAsync(id);
            if (borrowedCount > 0)
                throw new Exception("Cannot delete book. Some copies are still borrowed. Return all copies first.");

            await _bookRepo.SoftDeleteByIdAsync(id);
        }

        public async Task<BookDetailsDto> GetBookById(int id)
        {
            var book = await _bookRepo.GetDetailsAsync(id)
                ?? throw new Exception("Book not found");

            int total = await _copyRepo.CountByBookAsync(id);
            int available = await _copyRepo.CountAvailableAsync(id);
            var lastBorrow = await _borrowRepo.GetLastBorrowDateByBookAsync(id);

            return new BookDetailsDto
            {
                Id = book.Id,
                Title = book.Title,
                Version = book.Version,
                PublishDate = book.PublishDate,
                AuthorName = book.Author.AuthorName,
                CategoryName = book.Category.Name,
                PublisherName = book.Publisher.Name,
                TotalCopies = total,
                AvailableCopies = available,
                BorrowedCopies = total - available,
                LastBorrowedDate = lastBorrow,
                IsDeleted = book.IsDeleted
            };
        }

        public Task<List<BookListDto>> GetAllBooks()
            => _bookRepo.SearchAsync(new BookSearchDto());

        public Task<List<BookListDto>> SearchBooks(BookSearchDto dto)
            => _bookRepo.SearchAsync(dto);

        public Task<BookDetailsDto> GetBookDetails(int id)
            => GetBookById(id);
    }
}
