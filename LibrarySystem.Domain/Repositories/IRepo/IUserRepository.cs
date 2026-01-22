using LibrarySystem.Common.DTOs.Library.Users;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Repositories.IRepo
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task ApplyUserCreatedAsync(UserCreateDto dto);
        Task ApplyUserUpdatedAsync(int externalUserId, UserUpdateDto dto);
        Task ApplyUserDeactivatedAsync(int externalUserId);

        Task<User> GetRequiredByIdAsync(int id);
        Task<List<UserListDto>> GetAllListAsync();

        Task<User?> GetByExternalIdAsync(int externalUserId);
        Task<bool> ExistsByExternalIdAsync(int externalUserId);
    }
}
