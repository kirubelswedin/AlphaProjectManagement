namespace ASP.ViewModels.Components;

public class ProjectCardViewModel
{
  public string? Id { get; set; }
  public string ProjectName { get; set; } = null!;
  public string ClientName { get; set; } = null!;
  public string Description { get; set; } = null!;
  public string StartDate { get; set; } = null!;
  public string EndDate { get; set; } = null!;
  public string TimeLeft { get; set; } = "Unknown";
  public bool IsUrgent { get; set; }
  public bool IsOverdue { get; set; }
  public bool CompletedOnTime { get; set; }

  // manually set completed on time
  public bool CompletedOnTimeSet { get; set; }

  public string ProjectImage { get; set; } = "/images/project/Image-1.svg";
  public decimal? Budget { get; set; }
  public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
  public List<MemberViewModel> TeamMembers { get; set; } = new();
}

public enum ProjectStatus
{
  NotStarted,
  InProgress,
  Paused,
  Completed,
  Cancelled
}

public class MemberViewModel
{
  public string Id { get; set; } = null!;
  public string? Avatar { get; set; }
}