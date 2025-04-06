using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class NotificationTypeEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string NotificationType { get; set; } = null!;
    
    public virtual ICollection<NotificationEntity> Notifications { get; set; } = [];
}