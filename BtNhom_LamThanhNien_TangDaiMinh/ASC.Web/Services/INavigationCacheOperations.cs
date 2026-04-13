using ASC.Web.Models;

namespace ASC.Web.Services;

public interface INavigationCacheOperations
{
    Task<List<MenuItem>> GetNavigationMenuItems();
}
