using ASC.DataAccess;
using ASC.Web.Data;
using ASC.Web.Models;
using ASC.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASC.Web.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddMyDependencyGroup(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();
        services.AddRazorPages();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var googleClientId = configuration["Authentication:Google:ClientId"];
        var googleClientSecret = configuration["Authentication:Google:ClientSecret"];
        if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
        {
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = googleClientId;
                    options.ClientSecret = googleClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });
        }

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/Login";
        });

        services.AddDistributedMemoryCache();
        services.AddMemoryCache();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.Configure<ApplicationSettings>(configuration.GetSection("ApplicationSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        services.AddScoped<IUnitOfWork>(sp =>
            new UnitOfWork(sp.GetRequiredService<ApplicationDbContext>()));

        services.AddScoped<IIdentitySeed, IdentitySeed>();
        services.AddScoped<IEmailSender, AuthMessageSender>();
        services.AddScoped<INavigationCacheOperations, NavigationCacheOperations>();

        return services;
    }
}
