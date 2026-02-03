using LibrarySystem.Common.DTOs.Library.Users;
using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Services.Interfaces;

namespace LibrarySystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public Task ApplyUserCreatedEvent(UserCreateDto dto)
            => _userRepo.ApplyUserCreatedAsync(dto);

        public Task ApplyUserUpdatedEvent(int externalUserId, UserUpdateDto dto)
            => _userRepo.ApplyUserUpdatedAsync(externalUserId, dto);

        public Task ApplyUserDeactivatedEvent(int externalUserId)
            => _userRepo.ApplyUserDeactivatedAsync(externalUserId);

        public Task<List<UserListDto>> ListUsers()
            => _userRepo.GetAllListAsync();

        public async Task<UserDetailsDto> GetUserDetails(int id)
        {
            var user = await _userRepo.GetRequiredByIdAsync(id);

            return new UserDetailsDto
            {
                Id = user.Id,
                ExternalUserId = user.ExternalUserId,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                RoleName = user.RoleName
            };
        }
    }
}
