using System.Security.Claims;
using ASP.Extensions;
using Microsoft.AspNetCore.Mvc;
using ASP.ViewModels.Components;
using ASP.ViewModels.Forms;
using Microsoft.AspNetCore.Authorization;
using Business.Services;
using ASP.ViewModels.Views;
using Business.Dtos;
using ASP.Mappers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASP.Controllers;

[Authorize]
public class ProjectsController(
    IProjectService projectService,
    IClientService clientService,
    IUserService userService,
    IStatusService statusService,
    INotificationService notificationService
    ) : Controller
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
                ProjectMembers = await GetMemberSelectListAsync()
            },
            EditProject = new EditProjectViewModel
            {
                Clients = await GetClientSelectListAsync(),
                ProjectMembers = await GetMemberSelectListAsync(),
                Statuses = await GetStatusSelectListAsync()
            }
        };

        return View(viewModel);
    }
    
    private static PageHeaderViewModel CreatePageHeader() => new()
    {
        Title = "Projects",
        ButtonText = "Add Project",
        ModalId = "addprojectmodal"
    };

    [HttpPost]
    public async Task<IActionResult> AddProject([FromForm] AddProjectViewModel model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetModelErrors() });

        var dto = ProjectViewModelMapper.ToAddProjectFormDto(model);

        try
        {
            var result = await _projectService.CreateProjectAsync(dto, User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
            if (!result.Succeeded)
                return Json(new { success = false, error = result.Error ?? "Failed to create project" });

            var projectViewModel = ProjectViewModelMapper.ToProjectCardViewModel(result.Result!);
            await CreateProjectNotification(projectViewModel.ImageUrl, "Project Created", $"Project '{dto.ProjectName}' has been created");
            return Json(new { success = true, project = projectViewModel });
        }
        catch (Exception ex)
        { return Json(new { success = false, error = ex.Message }); }
    }

    [HttpGet("projects/{id}")]
    public async Task<IActionResult> GetProject(string id)
    {
        var result = await _projectService.GetProjectDetailsAsync(id);
        if (!result.Succeeded || result.Result == null)
            return Json(new { success = false, error = result.Error });
        
        var projectViewModel = ProjectViewModelMapper.ToProjectCardViewModel(result.Result);
        
        var clients = await GetClientSelectListAsync();
        var members = await GetMemberSelectListAsync();
        var statuses = await GetStatusSelectListAsync();
        
        if (statuses == null) return Json(new { success = false, error = "Failed to get statuses" });
            Console.WriteLine($" Clients count: {clients.Count}, Members count: {members.Count}, Statuses count: {statuses.Count}");
        
        return Json(new { success = true, project = projectViewModel, clients, members, statuses });
    }
    

    private async Task<List<ProjectCardViewModel>> GetProjectsAsync(string tab)
    {
        var result = await _projectService.GetProjectsAsync();
        if (!result.Succeeded) return [];

        var projects = result.Result!.ToList();
        var filteredProjects = tab.Equals("ALL", StringComparison.CurrentCultureIgnoreCase)
            ? projects
            : projects.Where(p => p.Status?.StatusName == tab).ToList();

        // using mappers to map projects to view models
        var projectCards = filteredProjects
            .Select(ProjectViewModelMapper.ToProjectCardViewModel)  
            .ToList();

        return projectCards;
    }
    
    [HttpPost]
    public async Task<IActionResult> EditProject([FromForm] EditProjectViewModel model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetModelErrors() });

        var dto = ProjectViewModelMapper.ToUpdateProjectFormDto(model);
        try
        {
            var result = await _projectService.UpdateProjectAsync(dto);
            if (!result.Succeeded)
                return Json(new { success = false, error = result.Error ?? "Failed to update project" });
            
            var projectViewModel = ProjectViewModelMapper.ToProjectCardViewModel(result.Result!);
            
            var clients = await GetClientSelectListAsync();
            var members = await GetMemberSelectListAsync();
            var statuses = await GetStatusSelectListAsync();
            
            if (statuses == null) return Json(new { success = false, error = "Failed to get statuses" });
                Console.WriteLine($" Clients count: {clients.Count}, Members count: {members.Count}, Statuses count: {statuses.Count}");
            
            await CreateProjectNotification(projectViewModel.ImageUrl,"Project Updated",$"Project {dto.ProjectName} has been updated");
            return Json(new { success = true, project = projectViewModel, clients, members, statuses  }); 
        }
        catch (Exception ex)
        { return Json(new { success = false, error = ex.Message}); }
    }

    [HttpDelete("projects/{id}")]
    public async Task<IActionResult> DeleteProject(string id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (!project.Succeeded)
            return Json(new { success = false, error = project.Error });

        var result = await _projectService.DeleteProjectAsync(id);
        if (!result.Succeeded)
            return Json(new { success = false, error = result.Error });
        
        var projectViewModel = ProjectViewModelMapper.ToProjectCardViewModel(result.Result!);
        await CreateProjectNotification(projectViewModel.ImageUrl, "Project Deleted", $"Project {project.Result!.ProjectName} has been deleted");
        return Json(new { success = true, project = projectViewModel }); 
    }
    
    [HttpPost]
    public async Task<IActionResult> RemoveMember(string projectId, string userId)
    {
        var result = await _projectService.RemoveProjectMemberAsync(projectId, userId);
        if (result.Succeeded)
            return Ok();
        return BadRequest(result.Error);
    }

    [HttpGet("projects/list")]
    public async Task<IActionResult> GetFilteredProjects(string tab = "ALL")
    {
        var projects = await GetProjectsAsync(tab);

        return PartialView("Partials/Components/TabFilter/_FilteredProjects", projects);
        // Return HTML instead of JSON for filtered projects
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
                IsActive = activeTab == status.StatusName
            });
        }

        return new TabFilterViewModel { Tabs = tabs };
    }

    public static class DateTimeHelpers
    {
        public static string GetTimeLeft(DateTime endDate)
        {
            var timeLeft = endDate - DateTime.UtcNow;
            switch (timeLeft.TotalDays)
            {
                case < 0:
                    return "Overdue";
                case < 1:
                    return "Due today";
                default:
                    return $"{timeLeft.Days} days left";
            }
        }
    }
    
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

    private async Task<List<ProjectMemberViewModel>> GetMemberSelectListAsync()
    {
        var result = await _userService.GetUsersAsync();
        return result.Succeeded && result.Result != null
            ? result.Result.Select(u => new ProjectMemberViewModel
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName}",
                ImageUrl = (u.ImageUrl ?? "default-user.svg").GetImageUrl("users")
            }).ToList()
            : [];
    }

    private async Task<List<SelectListItem>?> GetStatusSelectListAsync()
    {
        var statuses = await _statusService.GetStatusesAsync();
        return statuses.Result?.Select(s => new SelectListItem
        {
            Text = s.StatusName,
            Value = s.Id.ToString()
        }).ToList();
    }
    
    [HttpGet("projects/members/search")]
    public async Task<IActionResult> SearchUsers(string term)
    {
        var result = await _userService.GetUsersAsync();
        if (!result.Succeeded || result.Result == null)
        {
            Console.WriteLine("SearchUsers: Failed to get users");
            return Json(new List<object>());
        }

        var filteredUsers = result.Result
            .Where(u =>
                (u.FirstName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u.LastName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u.Email?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))
            .Select(u => new
            {
                id = u.Id,
                ImageUrl = (u.ImageUrl ?? "default-user.svg").GetImageUrl("users"),
                fullName = $"{u.FirstName} {u.LastName}",
            })
            .ToList();

        Console.WriteLine($"SearchUsers: Found {filteredUsers.Count} users for term '{term}'");
        return Json(filteredUsers);
    }


    private async Task CreateProjectNotification(string imageUrl, string title, string message)
    {
        var notificationDetails = new NotificationDetailsDto
        {
            NotificationTypeId = 2, // Project
            NotificationTargetId = 1, // Admin
            Title = title,
            Message = message,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _notificationService.AddNotificationAsync(notificationDetails);
        if (!result.Succeeded)
        {
            // Console.WriteLine($"Error creating notification: {result.Error}");
        }
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
}