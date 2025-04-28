using System.Security.Claims;
using Business.Dtos;
using Business.Services;
using Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ASP.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class NotificationsController(IHubContext<NotificationHub> notificationHub, INotificationService notificationService) : ControllerBase
{
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
    private readonly INotificationService _notificationService = notificationService;

    [HttpPost]
    public async Task<IActionResult> CreateNotification(NotificationDetailsDto notificationDetailsDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        notificationDetailsDto.UserId = userId;

        Console.WriteLine($"[DEBUG] CreateNotification: Title='{notificationDetailsDto.Title}', Message='{notificationDetailsDto.Message}', ImageUrl='{notificationDetailsDto.ImageUrl}', ImageType='{notificationDetailsDto.ImageType}', UserId='{notificationDetailsDto.UserId}'");
        await _notificationService.AddNotificationAsync(notificationDetailsDto);
        var notifications = await _notificationService.GetNotificationsAsync("");
        var newNotification = notifications.Result?.OrderByDescending(x => x.CreatedAt).FirstOrDefault();

        if (newNotification != null)
        {
            await _notificationHub.Clients.All.SendAsync("ReceiveNotification", newNotification);
        }
        else
        {
            Console.WriteLine("[DEBUG] No new notification found to send via SignalR.");
        }
        return Ok(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var roleName = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _notificationService.GetNotificationsAsync(userId, roleName);
        if (!result.Succeeded)
            return NotFound(new { success = false, error = result.Error });

        return Ok(new { success = true, notifications = result.Result?.ToList() ?? [] });
    }
    

    [HttpPost("dismiss/{id}")]
    public async Task<IActionResult> DismissNotification(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _notificationService.DismissNotificationAsync(id, userId);
        await _notificationHub.Clients.All.SendAsync("NotificationDismissed", id);
        return Ok(new { success = true });
    }

}







