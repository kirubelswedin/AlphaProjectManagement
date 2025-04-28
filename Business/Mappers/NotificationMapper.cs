using Business.Dtos;
using Data.Entities;
using Domain.Models;

namespace Business.Mappers;

public static class NotificationMapper
{
  public static NotificationEntity ToEntity(NotificationDetailsDto dto)
  {
    return new NotificationEntity
    {
      Id = Guid.NewGuid().ToString(),
      NotificationTypeId = dto.NotificationTypeId,
      NotificationTargetId = dto.NotificationTargetId,
      Title = dto.Title,
      ImageUrl = dto.ImageUrl,
      ImageType = dto.ImageType,
      Message = dto.Message,
      CreatedAt = dto.CreatedAt,
      IsRead = false
    };
  }

  public static Notification ToModel(NotificationEntity entity)
  {
    return new Notification
    {
      Id = entity.Id,
      ImageUrl = entity.ImageUrl,
      ImageType = entity.ImageType,
      Message = entity.Message,
      IsRead = entity.IsRead,
      CreatedAt = entity.CreatedAt
    };
  }
}
