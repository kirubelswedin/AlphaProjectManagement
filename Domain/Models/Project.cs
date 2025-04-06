namespace Domain.Models;

public class Project
{
    public string Id { get; set; } = null!;
    public string? Image { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public string ClientId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public int StatusId { get; set; }
    public Client Client { get; set; } = null!;
    public Status Status { get; set; } = null!;

    // Completion tracking
    public bool IsCompleted { get; set; }
    public bool CompletedOnTime { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Status calculations
    public bool IsOverdue => !IsCompleted && DateTime.UtcNow > EndDate;
    public bool IsUrgent => !IsCompleted && !IsOverdue && EndDate.HasValue && (EndDate.Value - DateTime.UtcNow).TotalDays <= 7;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}