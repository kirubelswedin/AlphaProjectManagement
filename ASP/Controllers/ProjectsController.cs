using Microsoft.AspNetCore.Mvc;
using ASP.ViewModels.Components;
using ASP.ViewModels.forms;
using ASP.ViewModels.MockData;
using ASP.ViewModels.Views;
using Business.Dtos;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ASP.Helpers;
using ASP.Constants;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ASP.Controllers;

[Authorize]
public class ProjectsController(
    IProjectService projectService,
    INotificationService notificationService,
    IUserService userService,
    IWebHostEnvironment env,
    AppDbContext context
  ) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IUserService _userService = userService;
    private readonly IWebHostEnvironment _env = env;
    private readonly AppDbContext _context = context;

    [Route("admin/projects")]
    public IActionResult Index(string tab = "ALL")
    {
        var viewModel = new ProjectsViewModel
        {
            PageHeader = new()
            {
                Title = "Projects",
                ButtonText = "Add Project",
                ModalId = "addProjectModal"
            }
        };

        var allProjects = ProjectsMockData.GetProjects();
        viewModel.Projects = FilterProjects(allProjects, tab);
        viewModel.TabFilter = TabFilterViewModel.CreateFromProjects(allProjects, tab);

        ViewBag.Members = MembersMockData.GetMembers();
        ViewBag.Clients = ClientsMockData.GetClients();
        
        
        // var project = await _context.Projects
        //     .Include(x => x.Members)
        //     .ThenInclude(x => x.User)
        //     .FirstOrDefaultAsync(x => x.Id == 1);

        return View(viewModel);
    }

    // [HttpPost]
    // public async Task<IActionResult> Add(ProjectEntity model, string SelectedUserIds)
    // {
    //     if (!ModelState.IsValid)
    //         return View("index", model);
    //     
    //     var existingMembers = await _context.ProjectMembers
    //         .Where(x => x.ProjectId == model.Id)
    //         .ToListAsync();
    //     
    //     _context.ProjectMembers.RemoveRange(existingMembers);
    //
    //     if (!string.IsNullOrEmpty(SelectedUserIds))
    //     {
    //         var userIds = JsonSerializer.Deserialize<List<int>>(SelectedUserIds);
    //         if (userIds != null)
    //         {
    //             foreach (var userId in userIds)
    //             {
    //                 _context.ProjectMembers.Add(new ProjectMemberEntity { ProjectId = model.Id, UserId = userId });
    //             }
    //         }
    //     }
    //
    //     _context.Update(model);
    //     await _context.SaveChangesAsync();
    //     
    //     return RedirectToAction("Index");
    // }
    

    [HttpPost]
    public async Task<IActionResult> AddProject(ProjectFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            return Json(new { success = false, errors });
        }

        // Map from ViewModel to DTO - passing the IFormFile directly to service layer
        var formData = new AddProjectFormDto
        {
            ProjectName = model.ProjectName,
            Description = model.Description ?? "",
            StartDate = model.StartDate ?? DateTime.UtcNow,
            EndDate = model.EndDate ?? DateTime.UtcNow.AddMonths(1),
            Budget = model.Budget,
            ClientId = model.ClientId,
            StatusId = model.StatusId != null ? int.Parse(model.StatusId) : 1, // Default to "Not Started"
            // MemberIds = model.MemberIds,
            ImageUrl = model.Image
        };

        // Use current user as createdBy
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userResult = await _userService.GetUserByIdAsync(userId);
        var user = userResult.Result;

        var result = await _projectService.CreateProjectAsync(formData, userId);

        if (result.Succeeded)
        {
            // Create notification for new project
            var notificationFormData = new NotificationDetailsDto
            {
                Title = "New Project Created",
                NotificationTypeId = 2,
                NotificationTargetId = 1,
                Message = $"{user?.FirstName} {user?.LastName} created a new project: {model.ProjectName}",
                ImageUrl = model.ImageUrl ?? FileUploadHelper.GetDefaultImage(UploadConstants.Directories.Projects),
                CreatedAt = DateTime.UtcNow
            };

            await _notificationService.AddNotificationAsync(notificationFormData);
            return Json(new { success = true, message = "Project created successfully" });
        }

        return Json(new { success = false, message = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProject(ProjectFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            return Json(new { success = false, errors });
        }

        // Map from ViewModel to DTO - passing the IFormFile directly to service layer
        var formData = new AddProjectFormDto
        {
            ProjectName = model.ProjectName,
            Description = model.Description ?? "",
            StartDate = model.StartDate ?? DateTime.UtcNow,
            EndDate = model.EndDate ?? DateTime.UtcNow.AddMonths(1),
            Budget = model.Budget,
            ClientId = model.ClientId,
            StatusId = int.Parse(model.StatusId),
            // MemberIds = model.MemberIds,
            ImageUrl = model.Image  // Using Image instead of File
        };

        var result = await _projectService.UpdateProjectAsync(model.Id!, formData);

        if (result.Succeeded)
        {
            return Json(new { success = true, message = "Project updated successfully" });
        }

        return Json(new { success = false, message = result.Error });
    }

    [HttpGet("projects/list")]
    public IActionResult GetFilteredProjects(string tab = "ALL")
    {
        var allProjects = ProjectsMockData.GetProjects();
        var filteredProjects = FilterProjects(allProjects, tab);

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return PartialView("Partials/Components/TabFilter/_FilteredProjects", filteredProjects);
        }

        return RedirectToAction("Index", new { tab });
    }

    [HttpGet("projects/{id}")]
    public IActionResult GetProject(string id)
    {
        var project = ProjectsMockData.GetProjects().FirstOrDefault(p => p.Id == id);
        if (project == null)
        {
            return NotFound();
        }

        return Json(new { success = true, project });
    }

    private static ProjectStatus DetermineProjectStatus(int statusId)
    {
        return statusId switch
        {
            1 => ProjectStatus.NotStarted,
            2 => ProjectStatus.InProgress,
            3 => ProjectStatus.Paused,
            4 => ProjectStatus.Completed,
            5 => ProjectStatus.Cancelled,
            _ => ProjectStatus.NotStarted
        };
    }

    private List<ProjectCardViewModel> FilterProjects(List<ProjectCardViewModel> projects, string tab)
    {
        if (tab == "ALL") return projects;

        if (Enum.TryParse(tab.Replace(" ", ""), true, out ProjectStatus status))
        {
            return projects.Where(p => p.Status == status).ToList();
        }

        return projects;
    }

    private string CalculateTimeLeft(string endDateStr)
    {
        if (!DateTime.TryParse(endDateStr, out DateTime endDate))
        {
            return "Unknown";
        }

        var timeSpan = endDate - DateTime.Now;

        // If the deadline has passed
        if (timeSpan.TotalDays < 0)
        {
            int days = Math.Abs((int)timeSpan.TotalDays);
            return days == 1 ? "1 day overdue" : $"{days} days overdue";
        }

        // Less than a week
        if (timeSpan.TotalDays < 7)
        {
            int days = (int)Math.Ceiling(timeSpan.TotalDays);
            return days == 1 ? "1 day left" : $"{days} days left";
        }

        // Less than a month
        if (timeSpan.TotalDays < 30)
        {
            int weeks = (int)Math.Ceiling(timeSpan.TotalDays / 7);
            return weeks == 1 ? "1 week left" : $"{weeks} weeks left";
        }

        // Less than a year
        if (timeSpan.TotalDays < 365)
        {
            int months = (int)Math.Ceiling(timeSpan.TotalDays / 30);
            return months == 1 ? "1 month left" : $"{months} months left";
        }

        // More than a year
        int years = (int)Math.Ceiling(timeSpan.TotalDays / 365);
        return years == 1 ? "1 year left" : $"{years} years left";
    }
}
