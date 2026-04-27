using ASC.Model;
using ASC.Web.Configuration;
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
        var serviceStatusKey = await EnsureMasterDataKeyAsync("ServiceStatus", "Master data for Service Status");
        var vehicleTypeKey = await EnsureMasterDataKeyAsync("VehicleType", "Master data for Vehicle Type");
        var vehicleNameKey = await EnsureMasterDataKeyAsync("VehicleName", "Master data for Vehicle Name");

        await EnsureMasterDataValuesAsync(serviceStatusKey, new[]
        {
            ASC.Model.Status.New.ToString(),
            ASC.Model.Status.Initiated.ToString(),
            ASC.Model.Status.InProgress.ToString(),
            ASC.Model.Status.PendingCustomerApproval.ToString(),
            ASC.Model.Status.RequestForInformation.ToString(),
            ASC.Model.Status.Pending.ToString(),
            ASC.Model.Status.Denied.ToString(),
            ASC.Model.Status.Completed.ToString()
        });

        await EnsureMasterDataValuesAsync(vehicleTypeKey, new[]
        {
            "Sedan",
            "SUV",
            "Truck"
        });

        await EnsureMasterDataValuesAsync(vehicleNameKey, new[]
        {
            "Honda Civic",
            "Toyota Camry",
            "Ford Ranger"
        });

        await _context.SaveChangesAsync();
    }

    private async Task<MasterDataKey> EnsureMasterDataKeyAsync(string key, string description)
    {
        var masterDataKey = await _context.MasterDataKeys
            .FirstOrDefaultAsync(k => k.Key == key);
        if (masterDataKey != null)
        {
            return masterDataKey;
        }

        masterDataKey = new MasterDataKey
        {
            Id = Guid.NewGuid().ToString(),
            Key = key,
            Description = description,
            IsActive = true,
            CreatedBy = "System",
            CreatedDate = DateTime.UtcNow,
            UpdatedBy = "System",
            UpdatedDate = DateTime.UtcNow
        };

        _context.MasterDataKeys.Add(masterDataKey);
        await _context.SaveChangesAsync();
        return masterDataKey;
    }

    private async Task EnsureMasterDataValuesAsync(MasterDataKey key, IReadOnlyList<string> defaults)
    {
        var existingValues = await _context.MasterDataValues
            .Where(v => v.MasterDataKeyId == key.Id)
            .Select(v => v.Value)
            .ToListAsync();

        var existingSet = existingValues
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v.Replace(" ", string.Empty).ToUpper())
            .ToHashSet();

        for (var i = 0; i < defaults.Count; i++)
        {
            var value = defaults[i];
            var normalizedValue = value.Replace(" ", string.Empty).ToUpper();
            if (existingSet.Contains(normalizedValue))
            {
                continue;
            }

            _context.MasterDataValues.Add(new MasterDataValue
            {
                Id = Guid.NewGuid().ToString(),
                Value = value,
                Description = value,
                DisplayOrder = i + 1,
                IsActive = true,
                MasterDataKeyId = key.Id,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = "System",
                UpdatedDate = DateTime.UtcNow
            });

            existingSet.Add(normalizedValue);
        }
    }
}
