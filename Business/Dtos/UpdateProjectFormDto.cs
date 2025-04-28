using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Dtos;

public class UpdateProjectFormDto
{
    [Required]
    public string Id { get; set; } = null!;

    [DataType(DataType.Upload)]
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Project name is required")]
    public string ProjectName { get; set; } = null!;

    [Required(ErrorMessage = "Client is required")]
    public string ClientId { get; set; } = null!;

    [DataType(DataType.Text)]
    public string? Description { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "At least one project member is required")]
    public List<string> SelectedMemberIds { get; set; } = [];

    [DataType(DataType.Currency)]
    public decimal? Budget { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public int StatusId { get; set; }
}