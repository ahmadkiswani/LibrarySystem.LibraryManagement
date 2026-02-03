using System.Security.Claims;
using LibrarySystem.Domain.Abstractions;
using LibrarySystem.Domain.Repositories.IRepo;
using Microsoft.AspNetCore.Http;

namespace LibrarySystem.Helper.Api
{
    /// <summary>
    /// Provides the current user id for audit fields from JWT (NameIdentifier -> Library User Id).
    /// Returns null when there is no HTTP context or user (e.g. MassTransit consumer).
    /// Resolves IUserRepository from a new scope to avoid circular dependency with LibraryDbContext.
    /// </summary>
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

            var externalId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            if (string.IsNullOrWhiteSpace(externalId) || !int.TryParse(externalId, out var externalUserId))
                return null;

            // Resolve IUserRepository from a new scope to avoid circular dependency:
            // LibraryDbContext -> IAuditUserProvider -> IUserRepository -> Repository<User> -> LibraryDbContext
            using var scope = _serviceProvider.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var localUser = userRepository.GetByExternalIdAsync(externalUserId).GetAwaiter().GetResult();
            return localUser?.Id;
        }
    }
}
