namespace ASC.Web.Models;

public class NavigationMenu
{
    public List<MenuItem> MenuItems { get; set; } = new();
}

public class MenuItem
{
    public string DisplayName { get; set; } = string.Empty;

    public string MaterialIcon { get; set; } = string.Empty;

    public string Link { get; set; } = string.Empty;

    public bool IsNested { get; set; }

    public int Sequence { get; set; }

    public List<string> UserRoles { get; set; } = new();

    public List<MenuItem> NestedItems { get; set; } = new();
}
