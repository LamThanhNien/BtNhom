namespace ASC.Web.Models;

public class EmailSettings
{
    // Lab format
    public string Mail { get; set; } = string.Empty;
    public string Host { get; set; } = "smtp.gmail.com";

    // Extended format
    public string SmtpServer { get; set; } = "smtp.gmail.com";

    public int Port { get; set; } = 587;

    public bool UseSsl { get; set; }

    public string SenderName { get; set; } = "ASC Support";

    public string SenderEmail { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
