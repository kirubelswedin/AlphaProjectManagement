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

    // Private helper methods
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

    private async Task<List<SelectListItem>?> GetStatusSelectListAsync()
    {
        var statuses = await _statusService.GetStatusesAsync();
        return statuses.Result?.Select(s => new SelectListItem
        {
            Text = s.StatusName,
            Value = s.StatusName
        }).ToList();
    }

    private async Task<List<SelectorItemViewModel>> GetMemberSelectListAsync()
    {
        var result = await _userService.GetUsersAsync();
        return result.Succeeded
            ? result.Result!.Select(u => new SelectorItemViewModel
            {
                Id = u.Id,
                Text = $"{u.FirstName} {u.LastName}",
                ImageUrl = u.ImageUrl
            }).ToList()
            : [];
    }

    private async Task<List<Project>> GetProjectsAsync(string tab)
    {
        var result = await _projectService.GetProjectsAsync();
        if (!result.Succeeded) return [];

        var projects = result.Result!.ToList();
        return tab.Equals("ALL"
, StringComparison.CurrentCultureIgnoreCase)
            ? projects
            : projects.Where(p => p.Status.StatusName.Replace(" ", "") == tab).ToList();
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
}