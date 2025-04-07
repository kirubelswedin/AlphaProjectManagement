namespace ASP.ViewModels.Components;

public class ProfileDropdownViewModel
{
    public string UserId { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
    public bool IsDarkMode { get; set; }
    public string LogoutUrl { get; set; } = null!;
}
