using System.Diagnostics;
using System.Security.Claims;
using ASP.Extensions;
using ASP.Mappers;
using ASP.ViewModels.Components;
using ASP.ViewModels.Forms;
using ASP.ViewModels.Views;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Dtos;

namespace ASP.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController(IUserService userService, INotificationService notificationService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly INotificationService _notificationService = notificationService;
    
    [Route("admin/members")]
    public async Task<IActionResult> Index()
    {
        var viewModel = new MembersViewModel
        {
            PageHeader = CreatePageHeader(),
            Members = await GetMembersAsync(),
            AddMember = new AddMemberViewModel(),
            EditMember = new EditMemberViewModel()
        };

        return View(viewModel);
    }
    
    private static PageHeaderViewModel CreatePageHeader() => new()
    {
        Title = "Members",
        ButtonText = "Add Member",
        ModalId = "addmembermodal"
    };

    [HttpPost]
    public async Task<IActionResult> AddMember([FromForm] AddMemberViewModel model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetModelErrors() });

        var dto = MemberViewModelMapper.ToAddMemberFormDto(model);
        
        try
        {
            var result = await _userService.CreateMemberAsync(dto, User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
            if (!result.Succeeded)
                return Json(new { success = false, error = result.Error ?? "Failed to add Member" });

            // string imageUrl = result.Result?.ImageUrl ?? "default-member.svg";
            var userViewModel = MemberViewModelMapper.ToMemberCardViewModel(result.Result!);
            await CreateMemberNotification(userViewModel.ImageUrl,"Member Added",$"Member {dto.FirstName} {dto.LastName} has been added");
            return Json(new { success = true, member = userViewModel }); 
        }
        catch (Exception ex)
        { return Json(new { success = false, error = ex.Message }); }
    }

    [HttpGet("members/{id}")]
    public async Task<IActionResult> GetMember(string id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        if (!result.Succeeded || result.Result == null)
            return Json(new { success = false, error = result.Error });
        
        var userViewModel = MemberViewModelMapper.ToMemberCardViewModel(result.Result!);
        return Json(new { success = true, member = userViewModel });
    }
    
    private async Task<IEnumerable<MemberCardViewModel>> GetMembersAsync()
    {
        var result = await _userService.GetUsersAsync();
        if (!result.Succeeded) return [];

        var members = result.Result!.ToList();
        var memberCards = new List<MemberCardViewModel>();

        foreach (var member in members)
        {
            Console.WriteLine($"Member: {member.FirstName} {member.LastName}, ImageUrl: {member.ImageUrl}");
            var memberCard = MemberViewModelMapper.ToMemberCardViewModel(member);
            memberCards.Add(memberCard);
        }
        return memberCards;
    }

    [HttpPost]
    public async Task<IActionResult> EditMember([FromForm] EditMemberViewModel model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetModelErrors() });

        var dto = MemberViewModelMapper.ToUpdateMemberFormDto(model);
        try
        {
            var result = await _userService.UpdateMemberAsync(dto);
            if (!result.Succeeded)
                return Json(new { success = false, error = result.Error ?? "Failed to update member" });

            // Console.WriteLine($"[DEBUG] EditMember: ImageUrl='{result.Result?.ImageUrl}', ImageType='{result.Result?.ImageType}'");
            
            var userViewModel = MemberViewModelMapper.ToMemberCardViewModel(result.Result!);
            await CreateMemberNotification(userViewModel.ImageUrl,"Member Updated",$"Member {dto.FirstName} {dto.LastName} has been updated");
            return Json(new { success = true, member = userViewModel }); 
        }
        catch (Exception ex)
        { return Json(new { success = false, error = ex.Message }); }
    }
    
    [HttpDelete("members/{id}")]
    public async Task<IActionResult> DeleteMember(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (!user.Succeeded)
            return Json(new { success = false, error = user.Error });

        var result = await _userService.DeleteMemberAsync(id);
        if (!result.Succeeded)
            return Json(new { success = false, error = result.Error });
        
        var userViewModel = MemberViewModelMapper.ToMemberCardViewModel(user.Result!);
        await CreateMemberNotification(userViewModel.ImageUrl, "Member Deleted", $"Member {user.Result!.FirstName} {user.Result!.LastName} has been deleted");
        return Json(new { success = true, member = userViewModel });
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
                ImageUrl = (x.ImageUrl ?? "default-user.svg").GetImageUrl("users"),
                fullName = $"{x.FirstName} {x.LastName}",
            })
            .ToList();

        return Json(filteredMembers);
    }

    private async Task CreateMemberNotification(string? imageUrl, string title, string message)
    {
        var notificationDetails = new NotificationDetailsDto
        {
            NotificationTypeId = 1, // Member type
            NotificationTargetId = 2, // Admins
            Title = title,
            Message = message,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _notificationService.AddNotificationAsync(notificationDetails);
        if (!result.Succeeded)
        {
            var errorMessage = result.Error;
            Debug.WriteLine($"Failed to create notification: {errorMessage}");
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