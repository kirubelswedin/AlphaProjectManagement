namespace ASP.ViewModels.Components;

public class TabFilterViewModel
{
  public List<TabViewModel> Tabs { get; set; } = new();
  
  public class TabViewModel
  {
    public string Text { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool IsActive { get; set; }
  }
  
  public static TabFilterViewModel CreateFromProjects(List<ProjectCardViewModel> projects, string activeTab = "ALL")
  {
    // Count projects in each category
    int allCount = projects.Count;
    int notStartedCount = projects.Count(p => p.Status == ProjectStatus.NotStarted);
    int inProgressCount = projects.Count(p => p.Status == ProjectStatus.InProgress);
    int pausedCount = projects.Count(p => p.Status == ProjectStatus.Paused);
    int completedCount = projects.Count(p => p.Status == ProjectStatus.Completed);
    int cancelledCount = projects.Count(p => p.Status == ProjectStatus.Cancelled);

    return new TabFilterViewModel
    {
      Tabs =
      [
        new TabViewModel { Text = "ALL", Count = allCount, IsActive = activeTab == "ALL" },
        new() { Text = "NOT STARTED", Count = notStartedCount, IsActive = activeTab == "NOT STARTED" },
        new() { Text = "IN PROGRESS", Count = inProgressCount, IsActive = activeTab == "IN PROGRESS" },
        new() { Text = "PAUSED", Count = pausedCount, IsActive = activeTab == "PAUSED" },
        new() { Text = "COMPLETED", Count = completedCount, IsActive = activeTab == "COMPLETED" },
        new() { Text = "CANCELLED", Count = cancelledCount, IsActive = activeTab == "CANCELLED" }
      ]
    };
  }
}

