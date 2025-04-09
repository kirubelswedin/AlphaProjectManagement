namespace Domain.Models;

public class Status
{
    public string Id { get; set; } = null!;
    public string StatusName { get; set; } = null!;
    public int SortOrder { get; set; }
    public string Color { get; set; } = null!;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}