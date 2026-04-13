using ASC.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASC.Web.Areas.Identity.Pages.Account;

[Authorize]
public class LogoutModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        return RedirectToPage("./Login");
    }

    public async Task<IActionResult> OnPost()
    {
        await _signInManager.SignOutAsync();
        HttpContext.Session.Remove(CurrentUser.SessionKey);

        _logger.LogInformation("User logged out.");
        return RedirectToPage("./Login");
    }
}
