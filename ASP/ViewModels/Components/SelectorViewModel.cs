namespace ASP.ViewModels.Components;

public class SelectorViewModel
{
  public string Id { get; set; } = null!;
  public string Label { get; set; } = null!;
  public string Placeholder { get; set; } = "Search...";
  public string Icon { get; set; } = "fa-duotone fa-solid fa-search";
  public bool IsMultiple { get; set; } = false;
  public bool IsRequired { get; set; } = false;
  public string HiddenInputName { get; set; } = null!;
  public string ValidationFor { get; set; } = null!;
  public List<SelectorItemViewModel> Items { get; set; } = [];
  
  // For single select
  public string? SelectedId { get; set; }
  
  // For multiple select
  public List<string>? SelectedIds { get; set; }
}

public class SelectorItemViewModel
{
  public string Id { get; set; } = null!;
  public string Text { get; set; } = null!;
  public string? ImageUrl { get; set; }
  public bool IsSelected { get; set; } = false;
}