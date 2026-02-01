using LibrarySystem.Common.DTOs.Library.Authors;
using LibrarySystem.Domain.Helper;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using LibrarySystem.Services.Interfaces;

namespace LibrarySystem.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepo;

        public AuthorService(IAuthorRepository authorRepo)
        {
            _authorRepo = authorRepo;
        }

        public async Task AddAuthor(int userId, AuthorCreateDto dto)
        {
            var author = new Author
            {
                AuthorName = dto.AuthorName
            };

            AuditHelper.OnCreate(author);
            await _authorRepo.AddAsync(author);
        }

        public async Task EditAuthor(int userId, int id, AuthorUpdateDto dto)
        {
            var author = await _authorRepo.GetByIdAsync(id)
                ?? throw new Exception("Author not found");

            author.AuthorName = dto.AuthorName;
            AuditHelper.OnUpdate(author, userId);

            await _authorRepo.UpdateAsync(author);
        }

        public async Task DeleteAuthor(int userId, int id)
        {
            var author = await _authorRepo.GetByIdAsync(id)
                ?? throw new Exception("Author not found");

            AuditHelper.OnSoftDelete(author, userId);
            await _authorRepo.SoftDeleteAsync(author);
        }

        public async Task<List<AuthorListDto>> GetAllAuthors()
        {
            var authors = await _authorRepo.GetAllAsync();

            return authors.Select(a => new AuthorListDto
            {
                Id = a.Id,
                AuthorName = a.AuthorName
            }).ToList();
        }

        public async Task<AuthorDetailsDto> GetAuthorById(int id)
        {
            var author = await _authorRepo.GetByIdAsync(id)
                ?? throw new Exception("Author not found");

            return new AuthorDetailsDto
            {
                Id = author.Id,
                AuthorName = author.AuthorName
            };
        }

        public async Task<List<AuthorListDto>> Search(AuthorSearchDto dto)
        {
            int page = dto.Page <= 0 ? 1 : dto.Page;
            int pageSize = dto.PageSize <= 0 || dto.PageSize > 200 ? 10 : dto.PageSize;

            var authors = await _authorRepo.SearchAsync(
                dto.Text,
                dto.Number,
                page,
                pageSize);

            return authors.Select(a => new AuthorListDto
            {
                Id = a.Id,
                AuthorName = a.AuthorName
            }).ToList();
        }
    }
}
