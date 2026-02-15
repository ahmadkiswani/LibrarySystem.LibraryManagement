using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Common.DTOs.Library.Users;

namespace LibrarySystem.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserListDto>> ListUsers();
        Task<PagedResultDto<UserListDto>> Search(UserSearchDto dto);
        Task<UserDetailsDto> GetUserDetails(int id);
        Task ApplyUserCreatedEvent(UserCreateDto dto);
        Task ApplyUserUpdatedEvent(int externalUserId, UserUpdateDto dto);
        Task ApplyUserDeactivatedEvent(int externalUserId);
        Task ApplyUserReactivatedEvent(int externalUserId);
    }
}
