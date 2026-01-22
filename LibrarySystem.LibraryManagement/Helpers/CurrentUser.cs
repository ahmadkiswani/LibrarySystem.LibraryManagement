using System.Security.Claims;

namespace LibrarySystem.LibraryManagement.Helpers;

public static class CurrentUser
{
    public static int GetUserId(ClaimsPrincipal user)
    {
       
        var id =
            user.FindFirstValue(ClaimTypes.NameIdentifier) ??
            user.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(id))
            throw new Exception("UserId claim not found in token");

        return int.Parse(id);
    }
}
