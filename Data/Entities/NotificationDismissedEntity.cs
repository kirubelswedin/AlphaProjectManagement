using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class NotificationDismissedEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public virtual UserEntity User { get; set; } = null!;

    [ForeignKey(nameof(Notification))]
    public string NotificationId { get; set; } = null!;
    public virtual NotificationEntity Notification { get; set; } = null!;

}