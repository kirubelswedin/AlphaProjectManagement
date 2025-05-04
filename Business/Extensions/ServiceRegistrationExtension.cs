using Business.Services;
using Business.Handlers;
using Data.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace Business.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IStatusService, StatusService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped(typeof(ICacheHandler<>), typeof(CacheHandler<>));
        services.AddScoped<AppDbContext, AppDbContext>();
        
        
        // If UseAzure is true, use AzureImageHandler, otherwise use LocalImageHandler
        services.AddScoped<IImageHandler>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var useAzure = config.GetValue<bool>("AzureStorageAccount:UseAzure");
            if (useAzure)
                return new AzureImageHandler(config);
            return new LocalImageHandler(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images"));
        });

        return services;
    }
}