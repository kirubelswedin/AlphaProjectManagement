using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class NotificationTargetEntity
{
    [Key]
    public int Id { get; set; }
    public string TargetName { get; set; } = null!;

    public virtual ICollection<NotificationEntity> Notifications { get; set; } = [];

}