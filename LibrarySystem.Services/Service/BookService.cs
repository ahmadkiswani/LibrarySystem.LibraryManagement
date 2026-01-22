using LibrarySystem.Common.DTOs.Library.Books;
using LibrarySystem.Domain.Repositories;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepo;
        private readonly IAuthorRepository _authorRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly IBookCopyRepository _bookCopyRepository;
        private readonly IBorrowRepository _borrowRepository;

        public BookService(
            IBookRepository bookRepo,
            IAuthorRepository authorRepository,
            ICategoryRepository categoryRepository,
            IPublisherRepository publisherRepository,
            IBookCopyRepository bookCopyRepository,
            IBorrowRepository borrowRepository)
        {
            _bookRepo = bookRepo;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _publisherRepository = publisherRepository;
            _bookCopyRepository = bookCopyRepository;
            _borrowRepository = borrowRepository;
        }

        public async Task<int> AddBook(BookCreateDto dto)
        {
            if (!await _authorRepository.GetQueryable().AnyAsync(a => a.Id == dto.AuthorId))
                throw new Exception("Author does not exist");

            if (!await _categoryRepository.GetQueryable().AnyAsync(c => c.Id == dto.CategoryId))
                throw new Exception("Category does not exist");

            if (!await _publisherRepository.GetQueryable().AnyAsync(p => p.Id == dto.PublisherId))
                throw new Exception("Publisher does not exist");

            if (await _bookRepo.ExistsAsync(dto))
                throw new Exception("Book already exists");

            return await _bookRepo.AddBookAsync(dto);
        }

        public async Task<BookDetailsDto> GetBookById(int id)
        {
            var book = await _bookRepo.GetDetailsAsync(id)
                ?? throw new Exception("Book not found");

            int total = await _bookCopyRepository.CountByBookAsync(id);
            int available = await _bookCopyRepository.CountAvailableAsync(id);
            var lastBorrow = await _borrowRepository.GetLastBorrowDateByBookAsync(id);

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

        public Task EditBook(int id, BookUpdateDto dto)
            => _bookRepo.UpdateBookAsync(id, dto);

        public Task DeleteBook(int id)
            => _bookRepo.SoftDeleteByIdAsync(id);

        public Task<List<BookListDto>> SearchBooks(BookSearchDto dto)
            => _bookRepo.SearchAsync(dto);

        public Task<BookDetailsDto> GetBookDetails(int id)
            => GetBookById(id);
    }
}
