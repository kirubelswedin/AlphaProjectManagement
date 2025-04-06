using ASP.ViewModels.Components;

namespace ASP.ViewModels.Views;

public class DashboardViewModel
{
  public PageHeaderViewModel PageHeader { get; set; } = new()
  {
    Title = "Dashboard"
  };
}