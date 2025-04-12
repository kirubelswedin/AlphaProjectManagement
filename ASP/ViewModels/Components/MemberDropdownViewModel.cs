namespace ASP.ViewModels.Components;

public class MemberDropdownViewModel
{
    public string Id { get; set; } = null!;
    public bool IsAdmin { get; set; }
}

public class MemberItemViewModel
{
    public string Id { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public string Text { get; set; } = null!;
    public bool IsChecked { get; set; }
}

