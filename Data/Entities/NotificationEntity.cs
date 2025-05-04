using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Entities;

public class NotificationEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [ForeignKey(nameof(NotificationType))]
    public int NotificationTypeId { get; set; }
    public virtual NotificationTypeEntity NotificationType { get; set; } = null!;

    [ForeignKey(nameof(NotificationTarget))]
    public int NotificationTargetId { get; set; }
    public virtual NotificationTargetEntity NotificationTarget { get; set; } = null!;

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string? ImageUrl { get; set; } = null!;
    public string ImageType { get; set; } = null!;

    [Required]
    public string Message { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? RecipientUserId { get; set; } // for individual recipients, null for groups

    public virtual ICollection<NotificationDismissedEntity> DismissedNotifications { get; set; } = [];

}
