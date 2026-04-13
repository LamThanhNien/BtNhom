using ASC.Utilities;
using ASC.Web.Areas.Accounts.Models;
using ASC.Web.Controllers;
using ASC.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Areas.Accounts.Controllers;

[Area("Accounts")]
[Authorize]
public class AccountController : BaseController
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IEmailSender emailSender,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> ServiceEngineers()
    {
        return View(new ServiceEngineerViewModel
        {
            ServiceEngineers = await GetServiceEngineersAsync(),
            Registration = new ServiceEngineerRegistrationViewModel()
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ServiceEngineers(ServiceEngineerViewModel viewModel)
    {
        if (viewModel.Registration is null)
        {
            viewModel = new ServiceEngineerViewModel();
        }

        if (string.IsNullOrWhiteSpace(viewModel.Registration.Id) && string.IsNullOrWhiteSpace(viewModel.Registration.Password))
        {
            ModelState.AddModelError("Registration.Password", "Password is required for new service engineer.");
        }

        if (!ModelState.IsValid)
        {
            viewModel.ServiceEngineers = await GetServiceEngineersAsync();
            return View(viewModel);
        }

        if (string.IsNullOrWhiteSpace(viewModel.Registration.Id))
        {
            var existingByEmail = await _userManager.FindByEmailAsync(viewModel.Registration.Email);
            if (existingByEmail is not null)
            {
                ModelState.AddModelError("Registration.Email", "Email already exists.");
                viewModel.ServiceEngineers = await GetServiceEngineersAsync();
                return View(viewModel);
            }

            var newUser = new IdentityUser
            {
                UserName = viewModel.Registration.UserName,
                Email = viewModel.Registration.Email,
                EmailConfirmed = true,
                LockoutEnabled = true
            };

            var createResult = await _userManager.CreateAsync(newUser, viewModel.Registration.Password!);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                viewModel.ServiceEngineers = await GetServiceEngineersAsync();
                return View(viewModel);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(newUser, "Engineer");
            if (!addRoleResult.Succeeded)
            {
                foreach (var error in addRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                viewModel.ServiceEngineers = await GetServiceEngineersAsync();
                return View(viewModel);
            }

            await SetUserActivationAsync(newUser, viewModel.Registration.IsActive);
            TempData["SuccessMessage"] = "Service engineer account created successfully.";
        }
        else
        {
            var user = await _userManager.FindByIdAsync(viewModel.Registration.Id);
            if (user is null)
            {
                TempData["ErrorMessage"] = "Service engineer account not found.";
                return RedirectToAction(nameof(ServiceEngineers));
            }

            user.UserName = viewModel.Registration.UserName;
            user.Email = viewModel.Registration.Email;
            user.LockoutEnabled = true;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                viewModel.ServiceEngineers = await GetServiceEngineersAsync();
                return View(viewModel);
            }

            await SetUserActivationAsync(user, viewModel.Registration.IsActive);
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Account update notification",
                    "Your service engineer account information was updated by administrator.");
            }

            TempData["SuccessMessage"] = "Service engineer account updated successfully.";
        }

        return RedirectToAction(nameof(ServiceEngineers));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Customers()
    {
        return View(new CustomerViewModel
        {
            Customers = await GetCustomersAsync(),
            Registration = new CustomerRegistrationViewModel()
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Customers(CustomerViewModel viewModel)
    {
        if (string.IsNullOrWhiteSpace(viewModel.Registration.Id))
        {
            TempData["ErrorMessage"] = "Invalid customer account.";
            return RedirectToAction(nameof(Customers));
        }

        var customer = await _userManager.FindByIdAsync(viewModel.Registration.Id);
        if (customer is null)
        {
            TempData["ErrorMessage"] = "Customer account not found.";
            return RedirectToAction(nameof(Customers));
        }

        await SetUserActivationAsync(customer, viewModel.Registration.IsActive);

        if (!string.IsNullOrWhiteSpace(customer.Email))
        {
            await _emailSender.SendEmailAsync(
                customer.Email,
                "Account status update",
                $"Your customer account is now {(viewModel.Registration.IsActive ? "active" : "inactive")}.");
        }

        TempData["SuccessMessage"] = "Customer account status updated successfully.";
        return RedirectToAction(nameof(Customers));
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        return View(new ProfileModel
        {
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        if (!ModelState.IsValid)
        {
            model.Email = user.Email ?? string.Empty;
            return View(model);
        }

        var userWithSameName = await _userManager.FindByNameAsync(model.UserName);
        if (userWithSameName is not null && userWithSameName.Id != user.Id)
        {
            ModelState.AddModelError(nameof(model.UserName), "Username already exists.");
            model.Email = user.Email ?? string.Empty;
            return View(model);
        }

        user.UserName = model.UserName;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.Email = user.Email ?? string.Empty;
            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);
        await SetCurrentUserSessionAsync(user);

        TempData["SuccessMessage"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    private async Task<List<ServiceEngineerRegistrationViewModel>> GetServiceEngineersAsync()
    {
        var engineerUsers = await _userManager.GetUsersInRoleAsync("Engineer");
        var serviceEngineerUsers = await _userManager.GetUsersInRoleAsync("ServiceEngineer");

        return engineerUsers
            .Concat(serviceEngineerUsers)
            .GroupBy(x => x.Id)
            .Select(group => group.First())
            .OrderBy(x => x.Email)
            .Select(user => new ServiceEngineerRegistrationViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                IsActive = IsUserActive(user)
            })
            .ToList();
    }

    private async Task<List<CustomerRegistrationViewModel>> GetCustomersAsync()
    {
        var customerUsers = await _userManager.GetUsersInRoleAsync("User");

        return customerUsers
            .OrderBy(x => x.Email)
            .Select(user => new CustomerRegistrationViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                IsActive = IsUserActive(user)
            })
            .ToList();
    }

    private async Task SetCurrentUserSessionAsync(IdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        HttpContext.Session.SetObject(CurrentUser.SessionKey, new CurrentUser
        {
            UserId = user.Id,
            UserName = user.UserName ?? user.Email ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList()
        });
    }

    private async Task SetUserActivationAsync(IdentityUser user, bool isActive)
    {
        user.LockoutEnabled = true;
        user.LockoutEnd = isActive ? null : DateTimeOffset.MaxValue;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            _logger.LogWarning("Failed to update activation state for user {UserId}", user.Id);
        }
    }

    private static bool IsUserActive(IdentityUser user)
    {
        return !user.LockoutEnabled || user.LockoutEnd is null || user.LockoutEnd <= DateTimeOffset.UtcNow;
    }
}
