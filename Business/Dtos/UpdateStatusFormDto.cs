using System.ComponentModel.DataAnnotations;

namespace Business.Dtos;

public class UpdateStatusFormDto
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    public string StatusName { get; set; } = null!;
}