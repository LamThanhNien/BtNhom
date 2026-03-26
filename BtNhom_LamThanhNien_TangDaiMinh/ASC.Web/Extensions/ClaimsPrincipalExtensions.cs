using ASC.Web.Models;
using System.Security.Claims;

namespace ASC.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static CurrentUser GetCurrentUser(this ClaimsPrincipal principal)
    {
        return new CurrentUser
        {
            UserId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
            Email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            FullName = principal.Identity?.Name ?? string.Empty,
            Roles = principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList()
        };
    }
}
