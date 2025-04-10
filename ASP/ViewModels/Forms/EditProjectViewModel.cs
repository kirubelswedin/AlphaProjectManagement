using System.ComponentModel.DataAnnotations;
using ASP.ViewModels.Components;
using ASP.ViewModels.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASP.ViewModels.forms;

public class EditProjectViewModel : BaseProjectViewModel
{
    [Required]
    public string Id { get; set; } = null!;

    public string? ImageUrl { get; set; }

    [Required]
    [Display(Name = "Status", Prompt = "Select status")]
    public string StatusName { get; set; } = null!;

    public List<SelectListItem>? Statuses { get; set; } = [];
}
