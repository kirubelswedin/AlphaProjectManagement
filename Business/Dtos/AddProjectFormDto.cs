using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Dtos;

public class AddProjectFormDto
{
    public IFormFile? ImageFile { get; set; }

    [Required]
    public string ProjectName { get; set; } = null!;

    [Required]
    public string ClientId { get; set; } = null!;

    public string? Description { get; set; } = null!;

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    public decimal? Budget { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public int StatusId { get; set; }

}