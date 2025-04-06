using Business.Dtos;
using Domain.Models;
using Domain.Responses;

namespace Business.Interfaces;

public interface INotificationService
{
    Task<NotificationResult> AddNotificationAsync(NotificationFormData formData, string userId = "");
    Task<NotificationResult<IEnumerable<Notification>>> GetNotificationsAsync(string userId, string? roleName = null, int take = 10);
    Task DismissNotificationAsync(string notificationId, string userId);
}