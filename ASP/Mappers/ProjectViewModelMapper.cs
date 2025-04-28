using ASP.Controllers;
using ASP.Extensions;
using ASP.ViewModels.Components;
using ASP.ViewModels.Forms;
using Business.Dtos;
using Domain.Models;

namespace ASP.Mappers;

public static class ProjectViewModelMapper
{
    public static AddProjectFormDto ToAddProjectFormDto(AddProjectViewModel model)
    {
        return new AddProjectFormDto
        {
            ImageFile = model.ImageFile,
            ProjectName = model.ProjectName,
            ClientId = model.ClientId,
            Description = model.Description,
            StartDate = model.StartDate ?? DateTime.MinValue,
            EndDate = model.EndDate ?? DateTime.MinValue,
            Budget = model.Budget,
            SelectedMemberIds = model.SelectedMemberIds.ToList()
        };
    }

    public static UpdateProjectFormDto ToUpdateProjectFormDto(EditProjectViewModel model)
    {
        return new UpdateProjectFormDto
        {
            Id = model.Id,
            ImageFile = model.ImageFile,
            ImageUrl = model.ImageUrl,
            ProjectName = model.ProjectName,
            ClientId = model.ClientId,
            Description = model.Description,
            StartDate = model.StartDate ?? DateTime.MinValue,
            EndDate = model.EndDate ?? DateTime.MinValue,
            Budget = model.Budget,
            StatusId = model.StatusId,
            SelectedMemberIds = model.SelectedMemberIds.ToList()

        };
    }
    
    public static ProjectCardViewModel ToProjectCardViewModel(ProjectDetailsDto project)
    {
        return new ProjectCardViewModel
        {
            Id = project.Id,
            ProjectName = project.ProjectName,
            ClientName = project.Client?.ClientName ?? "Okänd kund",
            Description = project.Description,
            StartDate = project.StartDate?.ToString("yyyy-MM-dd"),
            EndDate = project.EndDate?.ToString("yyyy-MM-dd"),
            TimeLeft = project.EndDate.HasValue ? ProjectsController.DateTimeHelpers.GetTimeLeft(project.EndDate.Value) : null,
            IsUrgent = project.IsUrgent,
            IsOverdue = project.IsOverdue,
            CompletedOnTime = project.CompletedOnTime,
            ImageUrl = (project.ImageUrl ?? "default-project.svg").GetImageUrl("projects"),
            Budget = project.Budget,
            Status = project.Status, // Om Status kan vara null, hantera det i vyn
            AllMembers = project.Members?.Select(m => new ProjectMemberViewModel
            {
                Id = m.User?.Id ?? "",
                FirstName = m.User?.FirstName ?? "",
                LastName = m.User?.LastName ?? "",
                ImageUrl = (m.User?.ImageUrl ?? "default-user.svg").GetImageUrl("members"),
            }).ToList() ?? []
        };
    }
}