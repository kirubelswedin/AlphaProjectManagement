using System.ComponentModel.DataAnnotations;
using ASP.ViewModels.Components;
using ASP.ViewModels.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ASP.ViewModels.forms;

public class AddProjectViewModel : BaseProjectViewModel
{
    [DataType(DataType.Upload)]
    [Display(Name = "Image")]
    public IFormFile? ImageFile { get; set; }
    
    public List<SelectListItem> Clients { get; set; } = [];
    public List<SelectorItemViewModel> Members { get; set; } = [];
}