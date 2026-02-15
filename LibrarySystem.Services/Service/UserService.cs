using LibrarySystem.Common.DTOs.Library.Helpers;
using LibrarySystem.Common.DTOs.Library.Users;
using LibrarySystem.Common.Helpers;
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

        public Task ApplyUserReactivatedEvent(int externalUserId)
            => _userRepo.ApplyUserReactivatedAsync(externalUserId);

        public Task<List<UserListDto>> ListUsers()
            => _userRepo.GetAllListAsync();

        public async Task<PagedResultDto<UserListDto>> Search(UserSearchDto dto)
        {
            int page = dto.Page <= 0 ? 1 : dto.Page;
            int pageSize = dto.PageSize <= 0 || dto.PageSize > 200 ? 10 : dto.PageSize;
            AppHelper.NormalizePage(ref page, ref pageSize, 10, 200);

            var users = await _userRepo.SearchAsync(dto.Text, dto.Number, dto.Status, page, pageSize);
            var totalCount = await _userRepo.CountForSearchAsync(dto.Text, dto.Number, dto.Status);
            var info = AppHelper.BuildPagingInfo(totalCount, page, pageSize);

            return new PagedResultDto<UserListDto>
            {
                Data = users,
                TotalCount = (int)info.TotalCount,
                Page = info.Page,
                PageSize = info.PageSize,
                TotalPages = info.TotalPages,
                HasNextPage = info.HasNextPage
            };
        }

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
