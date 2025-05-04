using ASP.Extensions;
using ASP.ViewModels.Components;
using Business.Dtos;
using Domain.Models;

namespace ASP.Mappers;

public class NotificationViewModelMapper
{
    public static NotificationItemViewModel ToViewModel(NotificationDetailsDto notification)
    {
        return new NotificationItemViewModel
        {
            Id = notification.Id,
            Message = notification.Message,
            ImageUrl = notification.ImageUrl.GetImageUrl(notification.ImageType),
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead
        };
    }
}