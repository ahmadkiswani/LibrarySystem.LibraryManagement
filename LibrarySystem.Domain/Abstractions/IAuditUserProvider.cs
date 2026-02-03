namespace LibrarySystem.Domain.Abstractions
{
    /// <summary>
    /// Provides the current user id for audit fields (CreatedBy, LastModifiedBy, DeletedBy).
    /// Implemented in the API layer using JWT claims / ICurrentUserContext.
    /// </summary>
    public interface IAuditUserProvider
    {
        int? GetCurrentUserId();
    }
}
