using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Repositories.IRepo
{
    public interface IAuthorRepository
    {
        Task AddAsync(Author author);
        Task UpdateAsync(Author author);
        Task SoftDeleteAsync(Author author);

        Task<Author?> GetByIdAsync(int id);
        Task<List<Author>> GetAllAsync();

        Task<List<Author>> SearchAsync(string? text, int? number, int page, int pageSize);

        Task<bool> ExistsAsync(int authorId);
    }
}
