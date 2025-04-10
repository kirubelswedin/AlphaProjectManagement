namespace Domain.Models;

public class NotificationType
{
  public int Id { get; set; }
  public string TypeName { get; set; } = null!;
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}