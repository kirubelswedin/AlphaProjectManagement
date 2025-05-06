using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserEntity>(options)
{
    public virtual DbSet<StatusEntity> Statuses { get; set; }
    public virtual DbSet<ClientEntity> Clients { get; set; }
    public virtual DbSet<ProjectEntity> Projects { get; set; }
    public virtual DbSet<NotificationEntity> Notifications { get; set; }
    public virtual DbSet<NotificationTypeEntity> NotificationTypes { get; set; }
    public virtual DbSet<NotificationTargetEntity> NotificationTargets { get; set; }
    public virtual DbSet<NotificationDismissedEntity> DismissedNotifications { get; set; }

}