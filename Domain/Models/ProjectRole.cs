namespace Domain.Models;

public class ProjectRole
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }

    public ICollection<ProjectMember>? ProjectMembers { get; set; }
}