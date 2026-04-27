using ASC.Business;
using ASC.Business.Interfaces;
using ASC.DataAccess;
using ASC.Web.Data;
using ASC.Web.Services;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using System.Net.Sockets;

namespace ASC.Web.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddAscFeatureServices(this IServiceCollection services, IConfiguration configuration)
    {
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

        var redisConnection = configuration.GetSection("CacheSettings:CacheConnectionString").Value;
        var redisInstanceName = configuration.GetSection("CacheSettings:CacheInstance").Value;
        var useRedis = configuration.GetValue<bool?>("CacheSettings:UseRedis") ?? true;

        if (useRedis && CanConnectToRedis(redisConnection, timeoutMilliseconds: 200))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                var redisOptions = ConfigurationOptions.Parse(redisConnection ?? "127.0.0.1:6379");
                redisOptions.AbortOnConnectFail = false;
                redisOptions.ConnectRetry = 1;
                redisOptions.ConnectTimeout = 300;
                redisOptions.AsyncTimeout = 300;
                redisOptions.SyncTimeout = 300;

                options.ConfigurationOptions = redisOptions;
                options.InstanceName = redisInstanceName;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddMemoryCache();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.IOTimeout = TimeSpan.FromMilliseconds(500);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddScoped<IUnitOfWork>(sp =>
            new UnitOfWork(sp.GetRequiredService<ApplicationDbContext>()));

        services.AddScoped<IIdentitySeed, IdentitySeed>();
        services.AddScoped<INavigationCacheOperations, NavigationCacheOperations>();
        services.AddScoped<IMasterDataOperations, MasterDataOperations>();
        services.AddScoped<IMasterDataCacheOperations, MasterDataCacheOperations>();
        services.AddScoped<IServiceRequestOperations, ServiceRequestOperations>();
        services.AddAutoMapper(typeof(ApplicationDbContext));

        return services;
    }

    private static bool CanConnectToRedis(string? connectionString, int timeoutMilliseconds)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        var endpoint = connectionString
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return false;
        }

        var host = endpoint;
        var port = 6379;

        var separatorIndex = endpoint.LastIndexOf(':');
        if (separatorIndex > 0 && separatorIndex < endpoint.Length - 1)
        {
            host = endpoint[..separatorIndex];
            if (!int.TryParse(endpoint[(separatorIndex + 1)..], out port))
            {
                port = 6379;
            }
        }

        try
        {
            using var tcpClient = new TcpClient();
            var connectTask = tcpClient.ConnectAsync(host, port);
            var completed = connectTask.Wait(timeoutMilliseconds);
            return completed && tcpClient.Connected;
        }
        catch
        {
            return false;
        }
    }
}
