namespace ASP.ViewModels.Components;

public class DropdownWindowViewModel
{
    public string? HeaderTitle { get; set; }
    public int? NotificationCount { get; set; }
    public List<DropdownWindowItemViewModel> Items { get; set; } = new();
}

public class DropdownWindowItemViewModel
{
    public string? Icon { get; set; }
    public string Text { get; set; } = null!;
    public string? OnClick { get; set; }
    public bool IsDanger { get; set; }
    public bool HasDivider { get; set; }
    public bool HasSwitch { get; set; }
    public string? AdditionalContent { get; set; }
}