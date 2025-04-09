

using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ClientEntity
{
    [Key] 
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string? ImageUrl { get; set; }
    
    [Required]
    public string ClientName { get; set; } = null!;

    [Required]
    public string ContactPerson { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Phone]
    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }


    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
}