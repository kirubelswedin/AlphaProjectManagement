using System.ComponentModel.DataAnnotations;
using ASP.ViewModels.Components;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASP.ViewModels.Forms;

public class BaseProjectViewModel
{
    [DataType(DataType.Upload)]
    [Display(Name = "")]
    public IFormFile? Image { get; set; }

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

    [Display(Name = "Member", Prompt = "Add team members")]
    public List<string> UserId { get; set; } = [];

    [DataType(DataType.Currency)]
    [Display(Name = "Budget", Prompt = "Enter budget")]
    public decimal? Budget { get; set; }

    public List<SelectListItem> Clients { get; set; } = [];
    public List<SelectorItemViewModel> Members { get; set; } = [];
}