using Domain.Models;

namespace ASP.ViewModels.Components;

public class ProjectCardViewModel
{
  public string Id { get; set; } = null!;
  public string ProjectName { get; set; } = null!;
  public string ClientName { get; set; } = null!;
  public string? Description { get; set; }
  public string? StartDate { get; set; }
  public string? EndDate { get; set; }
  public string? TimeLeft { get; set; }
  public bool IsUrgent { get; set; }
  public bool IsOverdue { get; set; }
  public bool CompletedOnTime { get; set; }

  // manually set completed on time
  public bool CompletedOnTimeSet { get; set; }

  public string ProjectImage { get; set; } = null!;
  public decimal? Budget { get; set; }
  public Status Status { get; set; } = null!;
  public List<MemberViewModel> Members { get; set; } = [];
}

public class MemberViewModel
{
  public string Id { get; set; } = null!;
  public string? Avatar { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
}