using ASC.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ASC.Web.Areas.Identity.Pages.Account;

public class ExternalLoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly ILogger<ExternalLoginModel> _logger;

    public ExternalLoginModel(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IUserStore<IdentityUser> userStore,
        ILogger<ExternalLoginModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string ProviderDisplayName { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public IActionResult OnGet() => RedirectToPage("./Login");

    public IActionResult OnPost(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/ServiceRequests/Dashboard/Dashboard");

        if (remoteError is not null)
        {
            TempData["ExternalLoginError"] = $"Lỗi đăng nhập ngoài: {remoteError}";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            TempData["ExternalLoginError"] = "Không thể tải thông tin đăng nhập Google.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
        if (result.Succeeded)
        {
            var signedInUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (signedInUser is not null)
            {
                await SetCurrentUserSessionAsync(signedInUser);
            }

            return LocalRedirect(returnUrl);
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["ExternalLoginError"] = "Google chưa trả về email hợp lệ.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            var roles = await _userManager.GetRolesAsync(existingUser);
            if (roles.Contains("Admin") || roles.Contains("Engineer") || roles.Contains("ServiceEngineer"))
            {
                TempData["ExternalLoginError"] = "Tài khoản Admin/Engineer không được tạo mới bằng đăng nhập Google.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
            if (!addLoginResult.Succeeded)
            {
                TempData["ExternalLoginError"] = "Không thể liên kết tài khoản Google với người dùng hiện có.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            await _signInManager.SignInAsync(existingUser, false, info.LoginProvider);
            await SetCurrentUserSessionAsync(existingUser);
            return LocalRedirect(returnUrl);
        }

        var user = CreateUser();
        await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
        var emailStore = GetEmailStore();
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            TempData["ExternalLoginError"] = "Không thể tạo tài khoản người dùng từ Google.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        await _userManager.AddToRoleAsync(user, "User");
        await _userManager.AddLoginAsync(user, info);

        await _signInManager.SignInAsync(user, false, info.LoginProvider);
        await SetCurrentUserSessionAsync(user);

        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
        return LocalRedirect(returnUrl);
    }

    public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/ServiceRequests/Dashboard/Dashboard");

        if (!ModelState.IsValid)
        {
            ReturnUrl = returnUrl;
            return Page();
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            TempData["ExternalLoginError"] = "Không thể tải thông tin đăng nhập ngoài trong lúc xác nhận.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        var user = CreateUser();
        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        var emailStore = GetEmailStore();
        await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");

            result = await _userManager.AddLoginAsync(user, info);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false, info.LoginProvider);
                await SetCurrentUserSessionAsync(user);
                return LocalRedirect(returnUrl);
            }
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        ProviderDisplayName = info.ProviderDisplayName ?? info.LoginProvider;
        ReturnUrl = returnUrl;
        return Page();
    }

    private IdentityUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<IdentityUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'.");
        }
    }

    private IUserEmailStore<IdentityUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }

        return (IUserEmailStore<IdentityUser>)_userStore;
    }

    private async Task SetCurrentUserSessionAsync(IdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        HttpContext.Session.SetSession(CurrentUser.SessionKey, new CurrentUser
        {
            UserId = user.Id,
            UserName = user.UserName ?? user.Email ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList()
        });
    }
}
