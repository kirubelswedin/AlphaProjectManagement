using Data.Entities;

namespace Data.Configuration;

public static class SeedDataConfiguration
{
  #region Notification Configuration
  public static IEnumerable<NotificationTypeEntity> GetNotificationTypes()
  {
    return new[]
    {
            new NotificationTypeEntity { TypeName = "User" },
            new NotificationTypeEntity { TypeName = "Project" },
            new NotificationTypeEntity { TypeName = "Client" },
            new NotificationTypeEntity { TypeName = "Member" }
        };
  }

  public static IEnumerable<NotificationTargetEntity> GetNotificationTargets()
  {
    return new[]
    {
            new NotificationTargetEntity { TargetName = "AllUsers" },
            new NotificationTargetEntity { TargetName = "Admins" }
        };
  }
  #endregion

  #region Status Configuration
  private static StatusEntity CreateProjectStatus(string name, int sortOrder, string color, bool isDefault = false)
  {
    return new StatusEntity
    {
      StatusName = name,
      SortOrder = sortOrder,
      Color = color,
      IsDefault = isDefault,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };
  }

  public static IEnumerable<StatusEntity> GetProjectStatuses()
  {
    return new[]
    {
            CreateProjectStatus("Not Started", 1, "#637085ff", true),
            CreateProjectStatus("In Progress", 2, "#2d99ffff"),
            CreateProjectStatus("Paused", 3, "#ff9640ff"),
            CreateProjectStatus("Completed", 4, "#30d482ff"),
            CreateProjectStatus("Cancelled", 5, "#e94e3fff")
        };
  }
  #endregion

  #region Project Role Configuration
  private static ProjectRoleEntity CreateProjectRole(string id, string name, string description, bool isDefault = false)
  {
    return new ProjectRoleEntity
    {
      Id = id,
      Name = name,
      Description = description,
      IsDefault = isDefault,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };
  }

  public static IEnumerable<ProjectRoleEntity> GetProjectRoles()
  {
    return new[]
    {
            CreateProjectRole("1", "Member", "Regular team member", true),
            CreateProjectRole("2", "Project Manager", "Manages the project"),
            CreateProjectRole("3", "Admin", "Administrate Project, Members and Clients")
        };
  }
  #endregion
}