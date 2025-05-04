using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class AppDbContext : IdentityDbContext<UserEntity>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public virtual DbSet<StatusEntity> Statuses { get; set; }
    public virtual DbSet<ClientEntity> Clients { get; set; }
    public virtual DbSet<ProjectEntity> Projects { get; set; }
    public virtual DbSet<NotificationEntity> Notifications { get; set; }
    public virtual DbSet<NotificationTypeEntity> NotificationTypes { get; set; }
    public virtual DbSet<NotificationTargetEntity> NotificationTargets { get; set; }
    public virtual DbSet<NotificationDismissedEntity> DismissedNotifications { get; set; }
    public virtual DbSet<ProjectMemberEntity> ProjectMembers { get; set; }
    public virtual DbSet<ProjectRoleEntity> ProjectRoles { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // when a project is deleted, all its project members are also deleted.
        modelBuilder.Entity<ProjectMemberEntity>()
            .HasOne(pm => pm.Project)
            .WithMany(p => p.ProjectMembers)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // When a user is deleted, their project memberships remain in the database. 
        modelBuilder.Entity<ProjectMemberEntity>()
            .HasOne(pm => pm.User)
            .WithMany()
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        // When a role is deleted, all project members with that role will have their role set to null
        modelBuilder.Entity<ProjectMemberEntity>()
            .HasOne(pm => pm.Role)
            .WithMany(r => r.ProjectMembers)
            .HasForeignKey(pm => pm.RoleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}