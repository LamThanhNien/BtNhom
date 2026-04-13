using System.Text.Json;
using ASC.Web.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ASC.Web.Services;

public class NavigationCacheOperations : INavigationCacheOperations
{
    public const string NavigationCacheKey = "NavigationMenu";

    private readonly IWebHostEnvironment _environment;
    private readonly IMemoryCache _cache;
    private readonly ILogger<NavigationCacheOperations> _logger;

    public NavigationCacheOperations(IWebHostEnvironment environment, IMemoryCache cache, ILogger<NavigationCacheOperations> logger)
    {
        _environment = environment;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<MenuItem>> GetNavigationMenuItems()
    {
        if (_cache.TryGetValue(NavigationCacheKey, out List<MenuItem>? cachedMenu) && cachedMenu is not null)
        {
            return cachedMenu;
        }

        var path = Path.Combine(_environment.ContentRootPath, "Navigation.json");
        if (!File.Exists(path))
        {
            _logger.LogWarning("Navigation file not found at {Path}", path);
            return new List<MenuItem>();
        }

        var json = await File.ReadAllTextAsync(path);
        var menu = JsonSerializer.Deserialize<NavigationMenu>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var menuItems = menu?.MenuItems
            .OrderBy(i => i.Sequence)
            .ToList() ?? new List<MenuItem>();

        _cache.Set(NavigationCacheKey, menuItems, TimeSpan.FromHours(12));

        return menuItems;
    }
}
