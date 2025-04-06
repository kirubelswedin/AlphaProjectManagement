using ASP.ViewModels.Components;
using ASP.ViewModels.forms;

namespace ASP.ViewModels.Views;

public class ProjectsViewModel
{

    public PageHeaderViewModel PageHeader { get; set; } = new()
    {
        Title = "Projects",
        ButtonText = "Add Project",
        ModalId = "addProjectModal"
    };
    
    public ProjectFormViewModel ProjectForm { get; set; } = new();

    public TabFilterViewModel TabFilter { get; set; } = new();
    public List<ProjectCardViewModel> Projects { get; set; } = [];
}
