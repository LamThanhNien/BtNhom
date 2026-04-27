using System.Text;
using ASC.Web.Configuration;
using ASC.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace ASC.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ApplicationSettings _applicationSettings;

    public ForgotPasswordModel(
        UserManager<IdentityUser> userManager,
        IEmailSender emailSender,
        IOptions<ApplicationSettings> options)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _applicationSettings = options.Value;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user is null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            return RedirectToPage("./ForgotPasswordConfirmation");
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ResetPassword",
            pageHandler: null,
            values: new { area = "Identity", code, email = Input.Email },
            protocol: Request.Scheme);

        var appName = string.IsNullOrWhiteSpace(_applicationSettings.ApplicationName)
            ? "ASC"
            : _applicationSettings.ApplicationName;

        await _emailSender.SendEmailAsync(
            Input.Email,
            $"{appName} - Reset Password",
            $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

        return RedirectToPage("./ForgotPasswordConfirmation");
    }
}
