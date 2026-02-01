using LibrarySystem.Common.DTOs.Library.BookCopies;
using LibrarySystem.Domain.Repositories.IRepo;
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
            // 👇 لازم تجيب الكتاب
            var book = await _bookRepo.GetRequiredByIdAsync(dto.BookId);

            // 👇 وتبعثه
            await _copyRepo.AddCopyAsync(dto, book);

            await _bookRepo.IncrementCopiesAsync(dto.BookId);
        }

        public async Task DeleteBookCopy(int id)
        {
            var minimal = await _copyRepo.GetByIdWithBookIdAsync(id)
                ?? throw new Exception("Copy not found");

            if (!minimal.IsAvailable)
                throw new Exception("Cannot delete a borrowed copy");

            await _copyRepo.DeleteCopyAsync(id);
            await _bookRepo.DecrementCopiesAsync(minimal.BookId);
        }

        public Task<List<BookCopyListDto>> ListBookCopies()
            => _copyRepo.GetAllListAsync();

        public Task<BookCopy> GetSpecificCopy(int id)
            => _copyRepo.GetRequiredCopyAsync(id);

        public Task<int> GetAllCopiesCount(int bookId)
            => _copyRepo.CountByBookAsync(bookId);

        public Task<int> GetAvailableCount(int bookId)
            => _copyRepo.CountAvailableAsync(bookId);

        public Task<int> GetBorrowedCount(int bookId)
            => _copyRepo.CountBorrowedAsync(bookId);
    }
}
