using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectMemberEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey(nameof(Project))]
    public string ProjectId { get; set; } = null!;
    public virtual ProjectEntity Project { get; set; } = null!;
    
    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public virtual UserEntity User { get; set; } = null!;
    
    [Required]
    [ForeignKey(nameof(Role))]
    public string RoleId { get; set; } = null!;
    public virtual ProjectRoleEntity Role { get; set; } = null!;
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}