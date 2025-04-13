using Microsoft.AspNetCore.Html;

namespace ASP.ViewModels.Components;

public class DropdownWindowViewModel
{
    public string? HeaderTitle { get; set; }
    public int? NotificationCount { get; set; }
    public string EmptyMessage { get; set; } = "No items";
    public List<DropdownItemViewModel> Items { get; set; } = [];
}

public class DropdownItemViewModel
{
    public string? Icon { get; set; }
    public string Text { get; set; } = null!;
    public string? OnClick { get; set; }
    public string? Content { get; set; } = null!;
    public IHtmlContent? ContentHtml { get; set; }
    public string HtmlAttributes { get; set; }
    public bool HasDivider { get; set; }
    public bool IsDanger { get; set; }
    public string? AdditionalContent { get; set; }
}