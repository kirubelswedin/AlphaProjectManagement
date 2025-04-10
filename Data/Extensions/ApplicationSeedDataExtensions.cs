using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Data.Configuration;
using Data.Contexts;
using Data.Entities;

namespace Data.Extensions;

public static class ApplicationSeedDataExtensions
{
  public static IApplicationBuilder UseSeedData(this IApplicationBuilder app)
  {
    using var scope = app.ApplicationServices.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Seed NotificationTypes
    if (!context.NotificationTypes.Any())
    {
      context.NotificationTypes.AddRange(SeedDataConfiguration.GetNotificationTypes());
      context.SaveChanges();
    }

    // Seed NotificationTargets
    if (!context.NotificationTargets.Any())
    {
      context.NotificationTargets.AddRange(SeedDataConfiguration.GetNotificationTargets());
      context.SaveChanges();
    }

    // Seed Project Statuses
    if (!context.Statuses.Any())
    {
      context.Statuses.AddRange(SeedDataConfiguration.GetProjectStatuses());
      context.SaveChanges();
    }

    // Seed Project Roles
    if (!context.ProjectRoles.Any())
    {
      context.ProjectRoles.AddRange(SeedDataConfiguration.GetProjectRoles());
      context.SaveChanges();
    }

    return app;
  }
}