using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Dtos;

public class UpdateProjectDto
{
    public string Id { get; set; }

    [Required(ErrorMessage = "Project name is required")]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required(ErrorMessage = "Client name is required")]
    [MaxLength(100)]
    public string ClientName { get; set; }

    public string Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public decimal Budget { get; set; }

    public bool IsActive { get; set; }

    public List<string> MemberIds { get; set; } = new List<string>();

    [Required(ErrorMessage = "Status is required")]
    public string StatusId { get; set; } = string.Empty;

    [DataType(DataType.Upload)]
    public IFormFile? ProjectImage { get; set; }
}