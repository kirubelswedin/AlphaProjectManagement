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
      CreatedAt = DateTime.SpecifyKind(dto.CreatedAt, DateTimeKind.Utc),
      IsRead = false
    };
  }

  public static NotificationDetailsDto ToDetailsDto(NotificationEntity entity)
  {
    return new NotificationDetailsDto
    {
      Id = entity.Id,
      NotificationTypeId = entity.NotificationTypeId,
      NotificationTargetId = entity.NotificationTargetId,
      Title = entity.Title,
      ImageUrl = entity.ImageUrl,
      ImageType = entity.ImageType,
      Message = entity.Message,
      CreatedAt = DateTime.SpecifyKind(entity.CreatedAt, DateTimeKind.Utc),
      IsRead = entity.IsRead
    };
  }
}
