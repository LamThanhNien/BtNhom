namespace ASC.Web.Models.Navigation;

public class NavigationItem
{
    public string DisplayName { get; set; } = string.Empty;
    public string MaterialIcon { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public bool IsNested { get; set; }
    public int Sequence { get; set; }
    public List<string> UserRoles { get; set; } = [];
    public List<NavigationItem> NestedItems { get; set; } = [];
}

public class NavigationRoot
{
    public List<NavigationItem> MenuItems { get; set; } = [];
}
