using ASC.Web.Models;
using ASC.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.ViewComponents;

public class LeftNavigationViewComponent : ViewComponent
{
    private readonly INavigationCacheOperations _navigationCache;

    public LeftNavigationViewComponent(INavigationCacheOperations navigationCache)
    {
        _navigationCache = navigationCache;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userRoles = HttpContext.User.Claims
            .Where(c => c.Type.EndsWith("role", StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var menu = await _navigationCache.GetNavigationMenuItems();
        var filteredItems = FilterByRole(menu, userRoles);

        return View("~/Views/Shared/LeftNavigation.cshtml", filteredItems);
    }

    private static List<MenuItem> FilterByRole(IEnumerable<MenuItem> items, IReadOnlyCollection<string> userRoles)
    {
        return items
            .Where(item => item.UserRoles.Count == 0 || item.UserRoles.Any(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase)))
            .OrderBy(item => item.Sequence)
            .Select(item => new MenuItem
            {
                DisplayName = item.DisplayName,
                MaterialIcon = item.MaterialIcon,
                Link = item.Link,
                IsNested = item.IsNested,
                Sequence = item.Sequence,
                UserRoles = item.UserRoles,
                NestedItems = FilterByRole(item.NestedItems, userRoles)
            })
            .ToList();
    }
}
