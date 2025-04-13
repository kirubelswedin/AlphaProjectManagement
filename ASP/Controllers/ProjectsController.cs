using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ASP.ViewModels.Components;
using ASP.ViewModels.forms;
using Microsoft.AspNetCore.Authorization;
using Business.Services;
using ASP.ViewModels.Views;
using Business.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using Domain.Models;

namespace ASP.Controllers;

[Authorize]
public class ProjectsController(
    IProjectService projectService,
    IClientService clientService,
    IUserService userService,
    IStatusService statusService,
    INotificationService notificationService) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly IClientService _clientService = clientService;
    private readonly IUserService _userService = userService;
    private readonly IStatusService _statusService = statusService;
    private readonly INotificationService _notificationService = notificationService;

    [Route("admin/projects")]
    public async Task<IActionResult> Index(string tab = "ALL")
    {
        var viewModel = new ProjectsViewModel
        {
            PageHeader = CreatePageHeader(),
            TabFilter = await CreateTabFilterAsync(tab),
            Projects = await GetProjectsAsync(tab),
            AddProject = new AddProjectViewModel
            {
                Clients = await GetClientSelectListAsync(),
                Members = await GetMemberSelectListAsync()
            },
            EditProject = new EditProjectViewModel
            {
                Clients = await GetClientSelectListAsync(),
                Members = await GetMemberSelectListAsync(),
                Statuses = await GetStatusSelectListAsync()
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddProject([FromForm] AddProjectFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, errors = GetModelErrors() });
        }

        var result = await _projectService.CreateProjectAsync(dto, User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
        if (!result.Succeeded)
        {
            return Json(new { success = false, error = result.Error });
        }

        await CreateProjectNotification("New Project Created", $"Project '{dto.ProjectName}' has been created");
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProject([FromForm] UpdateProjectFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, errors = GetModelErrors() });
        }

        var result = await _projectService.UpdateProjectAsync(dto);
        if (!result.Succeeded)
        {
            return Json(new { success = false, error = result.Error });
        }

        await CreateProjectNotification("Project Updated", $"Project '{dto.ProjectName}' has been updated");
        return Json(new { success = true });
    }

    [HttpGet("projects/{id}")]
    public async Task<IActionResult> GetProject(string id)
    {
        var result = await _projectService.GetProjectDetailsAsync(id);
        if (!result.Succeeded)
        {
            return Json(new { success = false, error = result.Error });
        }
        return Json(new { success = true, project = result.Result });
    }

    [HttpGet("projects/list")]
    public async Task<IActionResult> GetFilteredProjects(string tab = "ALL")
    {
        var projects = await GetProjectsAsync(tab);
        return Json(new { success = true, projects });
    }

    [HttpDelete("projects/{id}")]
    public async Task<IActionResult> DeleteProject(string id)
    {
        var result = await _projectService.DeleteProjectAsync(id);
        if (!result.Succeeded)
        {
            return Json(new { success = false, error = result.Error });
        }

        await CreateProjectNotification("Project Deleted", "A project has been deleted");
        return Json(new { success = true });
    }

    private async Task<TabFilterViewModel> CreateTabFilterAsync(string activeTab)
    {
        var projectsResult = await _projectService.GetProjectsAsync();
        var statusesResult = await _statusService.GetStatusesAsync();

        if (!statusesResult.Succeeded)
            return new TabFilterViewModel();

        var projects = projectsResult.Succeeded ? projectsResult.Result!.ToList() : [];
        var statuses = statusesResult.Result!.OrderBy(s => s.SortOrder);

        var tabs = new List<TabFilterViewModel.TabViewModel>
        {
            new() { Text = "ALL", Count = projects.Count, IsActive = activeTab == "ALL" }
        };

        // create a dictionary to count projects by status
        var projectCountByStatus = projects
            .GroupBy(p => p.Status?.StatusName ?? "Unknown")
            .ToDictionary(g => g.Key, g => g.Count());

        // Add all statuses, even if they have no projects
        foreach (var status in statuses)
        {
            var count = projectCountByStatus.GetValueOrDefault(status.StatusName, 0);

            tabs.Add(new TabFilterViewModel.TabViewModel
            {
                Text = status.StatusName,
                Count = count,
                IsActive = status.StatusName.Replace(" ", "") == activeTab
            });
        }

        return new TabFilterViewModel { Tabs = tabs };
    }

    private async Task<List<ProjectCardViewModel>> GetProjectsAsync(string tab)
    {
        var result = await _projectService.GetProjectsAsync();
        if (!result.Succeeded) return [];

        var projects = result.Result!.ToList();
        var filteredProjects = tab.Equals("ALL", StringComparison.CurrentCultureIgnoreCase)
            ? projects
            : projects.Where(p => p.Status.StatusName.Replace(" ", "") == tab).ToList();

        return filteredProjects.Select(p => new ProjectCardViewModel
        {
            Id = p.Id,
            ProjectName = p.ProjectName,
            ClientName = p.Client.ClientName,
            Description = p.Description,
            StartDate = p.StartDate?.ToString("yyyy-MM-dd"),
            EndDate = p.EndDate?.ToString("yyyy-MM-dd"),
            TimeLeft = p.EndDate.HasValue ? GetTimeLeft(p.EndDate.Value) : null,
            IsUrgent = p.IsUrgent,
            IsOverdue = p.IsOverdue,
            CompletedOnTime = p.CompletedOnTime,
            ProjectImage = p.ImageUrl ?? "/images/project/default-project.svg",
            Budget = p.Budget,
            Status = p.Status,
            Members = p.ProjectMembers?.Select(pm => new MemberViewModel
            {
                Id = pm.User.Id,
                Avatar = pm.User.ImageUrl ?? "/images/avatars/default-avatar.svg",
                FirstName = pm.User.FirstName,
                LastName = pm.User.LastName
            }).ToList() ?? []
        }).ToList();
    }

    private static string? GetTimeLeft(DateTime endDate)
    {
        var timeLeft = endDate - DateTime.UtcNow;
        if (timeLeft.TotalDays < 0)
            return "Overdue";
        if (timeLeft.TotalDays < 1)
            return "Due today";
        if (timeLeft.TotalDays < 7)
            return $"{timeLeft.Days} days left";
        return $"{timeLeft.Days} days left";
    }

    private static PageHeaderViewModel CreatePageHeader() => new()
    {
        Title = "Projects",
        ButtonText = "Add Project",
        ModalId = "addprojectmodal"
    };

    private async Task<List<SelectListItem>> GetClientSelectListAsync()
    {
        var result = await _clientService.GetClientsAsync();
        return result.Succeeded
            ? result.Result!.Select(c => new SelectListItem
            {
                Value = c.Id,
                Text = c.ClientName
            }).ToList()
            : [];
    }

    private async Task<List<SelectListItem>> GetMemberSelectListAsync()
    {
        var result = await _userService.GetUsersAsync();
        return result.Succeeded && result.Result != null
            ? result.Result.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.FirstName} {u.LastName}"
            }).ToList()
            : [];
    }

    private async Task<List<SelectListItem>?> GetStatusSelectListAsync()
    {
        var statuses = await _statusService.GetStatusesAsync();
        return statuses.Result?.Select(s => new SelectListItem
        {
            Text = s.StatusName,
            Value = s.StatusName
        }).ToList();
    }


    private async Task CreateProjectNotification(string title, string message)
    {
        var notification = new NotificationDetailsDto
        {
            NotificationTypeId = 2, // Project type
            NotificationTargetId = 2, // Admins
            Title = title,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationService.AddNotificationAsync(notification);
    }

    private Dictionary<string, string[]> GetModelErrors()
    {
        return ModelState
            .Where(x => x.Value!.Errors.Any())
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );
    }

    [HttpGet("projects/members/search")]
    public async Task<IActionResult> SearchMembers(string term)
    {
        var result = await _userService.GetUsersAsync();
        if (!result.Succeeded || result.Result == null)
        {
            return Json(new List<object>());
        }

        var filteredMembers = result.Result
            .Where(u =>
                (u.FirstName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u.LastName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u.Email?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))
            .Select(u => new
            {
                id = u.Id,
                fullName = $"{u.FirstName} {u.LastName}",
                imageUrl = u.ImageUrl ?? "/images/avatars/default-avatar.svg"
            })
            .ToList();

        return Json(filteredMembers);
    }
}