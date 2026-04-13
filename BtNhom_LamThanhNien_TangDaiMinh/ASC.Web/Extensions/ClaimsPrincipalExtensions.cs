using System.Security.Claims;
using ASC.Utilities;

namespace ASC.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static CurrentUser ToCurrentUser(this ClaimsPrincipal principal)
    {
        return new CurrentUser
        {
            UserId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
            UserName = principal.Identity?.Name ?? string.Empty,
            Email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        };
    }
}
