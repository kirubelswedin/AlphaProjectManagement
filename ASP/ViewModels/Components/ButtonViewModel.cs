namespace ASP.ViewModels.Components;

public class ButtonViewModel
{
  public string Text { get; set; } = string.Empty;
  public string Variant { get; set; } = "primary";
  public string? Icon { get; set; }
  public string? Type { get; set; }
  public string? OnClick { get; set; }
  public string? AdditionalClasses { get; set; }
  public bool IsSubmit { get; set; }
  public Dictionary<string, string>? DataAttributes { get; set; }
}