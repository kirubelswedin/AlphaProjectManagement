using System.ComponentModel.DataAnnotations;
using ASP.ViewModels.Components;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASP.ViewModels.Forms;

public class EditProjectViewModel
{
    [Required]
    public string Id { get; set; } = null!;

    [DataType(DataType.Upload)]
    [Display(Name = "Image")]
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
    
    [Required(ErrorMessage = "Project name is required")]
    [Display(Name = "Project Name", Prompt = "Enter project name")]
    public string ProjectName { get; set; } = null!;

    [Required(ErrorMessage = "Client is required")]
    [Display(Name = "Client", Prompt = "Select client")]
    public string ClientId { get; set; } = null!;
    public IEnumerable<SelectListItem> Clients { get; set; } = [];

    [DataType(DataType.Text)]
    [Display(Name = "Description", Prompt = "Type something")]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Start Date", Prompt = "Select start date")]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "End Date", Prompt = "Select end date")]
    public DateTime? EndDate { get; set; }
    
    [Display(Name = "Members", Prompt = "Select members")]
    public IEnumerable<SelectListItem> AllMembers { get; set; } = [];
    public List<ProjectMemberViewModel> ProjectMembers { get; set; } = [];
    
    public string[] SelectedMemberIds { get; set; } = [];

    [DataType(DataType.Currency)]
    [Display(Name = "Budget", Prompt = "Enter budget")]
    public decimal? Budget { get; set; }

    [Required]
    [Display(Name = "Status", Prompt = "Select status")]
    public int StatusId { get; set; }
    public IEnumerable<SelectListItem>? Statuses { get; set; } = [];
}
