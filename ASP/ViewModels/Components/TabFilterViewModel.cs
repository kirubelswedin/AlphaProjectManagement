namespace ASP.ViewModels.Components;

public class TabFilterViewModel
{
  public List<TabViewModel> Tabs { get; set; } = [];

  public class TabViewModel
  {
    public string Text { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool IsActive { get; set; }
  }

  public static TabFilterViewModel CreateFromProjects(List<ProjectCardViewModel> projects, string activeTab = "ALL")
  {
    var tabs = new List<TabViewModel>
    {
      new() { Text = "ALL", Count = projects.Count, IsActive = activeTab == "ALL" }
    };

    var statusGroups = projects.GroupBy(p => p.Status.StatusName);
    foreach (var group in statusGroups)
    {
      tabs.Add(new TabViewModel
      {
        Text = group.Key,
        Count = group.Count(),
        IsActive = activeTab == group.Key
      });
    }

    return new TabFilterViewModel { Tabs = tabs };
  }
}

