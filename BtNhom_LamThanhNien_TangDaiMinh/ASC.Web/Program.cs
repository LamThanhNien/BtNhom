using ASC.Business.Interfaces;
using ASC.Web.Configuration;
using ASC.Web.Data;
using ASC.Web.Models;
using ASC.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddOptions();
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
builder.Services.PostConfigure<ApplicationSettings>(options =>
{
    builder.Configuration.GetSection("AppSettings").Bind(options);
});
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddTransient<IEmailSender, AuthMessageSender>();
builder.Services.AddTransient<ISmsSender, AuthMessageSender>();

builder.Services.AddAscFeatureServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        var identitySeed = services.GetRequiredService<IIdentitySeed>();
        await identitySeed.SeedAsync();

        var masterDataCache = services.GetRequiredService<IMasterDataCacheOperations>();
        _ = Task.Run(async () =>
        {
            try
            {
                using var cacheScope = app.Services.CreateScope();
                var cacheOps = cacheScope.ServiceProvider.GetRequiredService<IMasterDataCacheOperations>();
                await cacheOps.CreateMasterDataCacheAsync();
            }
            catch (Exception cacheEx)
            {
                logger.LogWarning(cacheEx, "Background warm-up for master data cache failed.");
            }
        });

        var navigationCache = services.GetRequiredService<INavigationCacheOperations>();
        await navigationCache.GetNavigationMenuItems();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the application.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Dashboard}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
