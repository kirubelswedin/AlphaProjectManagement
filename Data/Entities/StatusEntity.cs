using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class StatusEntity
{
    [Key]
    public int Id { get; set; }
    public string StatusName { get; set; } = null!;
    public int SortOrder { get; set; }
    public string Color { get; set; } = null!;
    public bool IsDefault { get; set; }

    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}