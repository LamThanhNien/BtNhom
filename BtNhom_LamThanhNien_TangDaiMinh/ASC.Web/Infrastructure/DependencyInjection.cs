using ASC.DataAccess;
using ASC.Web.Data;
using ASC.Web.Models;
using ASC.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace ASC.Web.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMyDependencyGroup(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();
        services.AddRazorPages();
        services.AddMemoryCache();
        services.AddSession();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/Login";
        });

        services.Configure<ApplicationSettings>(configuration.GetSection("ApplicationSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        services.AddScoped<IUnitOfWork>(sp =>
            new UnitOfWork(sp.GetRequiredService<ApplicationDbContext>()));

        services.AddScoped<IIdentitySeed, IdentitySeed>();
        services.AddScoped<INavigationCacheOperations, NavigationCacheOperations>();
        services.AddScoped<IEmailSender, AuthMessageSender>();

        return services;
    }
}
