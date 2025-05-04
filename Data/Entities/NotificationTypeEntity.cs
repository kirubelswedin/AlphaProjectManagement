using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class NotificationTypeEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string TypeName { get; set; } = null!;
    
    public virtual ICollection<NotificationEntity> Notifications { get; set; } = [];
}