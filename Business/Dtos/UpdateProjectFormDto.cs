using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Dtos;

public class UpdateProjectFormDto
{
    [Required]
    public string Id { get; set; } = null!;

    [DataType(DataType.Upload)]
    public IFormFile? NewImageFile { get; set; }
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Project name is required")]
    public string ProjectName { get; set; } = null!;

    [Required(ErrorMessage = "Client name is required")]
    public string ClientId { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    public decimal? Budget { get; set; }


    [Required(ErrorMessage = "Status is required")]
    public int StatusId { get; set; }

}