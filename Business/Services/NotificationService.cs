using Business.Dtos;
using Business.Mappers;
using Data.Entities;
using Data.Repositories;
using Domain.Models;
using Domain.Responses;
using Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Business.Services;

public interface INotificationService
{
    Task<NotificationResult<Notification>> AddNotificationAsync(NotificationDetailsDto detailsDto, string userId = "");
    Task<NotificationResult<IEnumerable<Notification>>> GetNotificationsAsync(string userId, string? roleName = null, int take = 10);
    Task<int> GetTotalNotificationCountAsync(string userId, string? roleName = null);
    Task DismissNotificationAsync(string notificationId, string userId);
}

public class NotificationService(INotificationRepository notificationRepository, INotificationDismissedRepository notificationDismissedRepository, IHubContext<NotificationHub> notificationHub) : INotificationService
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly INotificationDismissedRepository _notificationDismissedRepository = notificationDismissedRepository;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;

    public async Task<NotificationResult<Notification>> AddNotificationAsync(NotificationDetailsDto? detailsDto, string userId = "")
    {
        Console.WriteLine($"[AddNotificationAsync] NotificationTypeId={detailsDto?.NotificationTypeId}, ImageUrl='{detailsDto?.ImageUrl}'");

        if (detailsDto == null)
            return new NotificationResult<Notification> { Succeeded = false, StatusCode = 400 };
        
        if (string.IsNullOrEmpty(detailsDto.ImageType))
        {
            detailsDto.ImageType = detailsDto.NotificationTypeId switch
            {
                1 => "avatars",  // user
                2 => "members",
                3 => "projects",
                4 => "clients",
                _ => "avatars"
            };
        }

        var notificationEntity = NotificationMapper.ToEntity(detailsDto);
        var result = await _notificationRepository.AddAsync(notificationEntity);

        var notifications = await GetNotificationsAsync(userId);
        var newNotification = notifications.Result?.OrderByDescending(x => x.CreatedAt).FirstOrDefault();

        if (result.Succeeded)
        {
            var notificationResult = await _notificationRepository.GetLatestNotification();
            await _notificationHub.Clients.All.SendAsync("ReceiveNotification", notificationResult.Result);
        }

        return result.Succeeded
            ? new NotificationResult<Notification> { Succeeded = true, StatusCode = 200 }
            : new NotificationResult<Notification> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    public async Task<NotificationResult<IEnumerable<Notification>>> GetNotificationsAsync(string userId, string? roleName = null, int take = 0)
    {
        const string adminTargetName = "Admin";
        var dismissedNotificationResult = await _notificationDismissedRepository.GetNotificationsIdsAsync(userId);
        var dismissedNotificationIds = dismissedNotificationResult.Result;

        var notificationResult = (!string.IsNullOrEmpty(roleName) && roleName == adminTargetName)
            ? await _notificationRepository.GetAllAsync(orderByDescending: true, sortByColumn: x => x.CreatedAt,
                filterBy: x => !dismissedNotificationIds!.Contains(x.Id), take: take)
            
            : await _notificationRepository.GetAllAsync(orderByDescending: true, sortByColumn: x => x.CreatedAt,
                filterBy: x => !dismissedNotificationIds!.Contains(x.Id) && x.NotificationTarget.TargetName != adminTargetName, take: take,
                includes: x => x.NotificationTarget);

        if (!notificationResult.Succeeded)
            return new NotificationResult<IEnumerable<Notification>> { Succeeded = false, StatusCode = 404 };

        var notifications = notificationResult.Result!.Select(NotificationMapper.ToModel);
        return new NotificationResult<IEnumerable<Notification>> { Succeeded = true, StatusCode = 200, Result = notifications };
    }
    
    public async Task<int> GetTotalNotificationCountAsync(string userId, string? roleName = null)
    {
        const string adminTargetName = "Admin";
        // Hämta notifications för rätt target (t.ex. Admin)
        var notificationResult = (!string.IsNullOrEmpty(roleName) && roleName == adminTargetName)
            ? await _notificationRepository.GetAllAsync(
                filterBy: x => true // Alla notifications för admin
            )
            : await _notificationRepository.GetAllAsync(
                filterBy: x => x.NotificationTarget.TargetName != adminTargetName,
                includes: x => x.NotificationTarget
            );

        return notificationResult.Result?.Count() ?? 0;
    }

    public async Task DismissNotificationAsync(string notificationId, string userId)
    {
        var exists = await _notificationDismissedRepository.ExistsAsync(x => x.NotificationId == notificationId && x.UserId == userId);
        if (!exists.Succeeded)
        {
            var entity = new NotificationDismissedEntity
            {
                NotificationId = notificationId,
                UserId = userId
            };

            await _notificationDismissedRepository.AddAsync(entity);
            await _notificationHub.Clients.All.SendAsync("NotificationDismissed", notificationId);
        }
    }
}