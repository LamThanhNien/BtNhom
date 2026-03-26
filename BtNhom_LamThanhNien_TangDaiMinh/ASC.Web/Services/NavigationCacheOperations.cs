using ASC.Web.Models.Navigation;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text.Json;

namespace ASC.Web.Services;

public class NavigationCacheOperations : INavigationCacheOperations
{
    private readonly IMemoryCache _cache;
    private readonly IWebHostEnvironment _environment;

    public NavigationCacheOperations(IMemoryCache cache, IWebHostEnvironment environment)
    {
        _cache = cache;
        _environment = environment;
    }

    public async Task<List<NavigationItem>> GetMenuByRoleAsync(ClaimsPrincipal user)
    {
        var menu = await GetNavigationFromCacheAsync();
        var roles = user.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();

        return menu
            .Where(m => m.UserRoles.Count == 0 || roles.Any(r => m.UserRoles.Contains(r)))
            .OrderBy(m => m.Sequence)
            .ToList();
    }

    private async Task<List<NavigationItem>> GetNavigationFromCacheAsync()
    {
        const string key = "ASC_NAVIGATION";
        if (_cache.TryGetValue(key, out List<NavigationItem>? menu) && menu is not null)
            return menu;

        var file = Path.Combine(_environment.ContentRootPath, "Navigation.json");
        var json = await File.ReadAllTextAsync(file);
        var root = JsonSerializer.Deserialize<NavigationRoot>(json) ?? new NavigationRoot();
        menu = root.MenuItems;

        _cache.Set(key, menu, TimeSpan.FromMinutes(30));
        return menu;
    }
}
