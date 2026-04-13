using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Areas.Accounts.Models;

public class ProfileModel
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
