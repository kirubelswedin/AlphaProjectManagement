using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class StatusEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string StatusName { get; set; } = null!;

    [MaxLength(200)]
    public string? Description { get; set; }

    public int SortOrder { get; set; }

    [Required]
    [MaxLength(50)]
    public string Color { get; set; } = null!;

    
    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
}