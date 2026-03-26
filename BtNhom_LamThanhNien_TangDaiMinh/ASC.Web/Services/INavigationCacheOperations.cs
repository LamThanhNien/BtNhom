using ASC.Web.Models.Navigation;
using System.Security.Claims;

namespace ASC.Web.Services;

public interface INavigationCacheOperations
{
    Task<List<NavigationItem>> GetMenuByRoleAsync(ClaimsPrincipal user);
}
