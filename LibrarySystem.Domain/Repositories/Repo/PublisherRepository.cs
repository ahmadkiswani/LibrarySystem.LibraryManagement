using LibrarySystem.Common.DTOs.Library.Publishers;
using LibrarySystem.Common.Repositories;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class PublisherRepository : IPublisherRepository
    {
        private readonly IRepository<Publisher> _repoP;

        public PublisherRepository(IRepository<Publisher> repo)
        {
            _repoP = repo;
        }

        public async Task AddPublisherAsync(PublisherCreateDto dto)
        {
            if (await ExistsByNameAsync(dto.Name))
                throw new Exception("Publisher already exists");

            await _repoP.AddAsync(new Publisher { Name = dto.Name });
            await _repoP.SaveAsync();
        }

        public async Task UpdatePublisherAsync(int id, PublisherUpdateDto dto)
        {
            var publisher = await _repoP.GetByIdAsync(id)
                ?? throw new Exception("Publisher not found");

            publisher.Name = dto.Name;
            await _repoP.UpdateAsync(publisher);
            await _repoP.SaveAsync();
        }

        public async Task SoftDeleteByIdAsync(int id)
        {
            var publisher = await _repoP.GetByIdAsync(id)
                ?? throw new Exception("Publisher not found");

            _repoP.SoftDelete(publisher);
            await _repoP.SaveAsync();
        }

        public Task<List<PublisherListDto>> GetAllListAsync()
            => _repoP.GetQueryable()
                .AsNoTracking()
                .Select(p => new PublisherListDto
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();

        public async Task<Publisher> GetRequiredByIdAsync(int id)
            => await _repoP.GetByIdAsync(id)
                ?? throw new Exception("Publisher not found");

        public Task<bool> ExistsByNameAsync(string name)
            => _repoP.GetQueryable().AnyAsync(p => p.Name == name);



        public Task<bool> ExistsAsync(int id)
        {
            return _repoP
                .GetQueryable()
                .AsNoTracking()
                .AnyAsync(p => p.Id == id);
        }

    }
}
