using ASP.ViewModels.Components;
using ASP.ViewModels.forms;
using Domain.Models;

namespace ASP.ViewModels.Views;

public class ProjectsViewModel
{

    public PageHeaderViewModel PageHeader { get; set; } = new()
    {
        Title = "Projects",
        ButtonText = "Add Project",
        ModalId = "addprojectmodal"
    };

    public TabFilterViewModel TabFilter { get; set; } = new();
    public List<ProjectCardViewModel> Projects { get; set; } = [];

    // Formulärdata
    public AddProjectViewModel AddProject { get; set; } = new();
    public EditProjectViewModel EditProject { get; set; } = new();
}

