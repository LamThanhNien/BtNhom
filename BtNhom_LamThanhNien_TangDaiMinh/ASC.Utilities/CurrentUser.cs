namespace ASC.Utilities;

public class CurrentUser
{
    public const string SessionKey = "CurrentUser";

    public string UserId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = new();
}
