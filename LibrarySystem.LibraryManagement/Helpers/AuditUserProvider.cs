using System.Security.Claims;
using LibrarySystem.Common.Helpers.Auth;
using LibrarySystem.Domain.Abstractions;
using LibrarySystem.Domain.Repositories.IRepo;
using Microsoft.AspNetCore.Http;

namespace LibrarySystem.Helper.Api
{

    public class AuditUserProvider : IAuditUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public AuditUserProvider(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public int? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                return null;

            var externalUserId = user.TryGetExternalUserId();
            if (externalUserId == null)
                return null;

           
            using var scope = _serviceProvider.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var localUser = userRepository.GetByExternalIdAsync(externalUserId.Value).GetAwaiter().GetResult();
            return localUser?.Id;
        }
    }
}
