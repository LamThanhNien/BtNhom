using ASC.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASC.Web.Data
{
    public class IdentitySeed : IIdentitySeed
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public IdentitySeed(UserManager<IdentityUser> userManager,
                            RoleManager<IdentityRole> roleManager,
                            ApplicationDbContext context,
                            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task SeedAsync()
        {
            var roles = new[] { Constants.AdminRole, Constants.ServiceEngineerRole, Constants.CustomerRole, "Engineer", "User" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = _configuration["IdentitySeed:AdminEmail"] ?? "admin@asc.com";
            var adminPassword = _configuration["IdentitySeed:AdminPassword"] ?? "Admin@123";

            if (await _userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, Constants.AdminRole);
                }
            }

            var engineerEmail = "engineer@test.com";
            var engineerPassword = "Engineer@123";

            if (await _userManager.FindByEmailAsync(engineerEmail) == null)
            {
                var engineerUser = new IdentityUser
                {
                    UserName = engineerEmail,
                    Email = engineerEmail,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(engineerUser, engineerPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(engineerUser, "Engineer");
                }
            }

            await SeedMasterDataAsync("MasterData:ServiceStatus", "ServiceStatus");
            await SeedMasterDataAsync("MasterData:PromotionTypes", "PromotionType");
        }

        private async Task SeedMasterDataAsync(string configKey, string masterKeyName)
        {
            var section = _configuration.GetSection(configKey);
            if (!section.Exists()) return;

            var keyEntity = await _context.MasterDataKeys.FirstOrDefaultAsync(k => k.Key == masterKeyName);
            if (keyEntity == null)
            {
                keyEntity = new MasterDataKey
                {
                    Id = Guid.NewGuid().ToString(),
                    Key = masterKeyName,
                    Description = $"Master data for {masterKeyName}",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow
                };
                await _context.MasterDataKeys.AddAsync(keyEntity);
                await _context.SaveChangesAsync();
            }

            var values = section.GetChildren().Select(c => new MasterDataValue
            {
                Id = Guid.NewGuid().ToString(),
                Value = c["Value"],
                Description = c["Description"] ?? c["Value"],
                DisplayOrder = int.Parse(c["DisplayOrder"] ?? "0"),
                IsActive = bool.Parse(c["IsActive"] ?? "true"),
                MasterDataKeyId = keyEntity.Id,
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            }).ToList();

            foreach (var val in values)
            {
                if (!await _context.MasterDataValues.AnyAsync(v => v.Value == val.Value && v.MasterDataKeyId == keyEntity.Id))
                {
                    await _context.MasterDataValues.AddAsync(val);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
