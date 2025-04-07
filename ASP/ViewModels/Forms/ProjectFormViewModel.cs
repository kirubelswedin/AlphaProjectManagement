using System.ComponentModel.DataAnnotations;

namespace ASP.ViewModels.forms;

public class ProjectFormViewModel
{
    public string? Id { get; set; }
    
    [DataType(DataType.Upload)]
    [Display(Name = "Project Image", Prompt = "Select project image")]
    public IFormFile? Image { get; set; }
    public string? ImageUrl { get; set; }

    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Project Name", Prompt = "Enter project name")]
    public string ProjectName { get; set; } = null!;

    [DataType(DataType.Text)]
    [Display(Name = "Description", Prompt = "Type something")]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Start Date", Prompt = "Enter start date")]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "End Date", Prompt = "Enter end date")]
    public DateTime? EndDate { get; set; }

    [DataType(DataType.Currency)]
    [Display(Name = "Budget", Prompt = "Enter budget")]
    public decimal? Budget { get; set; }

    [Required]
    [Display(Name = "Client", Prompt = "Select client")] 
    public string ClientId { get; set; } = null!;

    [Required]
    [Display(Name = "Status", Prompt = "Select status")]
    public string StatusId { get; set; } = null!;

    [Display(Name = "Member", Prompt = "Add team members")]
    public List<string> MemberIds { get; set; } = [];
}