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
            // Tạo roles
            var roles = new[] { Constants.AdminRole, Constants.ServiceEngineerRole, Constants.CustomerRole };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Tạo admin user mặc định
            var adminEmail = "admin@asc.com";
            if (await _userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, Constants.AdminRole);
                }
            }

            // Seed MasterData từ appsettings.json
            await SeedMasterDataAsync("MasterData:ServiceStatus", "ServiceStatus");
            await SeedMasterDataAsync("MasterData:PromotionTypes", "PromotionType");
        }

        private async Task SeedMasterDataAsync(string configKey, string masterKeyName)
        {
            var section = _configuration.GetSection(configKey);
            if (!section.Exists()) return;

            // Tìm hoặc tạo MasterDataKey
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

            // Lấy danh sách giá trị từ config
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

            // Thêm nếu chưa tồn tại
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