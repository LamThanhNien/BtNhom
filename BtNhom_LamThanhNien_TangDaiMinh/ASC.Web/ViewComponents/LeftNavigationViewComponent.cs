using ASC.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.ViewComponents;

public class LeftNavigationViewComponent : ViewComponent
{
    private readonly INavigationCacheOperations _navigation;

    public LeftNavigationViewComponent(INavigationCacheOperations navigation)
    {
        _navigation = navigation;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = await _navigation.GetMenuByRoleAsync(HttpContext.User);
        return View(items);
    }
}
