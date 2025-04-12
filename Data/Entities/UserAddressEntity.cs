using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserAddressEntity
{
    [Key, ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public virtual UserEntity User { get; set; } = null!;

    [MaxLength(200)]
    [ProtectedPersonalData]
    public string? StreetAddress { get; set; }

    [MaxLength(50)]
    [ProtectedPersonalData]
    public string? PostalCode { get; set; }

    [MaxLength(100)]
    [ProtectedPersonalData]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }
}