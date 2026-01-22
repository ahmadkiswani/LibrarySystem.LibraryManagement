using LibrarySystem.Common.DTOs.Library.Publishers;
using LibrarySystem.Domain.Data;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class PublisherRepository
        : GenericRepository<Publisher>, IPublisherRepository
    {
        public PublisherRepository(LibraryDbContext context)
            : base(context)
        {
        }

        public async Task AddPublisherAsync(PublisherCreateDto dto)
        {
            if (await ExistsByNameAsync(dto.Name))
                throw new Exception("Publisher already exists");

            var publisher = new Publisher
            {
                Name = dto.Name
            };

            await AddAsync(publisher);
        }

        public async Task UpdatePublisherAsync(int id, PublisherUpdateDto dto)
        {
            var publisher = await GetByIdAsync(id)
                ?? throw new Exception("Publisher not found");

            publisher.Name = dto.Name;
            await UpdateAsync(publisher);
        }

        public async Task SoftDeleteByIdAsync(int id)
        {
            var publisher = await GetByIdAsync(id)
                ?? throw new Exception("Publisher not found");

            await SoftDeleteAsync(publisher);
        }

        public async Task<List<PublisherListDto>> GetAllListAsync()
        {
            return await GetQueryable()
                .Select(p => new PublisherListDto
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
        }

        public async Task<Publisher> GetRequiredByIdAsync(int id)
        {
            return await GetByIdAsync(id)
                ?? throw new Exception("Publisher not found");
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(p => p.Name == name);
        }
    }
}
