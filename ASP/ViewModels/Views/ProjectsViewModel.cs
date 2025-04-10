using ASP.ViewModels.Components;
using ASP.ViewModels.forms;
using Business.Dtos;
using Domain.Models;

namespace ASP.ViewModels.Views;

public class ProjectsViewModel
{

    public PageHeaderViewModel PageHeader { get; set; } = new();

    public TabFilterViewModel TabFilter { get; set; } = new();
    public List<Project> Projects { get; set; } = [];

    // Formulärdata
    public AddProjectViewModel AddProject { get; set; } = new();
    public EditProjectViewModel EditProject { get; set; } = new();
}
