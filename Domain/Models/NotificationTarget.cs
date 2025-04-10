namespace Domain.Models;

public class NotificationTarget
{
  public int Id { get; set; }
  public string TargetName { get; set; } = null!;
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}