using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class UserAddressEntity
{
    [Key, ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    
    [MaxLength(200)]
    public string? StreetAddress { get; set; }
    
    [MaxLength(50)]
    public string? PostalCode { get; set; }
    
    [MaxLength(100)]
    public string? City { get; set; }
    
    [MaxLength(100)]
    public string? Country { get; set; }

    public virtual UserEntity User { get; set; } = null!;
}