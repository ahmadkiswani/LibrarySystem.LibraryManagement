
using LibrarySystem.Common.DTOs.Library.Users;

namespace LibrarySystem.Services.Interfaces
{
    public interface IUserService
    {
    
        Task<List<UserListDto>> ListUsers();
        Task<UserDetailsDto> GetUserDetails(int id);
        Task ApplyUserCreatedEvent(UserCreateDto dto);
        Task ApplyUserUpdatedEvent(int externalUserId, UserUpdateDto dto);
        Task ApplyUserDeactivatedEvent(int externalUserId);

    }
}
