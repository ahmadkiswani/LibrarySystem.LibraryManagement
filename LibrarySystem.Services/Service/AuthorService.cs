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
        public async Task AddAuthor(AuthorCreateDto dto)
        {
            await _authorRepo.AddAuthorAsync(dto);
        }


        public async Task DeleteAuthor(int id)
        {
            await _authorRepo.SoftDeleteByIdAsync(id);
        }

        public async Task EditAuthor(int id, AuthorUpdateDto dto)
        {
            await _authorRepo.UpdateNameAsync(id, dto.AuthorName);
        }

        public async Task<List<AuthorListDto>> GetAllAuthors()
        {
            return await _authorRepo.GetAllListAsync();
        }

        public async Task<AuthorDetailsDto> GetAuthorById(int id)
        {
            return await _authorRepo.GetDetailsAsync(id);
        }

        public async Task<List<AuthorListDto>> Search(AuthorSearchDto dto)
        {
            return await _authorRepo.SearchAsync(dto);
        }

    }
}