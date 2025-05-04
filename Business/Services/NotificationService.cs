using Business.Dtos;
using Business.Mappers;
using Data.Entities;
using Data.Repositories;
using Domain.Responses;
using Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Business.Services;

public interface INotificationService
{
    Task<NotificationResult<NotificationDetailsDto>> AddNotificationAsync(NotificationDetailsDto detailsDto, string userId = "");
    Task<NotificationResult<IEnumerable<NotificationDetailsDto>>> GetNotificationsAsync(string userId, string? roleName = null, int take = 100);
    Task<int> GetTotalNotificationCountAsync(string userId, string? roleName = null);
    Task DismissNotificationAsync(string notificationId, string userId);
}

public class NotificationService(INotificationRepository notificationRepository, INotificationDismissedRepository notificationDismissedRepository, IHubContext<NotificationHub> notificationHub) : INotificationService
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly INotificationDismissedRepository _notificationDismissedRepository = notificationDismissedRepository;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;

    // Adds a new notification, sets default image type if missing, saves to DB, and notifies all clients via SignalR.
    public async Task<NotificationResult<NotificationDetailsDto>> AddNotificationAsync(NotificationDetailsDto? detailsDto, string userId = "")
    {
        // Console.WriteLine($"[AddNotificationAsync] NotificationTypeId={detailsDto?.NotificationTypeId}, ImageUrl='{detailsDto?.ImageUrl}'");
        try
        {
            if (detailsDto == null)
                return new NotificationResult<NotificationDetailsDto> { Succeeded = false, StatusCode = 400 };
            
            if (string.IsNullOrEmpty(detailsDto.ImageType))
            {
                detailsDto.ImageType = detailsDto.NotificationTypeId switch
                {
                    1 => "users",
                    2 => "projects",
                    3 => "clients",
                    _ => "clients"
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
            
            var dto = NotificationMapper.ToDetailsDto(notificationEntity);
            return new NotificationResult<NotificationDetailsDto> { Succeeded = result.Succeeded, StatusCode = result.StatusCode, Error = result.Error, Result = dto };
        }
        catch (Exception ex)
        { return new NotificationResult<NotificationDetailsDto> { Succeeded = false, StatusCode = 500, Error = $"Failed to add notification: {ex.Message}" }; }
        
    }

    // Gets notifications for a user, filtered by role and excluding dismissed notifications.
    public async Task<NotificationResult<IEnumerable<NotificationDetailsDto>>> GetNotificationsAsync(string userId, string? roleName = null, int take = 100)
    {
        Console.WriteLine($"[DEBUG] GetNotificationsAsync called with userId: '{userId}', roleName: '{roleName}'");
        
        const string adminTargetName = "Admin";
        var dismissedNotificationResult = await _notificationDismissedRepository.GetNotificationsIdsAsync(userId);
        var dismissedNotificationIds = dismissedNotificationResult.Result;
        
        // Admins see all, others see only their own target notifications 
        var notificationResult = (!string.IsNullOrEmpty(roleName) && roleName == adminTargetName)
            ? await _notificationRepository.GetAllAsync(orderByDescending: true, sortByColumn: x => x.CreatedAt,
                filterBy: x => !dismissedNotificationIds!.Contains(x.Id), take: take)
            
            : await _notificationRepository.GetAllAsync(orderByDescending: true, sortByColumn: x => x.CreatedAt,
                filterBy: x => !dismissedNotificationIds!.Contains(x.Id) && x.NotificationTarget.TargetName != adminTargetName, take: take,
                includes: x => x.NotificationTarget);


        if (!notificationResult.Succeeded)
            return new NotificationResult<IEnumerable<NotificationDetailsDto>> { Succeeded = false, StatusCode = notificationResult.StatusCode, Error = notificationResult.Error };

        var notifications = notificationResult.Result!.Select(NotificationMapper.ToDetailsDto).ToList();
        return new NotificationResult<IEnumerable<NotificationDetailsDto>> { Succeeded = true, StatusCode = 200, Result = notifications };
    }
    
    public async Task<int> GetTotalNotificationCountAsync(string userId, string? roleName = null)
    {
        const string adminTargetName = "Admin";
        // Get notifications for right target
        var notificationResult = (!string.IsNullOrEmpty(roleName) && roleName == adminTargetName)
            ? await _notificationRepository.GetAllAsync(
                filterBy: x => true 
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