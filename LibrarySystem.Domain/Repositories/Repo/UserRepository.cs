using LibrarySystem.Common.DTOs.Library.Users;
using LibrarySystem.Domain.Data;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class UserRepository
        : GenericRepository<User>, IUserRepository
    {
        public UserRepository(LibraryDbContext context)
            : base(context)
        {
        }

        public async Task ApplyUserCreatedAsync(UserCreateDto dto)
        {
            if (await ExistsByExternalIdAsync(dto.ExternalUserId))
                return;

            var user = new User
            {
                ExternalUserId = dto.ExternalUserId,
                UserName = dto.UserName,
                UserEmail = dto.UserEmail
            };

            await AddAsync(user);
        }

        public async Task ApplyUserUpdatedAsync(int externalUserId, UserUpdateDto dto)
        {
            var user = await GetByExternalIdAsync(externalUserId);
            if (user == null) return;

            user.UserName = dto.UserName;
            user.UserEmail = dto.UserEmail;

            await UpdateAsync(user);
        }

        public async Task ApplyUserDeactivatedAsync(int externalUserId)
        {
            var user = await GetByExternalIdAsync(externalUserId);
            if (user == null) return;

            await SoftDeleteAsync(user);
        }

        public async Task<List<UserListDto>> GetAllListAsync()
        {
            return await GetQueryable()
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    UserName = u.UserName
                })
                .ToListAsync();
        }

        public async Task<User> GetRequiredByIdAsync(int id)
        {
            return await GetByIdAsync(id)
                ?? throw new Exception("User not found");
        }

        public async Task<User?> GetByExternalIdAsync(int externalUserId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.ExternalUserId == externalUserId);
        }

        public async Task<bool> ExistsByExternalIdAsync(int externalUserId)
        {
            return await _dbSet
                .AnyAsync(u => u.ExternalUserId == externalUserId);
        }
    }
}
