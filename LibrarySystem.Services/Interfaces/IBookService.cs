
using LibrarySystem.Common.DTOs.Library.Books;

namespace LibrarySystem.Services.Interfaces
{
    public interface IBookService
    {
        Task<int> AddBook(BookCreateDto dto);
        Task<List<BookListDto>> GetAllBooks();
        Task<BookDetailsDto> GetBookById(int id);
        Task EditBook(int id, BookUpdateDto dto);
        Task DeleteBook(int id);
        Task<List<BookListDto>> SearchBooks(BookSearchDto dto);
        Task<BookDetailsDto> GetBookDetails(int id);


    }
}
