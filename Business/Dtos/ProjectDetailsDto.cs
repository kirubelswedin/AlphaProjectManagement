namespace Business.Dtos;

public class ProjectDetailsDto
{
    // Basic Information
    public string Id { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }

    // Status Information
    public int StatusId { get; set; }
    public string StatusName { get; set; } = null!;
    public string StatusColor { get; set; } = null!;

    // Client Information
    public string ClientId { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string ClientEmail { get; set; } = null!;
    public string ClientContactPerson { get; set; } = null!;

    // Creator Information
    public string CreatedById { get; set; } = null!;
    public string CreatedByName { get; set; } = null!;
    public string? CreatedByImageUrl { get; set; }

    // Project Members
    public ICollection<ProjectMemberDto> Members { get; set; } = [];

    // Statistics for dashboard 
    public int TotalMembers { get; set; }
    public int CompletedTasks { get; set; }
    public int TotalTasks { get; set; }
    public double ProgressPercentage => TotalTasks > 0 ? (CompletedTasks / (double)TotalTasks) * 100 : 0;

    // Time Tracking
    public bool IsOverdue => !IsCompleted && EndDate.HasValue && DateTime.UtcNow > EndDate.Value;
    public bool IsUrgent => !IsCompleted && !IsOverdue && EndDate.HasValue && (EndDate.Value - DateTime.UtcNow).TotalDays <= 7;
    public bool IsCompleted { get; set; }
    public bool CompletedOnTime { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class ProjectMemberDto
{
    public string UserId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string RoleId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
}