

using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ClientEntity
{
    [Key] 
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [MaxLength(200)]
    public string? Image { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ClientName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string ContactPerson { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = null!;

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }


    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
}