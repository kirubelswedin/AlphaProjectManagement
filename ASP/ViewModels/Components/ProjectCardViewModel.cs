using Domain.Models;

namespace ASP.ViewModels.Components;

public class ProjectCardViewModel
{
  public string Id { get; set; } = null!;
  public string ImageUrl { get; set; } = null!;
  public string ProjectName { get; set; } = null!;
  public string ClientName { get; set; } = null!;
  public string? Description { get; set; }
  public string? StartDate { get; set; }
  public string? EndDate { get; set; }
  public string? TimeLeft { get; set; }
  public bool IsUrgent { get; set; }
  public bool IsOverdue { get; set; }
  public bool CompletedOnTime { get; set; }

  public decimal? Budget { get; set; }
  public Status Status { get; set; } = null!;

  public List<ProjectMemberViewModel> AllMembers { get; set; } = [];
  public ProjectDropdownViewModel Dropdown => new() { Id = Id };
}

public class ProjectMemberViewModel
{
  public string Id { get; set; } = null!;
  public string? ImageUrl { get; set; }
  public string? FullName { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
}

public class ProjectDropdownViewModel
{
  public string? Id { get; set; }
  public string? Controller { get; set; } = "Projects";
  public string? EditAction { get; set; } = "Edit";
  public string? DeleteAction { get; set; } = "Delete";
}