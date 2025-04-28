using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Data.Configuration;
using Data.Contexts;


namespace Data.Extensions;

public static class ApplicationSeedDataExtensions
{
  public static IApplicationBuilder UseSeedData(this IApplicationBuilder app)
  {
    using var scope = app.ApplicationServices.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    if (!context.NotificationTypes.Any())
    {
      context.NotificationTypes.AddRange(SeedDataConfiguration.GetNotificationTypes());
      context.SaveChanges();
    }
    
    if (!context.NotificationTargets.Any())
    {
      context.NotificationTargets.AddRange(SeedDataConfiguration.GetNotificationTargets());
      context.SaveChanges();
    }
    
    if (!context.Statuses.Any())
    {
      context.Statuses.AddRange(SeedDataConfiguration.GetProjectStatuses());
      context.SaveChanges();
    }
    
    if (!context.ProjectRoles.Any())
    {
      context.ProjectRoles.AddRange(SeedDataConfiguration.GetProjectRoles());
      context.SaveChanges();
    }

    return app;
  }
}