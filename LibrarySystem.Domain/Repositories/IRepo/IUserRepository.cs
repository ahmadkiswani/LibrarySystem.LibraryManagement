using LibrarySystem.Common.DTOs.Library.Users;
using LibrarySystem.Entities.Models;

namespace LibrarySystem.Domain.Repositories.IRepo
{
    public interface IUserRepository 
    {
        Task ApplyUserCreatedAsync(UserCreateDto dto);
        Task ApplyUserUpdatedAsync(int externalUserId, UserUpdateDto dto);
        Task ApplyUserDeactivatedAsync(int externalUserId);
        Task ApplyUserReactivatedAsync(int externalUserId);

        Task<User> GetRequiredByIdAsync(int id);
        Task<List<UserListDto>> GetAllListAsync();
        Task<List<UserListDto>> SearchAsync(string? text, int? number, string? status, int page, int pageSize);
        Task<int> CountForSearchAsync(string? text, int? number, string? status);

        Task<User?> GetByExternalIdAsync(int externalUserId);
        Task<bool> ExistsByExternalIdAsync(int externalUserId);
    }
}
