using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ProjectRoleEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public bool IsDefault { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }


    public virtual ICollection<ProjectMemberEntity> ProjectMembers { get; set; } = [];
}
