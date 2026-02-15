using LibrarySystem.Common.Repositories;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly IRepository<Author> _authorRepo;

        public AuthorRepository(IRepository<Author> authorRepo)
        {
            _authorRepo = authorRepo;
        }

        public async Task AddAsync(Author author)
        {
            await _authorRepo.AddAsync(author);
            await _authorRepo.SaveAsync();
        }

        public async Task UpdateAsync(Author author)
        {
            await _authorRepo.UpdateAsync(author);
            await _authorRepo.SaveAsync();
        }

        public async Task SoftDeleteAsync(Author author)
        {
            _authorRepo.SoftDelete(author);
            await _authorRepo.SaveAsync();
        }

        public async Task<Author?> GetByIdAsync(int id)
        {
            return await _authorRepo.GetFirstAsync(a => a.Id == id);
        }

        public async Task<List<Author>> GetAllAsync()
        {
            return await _authorRepo
                .GetQueryable()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Author>> SearchAsync(
            string? text,
            int? number,
            int page,
            int pageSize)
        {
            var query = _authorRepo
                .GetQueryable()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(text))
                query = query.Where(a => a.AuthorName.Contains(text));

            if (number.HasValue)
                query = query.Where(a => a.Id == number.Value);

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountForSearchAsync(string? text, int? number)
        {
            var query = _authorRepo.GetQueryable().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(text))
                query = query.Where(a => a.AuthorName.Contains(text));

            if (number.HasValue)
                query = query.Where(a => a.Id == number.Value);

            return await query.CountAsync();
        }

        public Task<bool> ExistsAsync(int authorId)
        {
            return _authorRepo
                .GetQueryable()
                .AsNoTracking()
                .AnyAsync(a => a.Id == authorId);
        }

    }
}
