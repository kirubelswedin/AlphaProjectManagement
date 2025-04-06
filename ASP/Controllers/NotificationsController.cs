using System.Security.Claims;
using Business.Dtos;
using Business.Interfaces;
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
    public async Task<IActionResult> CreateNotification(NotificationFormData notificationFormData)
    {
        await _notificationService.AddNotificationAsync(notificationFormData);
        var notifications = await _notificationService.GetNotificationsAsync("");
        var newNotification = notifications.Result?.OrderByDescending(x => x.CreatedAt).FirstOrDefault();

        if (newNotification != null)
        {
            await _notificationHub.Clients.All.SendAsync("ReceiveNotification", newNotification);
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

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var roleName = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _notificationService.GetNotificationsAsync(userId, roleName);
        if (!result.Succeeded)
            return NotFound(new { error = result.Error });

        var unreadCount = result.Result?.Count(n => !n.IsRead) ?? 0;
        return Ok(new { success = true, count = unreadCount });
    }

    [HttpPost("read/{id}")]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Här skulle vi ha en metod för att markera som läst
        // await _notificationService.MarkAsReadAsync(id, userId);
        await _notificationHub.Clients.User(userId).SendAsync("NotificationRead", id);
        return Ok(new { success = true });
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

    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeleteNotification(string id)
    // {
    //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    //     if (string.IsNullOrEmpty(userId))
    //         return Unauthorized();

    //     await _notificationService.DeleteNotificationAsync(id, userId);
    //     return Ok(new { success = true });
    // }
}