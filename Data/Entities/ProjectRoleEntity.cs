using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ProjectRoleEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    public int SortOrder { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }


    public virtual ICollection<ProjectMemberEntity> ProjectMembers { get; set; } = [];
}