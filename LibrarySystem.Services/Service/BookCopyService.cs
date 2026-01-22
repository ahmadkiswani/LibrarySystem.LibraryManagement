using LibrarySystem.Common.DTOs.Library.BookCopies;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Domain.Repositories.Repo;
using LibrarySystem.Entities.Models;
using LibrarySystem.Services.Interfaces;

namespace LibrarySystem.Services
{
    public class BookCopyService : IBookCopyService
    {
        private readonly IBookCopyRepository _copyRepo;
        private readonly IBookRepository _bookRepo;

        public BookCopyService(
            IBookCopyRepository copyRepo,
            IBookRepository bookRepo)
        {
            _copyRepo = copyRepo;
            _bookRepo = bookRepo;
        }

        public async Task AddBookCopy(BookCopyCreateDto dto)
        {
            var book = await _bookRepo.GetByIdAsync(dto.BookId);
            if (book == null)
                throw new Exception("Book does not exist");

            await _copyRepo.AddCopyAsync(dto, book);

            book.TotalCopies += 1;
            await _bookRepo.UpdateAsync(book);
        }

        public async Task DeleteBookCopy(int id)
        {
            await _copyRepo.SoftDeleteByIdAsync(id);
        }

        public async Task<List<BookCopyListDto>> ListBookCopies()
        {
            return await _copyRepo.GetAllListAsync();
        }

        public async Task<BookCopy> GetSpecificCopy(int id)
        {
            return await _copyRepo.GetRequiredCopyAsync(id);
        }

        public Task<int> GetAllCopiesCount(int bookId)
            => _copyRepo.CountByBookAsync(bookId);

        public Task<int> GetAvailableCount(int bookId)
            => _copyRepo.CountAvailableAsync(bookId);

        public Task<int> GetBorrowedCount(int bookId)
            => _copyRepo.CountBorrowedAsync(bookId);
    }
}
