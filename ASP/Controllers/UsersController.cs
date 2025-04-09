using ASP.ViewModels.forms;
using ASP.ViewModels.MockData;
using ASP.ViewModels.Views;
using Business.Interfaces;
using Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Entities;

namespace ASP.Controllers;

[Authorize(Roles = "Admin")]
public class MembersController(
    IUserService userService, 
    AppDbContext context,
    UserManager<UserEntity> userManager,
    RoleManager<IdentityRole> roleManager) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly AppDbContext _context = context;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    [Route("admin/members")]
    public IActionResult Index()
    {
        var viewModel = new MembersViewModel
        {
            PageHeader = new()
            {
                Title = "Member",
                ButtonText = "Add Member",
                ModalId = "addMemberModal"
            },
            Members = MembersMockData.GetMembers()
        };

        return View(viewModel);
    }

    [HttpGet("members/{id}")]
    public async Task<IActionResult> GetMember(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        var member = new
        {
            Id = user.Id,
            Avatar = user.ImageUrl,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            JobTitle = user.JobTitle,
            IsAdmin = isAdmin
        };

        return Json(new { success = true, member });
    }

    [HttpGet("members/list")]
    public IActionResult GetMembersList()
    {
        var members = MembersMockData.GetMembers();
        return Json(new { success = true, members });
    }

    [HttpPost]
    public IActionResult AddMember(MembersFormViewModel model)
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


        return Json(new { success = true, message = "Member added successfully" });
    }

    [HttpPut("members/{id}")]
    public IActionResult UpdateMember(string id, MembersFormViewModel model)
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


        return Json(new { success = true, message = "Member updated successfully" });
    }

    [HttpDelete("members/{id}")]
    public IActionResult DeleteMember(string id)
    {

        return Json(new { success = true, message = "Member deleted successfully" });
    }
    
    // Search users in projects list
    [HttpGet("members/search")]
    public async Task<JsonResult> SearchUsers(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var users = await _context.Users
            .Where(x => x.FirstName!.Contains(term) || x.LastName!.Contains(term) || x.Email!.Contains(term))
            .Select(x => new { x.Id, x.ImageUrl, FullName = $"{x.FirstName} {x.LastName}" })
            .ToListAsync();

        return Json(users);
    }

    [HttpPost("members/{id}/toggle-admin")]
    public async Task<IActionResult> ToggleAdminRole(string id, [FromBody] bool isAdmin)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { success = false, message = "User not found" });
        }

        var currentUserIsAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        if (isAdmin && !currentUserIsAdmin)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }
        else if (!isAdmin && currentUserIsAdmin)
        {
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        }

        return Json(new { success = true, message = $"User admin status updated to {isAdmin}" });
    }
}