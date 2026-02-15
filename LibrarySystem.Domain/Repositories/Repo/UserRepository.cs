using LibrarySystem.Common.DTOs.Library.Users;
using LibrarySystem.Common.Repositories;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Domain.Repositories.Repo
{
    public class UserRepository : IUserRepository
    {
        private readonly IRepository<User> _repo;

        public UserRepository(IRepository<User> repo)
        {
            _repo = repo;
        }

        public async Task ApplyUserCreatedAsync(UserCreateDto dto)
        {
            if (await ExistsByExternalIdAsync(dto.ExternalUserId))
                return;

            await _repo.AddAsync(new User
            {
                ExternalUserId = dto.ExternalUserId,
                UserName = dto.UserName,
                UserEmail = dto.UserEmail,
                RoleName = dto.RoleName
            });

            await _repo.SaveAsync();
        }

        public async Task ApplyUserUpdatedAsync(int externalUserId, UserUpdateDto dto)
        {
            var user = await GetByExternalIdAsync(externalUserId);
            if (user == null) return;

            user.UserName = dto.UserName;
            user.UserEmail = dto.UserEmail;
            user.RoleName = dto.RoleName;
            await _repo.UpdateAsync(user);
            await _repo.SaveAsync();
        }

        public async Task ApplyUserDeactivatedAsync(int externalUserId)
        {
            var user = await GetByExternalIdAsync(externalUserId);
            if (user == null) return;

            _repo.SoftDelete(user);
            await _repo.SaveAsync();
        }

        public async Task ApplyUserReactivatedAsync(int externalUserId)
        {
            var user = await GetByExternalIdAsync(externalUserId);
            if (user == null) return;

            if (user.IsDeleted)
            {
                user.IsDeleted = false;
                user.DeletedDate = null;
                user.DeletedBy = null;
                await _repo.UpdateAsync(user);
                await _repo.SaveAsync();
            }
        }

        public Task<List<UserListDto>> GetAllListAsync()
            => _repo.GetQueryable()
                .AsNoTracking()
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    UserTypeName = u.RoleName,
                    IsDeleted = u.IsDeleted
                })
                .ToListAsync();

        public async Task<List<UserListDto>> SearchAsync(string? text, int? number, string? status, int page, int pageSize)
        {
            var query = _repo.GetQueryable().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(text))
                query = query.Where(u => u.UserName.Contains(text) || (u.UserEmail != null && u.UserEmail.Contains(text)));

            if (number.HasValue)
                query = query.Where(u => u.Id == number.Value || u.ExternalUserId == number.Value);

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(u => !u.IsDeleted);
                else if (string.Equals(status, "deactivated", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(u => u.IsDeleted);
            }

            return await query
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    UserTypeName = u.RoleName,
                    IsDeleted = u.IsDeleted
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountForSearchAsync(string? text, int? number, string? status)
        {
            var query = _repo.GetQueryable().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(text))
                query = query.Where(u => u.UserName.Contains(text) || (u.UserEmail != null && u.UserEmail.Contains(text)));

            if (number.HasValue)
                query = query.Where(u => u.Id == number.Value || u.ExternalUserId == number.Value);

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(u => !u.IsDeleted);
                else if (string.Equals(status, "deactivated", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(u => u.IsDeleted);
            }

            return await query.CountAsync();
        }

        public async Task<User> GetRequiredByIdAsync(int id)
            => await _repo.GetByIdAsync(id)
                ?? throw new Exception("User not found");

        public Task<User?> GetByExternalIdAsync(int externalUserId)
            => _repo.GetQueryable()
                .FirstOrDefaultAsync(u => u.ExternalUserId == externalUserId);

        public Task<bool> ExistsByExternalIdAsync(int externalUserId)
            => _repo.GetQueryable().AnyAsync(u => u.ExternalUserId == externalUserId);
    }
}
