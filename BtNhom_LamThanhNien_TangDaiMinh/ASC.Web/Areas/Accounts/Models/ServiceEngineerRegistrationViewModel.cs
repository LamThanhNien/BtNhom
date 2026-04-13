using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Areas.Accounts.Models;

public class ServiceEngineerRegistrationViewModel
{
    public string? Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public bool IsActive { get; set; } = true;
}
