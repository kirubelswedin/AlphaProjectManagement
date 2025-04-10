namespace Domain.Models;

public class ProjectMember
{
    public string Id { get; set; } = null!;
    
    public string ProjectId { get; set; } = null!;
    public Project? Project { get; set; }
    
    public string UserId { get; set; } = null!;
    public User? User { get; set; }
    
    public string RoleId { get; set; } = null!;
    public ProjectRole? Role { get; set; }
    
    public DateTime JoinedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}