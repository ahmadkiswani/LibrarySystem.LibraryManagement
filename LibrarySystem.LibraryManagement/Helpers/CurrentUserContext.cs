using LibrarySystem.Domain.Repositories.IRepo;
using LibrarySystem.Services.Interfaces;
using System.Security.Claims;
using LibrarySystem.Common.Helpers.Auth;
using Microsoft.AspNetCore.Http;


namespace LibrarySystem.Helper.Api
{
    public class CurrentUserContext : ICurrentUserContext
    {
        public int ExternalUserId { get; }
        public int LocalUserId { get; }

        public CurrentUserContext(
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepo)
        {
            var user = httpContextAccessor.HttpContext?.User
                ?? throw new Exception("No HttpContext");

            ExternalUserId = user.TryGetExternalUserId()
                ?? throw new Exception("UserId claim missing");

            var localUser = userRepo
                .GetByExternalIdAsync(ExternalUserId)
                .GetAwaiter()
                .GetResult()
                ?? throw new Exception("User not synced");

            LocalUserId = localUser.Id;
        }
    }
}
