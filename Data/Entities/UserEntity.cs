using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserEntity : IdentityUser
{
    [ProtectedPersonalData]
    public string? ImageUrl { get; set; }

    [Required]
    [ProtectedPersonalData]
    public string FirstName { get; set; } = null!;

    [Required]
    [ProtectedPersonalData]
    public string LastName { get; set; } = null!;

    [ProtectedPersonalData]
    public string? JobTitle { get; set; }

    [ProtectedPersonalData]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public virtual UserAddressEntity? Address { get; set; }

    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
    public virtual ICollection<NotificationDismissedEntity> DismissedNotifications { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

