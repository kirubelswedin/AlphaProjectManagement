using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? ImageUrl { get; set; }

    [Required]
    [ForeignKey(nameof(Client))]
    public string ClientId { get; set; } = null!;
    public virtual ClientEntity Client { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string ProjectName { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    [Column(TypeName = "date")]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    [Column(TypeName = "date")]
    public DateTime EndDate { get; set; }

    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Budget { get; set; }

    [Required]
    [ForeignKey(nameof(Status))]
    public int StatusId { get; set; }
    public virtual StatusEntity Status { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public virtual UserEntity User { get; set; } = null!;

    // Completion tracking
    public bool IsCompleted { get; set; }
    public bool CompletedOnTime { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Status calculations
    public bool IsOverdue => !IsCompleted && DateTime.UtcNow > EndDate;
    public bool IsUrgent => !IsCompleted && !IsOverdue && (EndDate - DateTime.UtcNow).TotalDays <= 7;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ProjectMemberEntity> ProjectMembers { get; set; } = new List<ProjectMemberEntity>();
}