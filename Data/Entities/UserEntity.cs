using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserEntity : IdentityUser
{
    [ProtectedPersonalData]
    public string? Image { get; set; }

    [ProtectedPersonalData]
    [Required]
    public string FirstName { get; set; } = null!;

    [ProtectedPersonalData]
    [Required]
    public string LastName { get; set; } = null!;

    [ProtectedPersonalData]
    public string? JobTitle { get; set; }

    [ProtectedPersonalData]
    public DateTime? DateOfBirth { get; set; }


    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
    public virtual UserAddressEntity? Address { get; set; }
    public virtual ICollection<NotificationDismissedEntity> DismissedNotifications { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}