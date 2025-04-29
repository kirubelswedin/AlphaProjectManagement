using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Dtos;

public class AddProjectFormDto
{
    [DataType(DataType.Upload)]
    public IFormFile? ImageFile { get; set; }

    [Required(ErrorMessage = "Project name is required")]
    public string ProjectName { get; set; } = null!;

    [Required(ErrorMessage = "Client is required")]
    public string ClientId { get; set; } = null!;

    [DataType(DataType.Text)]
    public string? Description { get; set; } = null!;

    [Required(ErrorMessage = "Start date is required")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "At least one project member is required")]
    public List<string> SelectedMemberIds { get; set; } = [];
    
    [DataType(DataType.Currency)]
    public decimal? Budget { get; set; }
}