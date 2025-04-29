namespace Business.Dtos;

public class NotificationDetailsDto
{
    public string Id { get; set; } = null!;
    public int NotificationTypeId { get; set; }
    public int NotificationTargetId { get; set; }
    public string Title { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string ImageType { get; set; } = null!; // ex: "users", "clients", "projects"
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public string? UserId { get; set; }
}