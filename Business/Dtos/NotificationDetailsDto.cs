namespace Business.Dtos;

public class NotificationDetailsDto
{
    public int NotificationTypeId { get; set; }
    public int NotificationTargetId { get; set; }
    public string Title { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string? UserId { get; set; }
}