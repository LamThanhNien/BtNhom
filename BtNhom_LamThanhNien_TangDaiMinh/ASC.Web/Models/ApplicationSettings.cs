namespace ASC.Web.Models;

public class ApplicationSettings
{
    public string ApplicationName { get; set; } = "Automobile Service Center";

    public string Version { get; set; } = "1.0.0";

    public string SupportEmail { get; set; } = string.Empty;

    public string ApplicationTitle { get; set; } = "Automobile Service Center";

    public string WebUrl { get; set; } = "https://localhost:7246";

    public string AdminName { get; set; } = "ASC Admin";

    public string AdminEmail { get; set; } = "admin@asc.com";

    public string AdminPassword { get; set; } = "Admin@123";

    public string EngineerEmail { get; set; } = "engineer@asc.com";

    public string EngineerPassword { get; set; } = "Engineer@123";

    public string UserEmail { get; set; } = "user@asc.com";

    public string UserPassword { get; set; } = "User@123";
}
