using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASP.ViewModels.forms;

public class EditProjectViewModel
{
    
    public List<SelectListItem> Clients { get; set; } = [];
    public List<SelectListItem> Members { get; set; } = [];
    public List<SelectListItem>? Statuses { get; set; } = [];
    
    [Required]
    public string Id { get; set; } = null!;

    [DataType(DataType.Upload)]
    [Display(Name = "Image")]
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
    
    [Required]
    [Display(Name = "Project Name", Prompt = "Enter project name")]
    public string ProjectName { get; set; } = null!;

    [Required]
    [Display(Name = "Client", Prompt = "Select client")]
    public string ClientId { get; set; } = null!;

    [DataType(DataType.Text)]
    [Display(Name = "Description", Prompt = "Type something")]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Start Date", Prompt = "Select start date")]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "End Date", Prompt = "Select end date")]
    public DateTime? EndDate { get; set; }
    
    [Required]
    [Display(Name = "Member", Prompt = "Select member")]
    public string UserId { get; set; } = null!;

    [DataType(DataType.Currency)]
    [Display(Name = "Budget", Prompt = "Enter budget")]
    public decimal? Budget { get; set; }

    [Required]
    [Display(Name = "Status", Prompt = "Select status")]
    public string StatusName { get; set; } = null!;
    
}
