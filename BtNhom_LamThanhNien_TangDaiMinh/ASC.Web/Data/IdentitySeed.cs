using ASC.Model;
using ASC.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.Web.Data;

public class IdentitySeed : IIdentitySeed
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly ApplicationSettings _applicationSettings;

    public IdentitySeed(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IOptions<ApplicationSettings> options)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _applicationSettings = options.Value;
    }

    public async Task SeedAsync()
    {
        var roles = new[]
        {
            Constants.AdminRole,
            Constants.ServiceEngineerRole,
            Constants.CustomerRole
        };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await EnsureUserAsync(_applicationSettings.AdminEmail, _applicationSettings.AdminPassword, Constants.AdminRole);
        await EnsureUserAsync(_applicationSettings.EngineerEmail, _applicationSettings.EngineerPassword, Constants.ServiceEngineerRole);
        await EnsureUserAsync(_applicationSettings.UserEmail, _applicationSettings.UserPassword, Constants.CustomerRole);

        await SeedMasterDataAsync();
    }

    private async Task EnsureUserAsync(string email, string password, string role)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return;
            }
        }
        else
        {
            var shouldUpdateUser = false;
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                shouldUpdateUser = true;
            }

            if (user.LockoutEnd.HasValue)
            {
                user.LockoutEnd = null;
                shouldUpdateUser = true;
            }

            if (!string.Equals(user.UserName, email, StringComparison.OrdinalIgnoreCase))
            {
                user.UserName = email;
                shouldUpdateUser = true;
            }

            if (shouldUpdateUser)
            {
                await _userManager.UpdateAsync(user);
            }

            var isCurrentPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!isCurrentPassword)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, password);
                if (!resetPasswordResult.Succeeded)
                {
                    await _userManager.RemovePasswordAsync(user);
                    await _userManager.AddPasswordAsync(user, password);
                }
            }
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }
    }

    private async Task SeedMasterDataAsync()
    {
        var serviceStatusKey = await _context.MasterDataKeys.FirstOrDefaultAsync(k => k.Key == "ServiceStatus");
        if (serviceStatusKey is null)
        {
            serviceStatusKey = new MasterDataKey
            {
                Id = Guid.NewGuid().ToString(),
                Key = "ServiceStatus",
                Description = "Master data for Service Status",
                CreatedBy = "System",
<<<<<<< HEAD
                CreatedDate = DateTime.UtcNow
=======
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "System",
                UpdatedDate = DateTime.UtcNow
>>>>>>> dd2e859 (Nop bai Lab09 LamThanhNien TangDaiMinh)
            };
            _context.MasterDataKeys.Add(serviceStatusKey);
            await _context.SaveChangesAsync();
        }

        var defaults = new[] { "New", "In Progress", "Completed" };
        for (var i = 0; i < defaults.Length; i++)
        {
            var status = defaults[i];
            var exists = await _context.MasterDataValues.AnyAsync(v =>
                v.MasterDataKeyId == serviceStatusKey.Id &&
                v.Value == status);

            if (!exists)
            {
                _context.MasterDataValues.Add(new MasterDataValue
                {
                    Id = Guid.NewGuid().ToString(),
                    Value = status,
                    Description = status,
                    DisplayOrder = i + 1,
                    IsActive = true,
                    MasterDataKeyId = serviceStatusKey.Id,
                    CreatedBy = "System",
<<<<<<< HEAD
                    CreatedDate = DateTime.UtcNow
=======
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = "System",
                    UpdatedDate = DateTime.UtcNow
>>>>>>> dd2e859 (Nop bai Lab09 LamThanhNien TangDaiMinh)
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}
