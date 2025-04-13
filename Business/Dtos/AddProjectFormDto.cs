using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Dtos;

public class AddProjectFormDto
{
    public IFormFile? ImageFile { get; set; }

    [Required(ErrorMessage = "Project name is required")]
    public string ProjectName { get; set; } = null!;

    [Required(ErrorMessage = "Client is required")]
    public string ClientId { get; set; } = null!;

    [DataType(DataType.Text)]
    public string? Description { get; set; } = null!;

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }
    
    [Required(ErrorMessage = "At least one Team member are required")]
    public string UserId { get; set; } = null!;

    [DataType(DataType.Currency)]
    public decimal? Budget { get; set; }
}