using System.ComponentModel.DataAnnotations;

namespace Business.Dtos;

public class AddStatusFormDto
{
    [Required]
    public string StatusName { get; set; } = null!;
}