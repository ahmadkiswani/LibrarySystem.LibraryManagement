using LibrarySystem.Common.DTOs.Library.Books;
using LibrarySystem.Common.Helpers;
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

        public async Task<BookSearchResultDto> SearchBooks(BookSearchDto dto)
        {
            int page = dto.Page ?? 1;
            int pageSize = dto.PageSize ?? 10;
            AppHelper.NormalizePage(ref page, ref pageSize, defaultPageSize: 10, maxPageSize: 200);

            var data = await _bookRepo.SearchAsync(dto);
            var totalCount = await _bookRepo.CountForSearchAsync(dto);

            var info = AppHelper.BuildPagingInfo(totalCount, page, pageSize);

            return new BookSearchResultDto
            {
                Data = data,
                TotalCount = (int)info.TotalCount,
                Page = info.Page,
                PageSize = info.PageSize,
                TotalPages = info.TotalPages,
                HasNextPage = info.HasNextPage
            };
        }

        public Task<BookDetailsDto> GetBookDetails(int id)
            => GetBookById(id);
    }
}
