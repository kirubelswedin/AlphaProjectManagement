using ASP.ViewModels.Components;
using ASP.ViewModels.forms;
using ASP.ViewModels.Forms;
using ASP.ViewModels.Views;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Dtos;
using Business.Mappers;

namespace ASP.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController(
    IUserService userService,
    INotificationService notificationService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly INotificationService _notificationService = notificationService;

    [Route("admin/members")]
    public async Task<IActionResult> Index()
    {
        var result = await _userService.GetUsersAsync();
        var viewModel = new MembersViewModel
        {
            PageHeader = CreatePageHeader(),
            Members = result.Succeeded ? result.Result?.Select(user =>
            {
                var userDetails = UserMapper.ToUserDetailsDto(user);
                return new MemberCardViewModel
                {
                    Id = userDetails.Id,
                    Avatar = userDetails.Avatar,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Email = userDetails.Email,
                    PhoneNumber = userDetails.PhoneNumber,
                    JobTitle = userDetails.JobTitle,
                    StreetAddress = userDetails.StreetAddress,
                    City = userDetails.City,
                    PostalCode = userDetails.PostalCode,
                    IsAdmin = userDetails.IsAdmin
                };
            }) ?? [] : [],
            AddMember = new AddMembersViewModel(),
            EditProject = new EditMembersViewModel()
        };

        return View(viewModel);
    }

    [HttpGet("members/list")]
    public async Task<IActionResult> GetMembers()
    {
        var result = await _userService.GetUsersAsync();
        if (!result.Succeeded)
            return Json(new { success = false, error = result.Error });

        var members = result.Result?.Select(user =>
        {
            var userDetails = UserMapper.ToUserDetailsDto(user);
            return new MemberCardViewModel
            {
                Id = userDetails.Id,
                Avatar = userDetails.Avatar,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Email = userDetails.Email,
                PhoneNumber = userDetails.PhoneNumber,
                JobTitle = userDetails.JobTitle,
                StreetAddress = userDetails.StreetAddress,
                City = userDetails.City,
                PostalCode = userDetails.PostalCode,
                IsAdmin = userDetails.IsAdmin
            };
        }) ?? [];
        return Json(new { success = true, members });
    }

    [HttpPost]
    public async Task<IActionResult> AddMember(AddUserFormDto dto)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetModelErrors() });

        var result = await _userService.CreateMemberAsync(dto);
        if (!result.Succeeded)
            return Json(new { success = false, error = result.Error });

        await CreateMemberNotification("New Member Added",
            $"Member {dto.FirstName} {dto.LastName} has been added successfully.");
        return Json(new { success = true });
    }

    [HttpGet("members/{id}")]
    public async Task<IActionResult> GetMember(string id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        if (!result.Succeeded)
            return Json(new { success = false, error = result.Error });

        return Json(new { success = true, member = result.Result });
    }

    [HttpPut("members/{id}")]
    public async Task<IActionResult> UpdateMember(string id, UpdateUserFormDto dto)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetModelErrors() });

        var result = await _userService.UpdateMemberAsync(dto);
        if (!result.Succeeded)
            return Json(new { success = false, error = result.Error });

        await CreateMemberNotification("Member Updated",
            $"Member {dto.FirstName} {dto.LastName} has been updated successfully.");
        return Json(new { success = true });
    }

    [HttpDelete("members/{id}")]
    public async Task<IActionResult> DeleteMember(string id)
    {
        var result = await _userService.DeleteMemberAsync(id);
        if (!result.Succeeded)
            return Json(new { success = false, error = result.Error });

        await CreateMemberNotification("Member Deleted", "A member has been deleted");
        return Json(new { success = true });
    }

    [HttpGet("members/search")]
    public async Task<IActionResult> SearchMembers(string term)
    {
        var result = await _userService.GetUsersAsync();
        if (!result.Succeeded)
            return Json(new List<object>());

        var filteredMembers = result.Result!
            .Where(x =>
                (x.FirstName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.LastName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.Email?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))
            .Select(x => new
            {
                id = x.Id,
                fullName = $"{x.FirstName} {x.LastName}",
                imageUrl = x.ImageUrl ?? "/images/avatars/default-avatar.svg"
            })
            .ToList();

        return Json(filteredMembers);
    }

    private static PageHeaderViewModel CreatePageHeader() => new()
    {
        Title = "Members",
        ButtonText = "Add Member",
        ModalId = "addmembermodal"
    };

    private async Task CreateMemberNotification(string title, string message)
    {
        var notification = new NotificationDetailsDto
        {
            NotificationTypeId = 1, // Member type
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
}