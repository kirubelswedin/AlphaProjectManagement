namespace ASP.ViewModels.Components;

public class NotificationDropdownViewModel
{
    public int TotalCount { get; set; } 
    public int UnreadCount { get; set; }
    public List<NotificationItemViewModel> Notifications { get; set; } = [];
}

public class NotificationItemViewModel
{
    public string Id { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
} 