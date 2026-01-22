using LibrarySystem.Common.DTOs.Library.Authors;
using LibrarySystem.Domain.Data;
using LibrarySystem.Domain.Helper;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(LibraryDbContext context)
            : base(context)
        {
        }
        public async Task AddAuthorAsync(AuthorCreateDto dto)
        {
            var author = new Author
            {
                AuthorName = dto.AuthorName
            };

            AuditHelper.OnCreate(author);

            await AddAsync(author);
        }

        public async Task SoftDeleteByIdAsync(int id)
        {
            var author = await GetByIdAsync(id);
            if (author == null)
                throw new Exception("Author not found");

            AuditHelper.OnSoftDelete(author, id);
            await SoftDeleteAsync(author);
        }

        public async Task UpdateNameAsync(int id, string authorName)
        {
            var author = await GetByIdAsync(id);
            if (author == null)
                throw new Exception("Author not found");

            author.AuthorName = authorName;
            AuditHelper.OnUpdate(author, id);

            await UpdateAsync(author);
        }

        public async Task<List<AuthorListDto>> GetAllListAsync()
        {
            return await GetQueryable()
                .Select(a => new AuthorListDto
                {
                    Id = a.Id,
                    AuthorName = a.AuthorName
                })
                .ToListAsync();
        }

        public async Task<AuthorDetailsDto> GetDetailsAsync(int id)
        {
            return await GetQueryable()
                .Where(a => a.Id == id)
                .Select(a => new AuthorDetailsDto
                {
                    Id = a.Id,
                    AuthorName = a.AuthorName
                })
                .FirstOrDefaultAsync()
                ?? throw new Exception("Author not found");
        }

        public async Task<List<AuthorListDto>> SearchAsync(AuthorSearchDto dto)
        {
            var query = GetQueryable();

            if (!string.IsNullOrWhiteSpace(dto.Text))
                query = query.Where(a => a.AuthorName.Contains(dto.Text));

            if (dto.Number.HasValue)
                query = query.Where(a => a.Id == dto.Number.Value);

            int page = dto.Page <= 0 ? 1 : dto.Page;
            int pageSize = dto.PageSize <= 0 || dto.PageSize > 200 ? 10 : dto.PageSize;

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuthorListDto
                {
                    Id = a.Id,
                    AuthorName = a.AuthorName
                })
                .ToListAsync();
        }
    }
}
