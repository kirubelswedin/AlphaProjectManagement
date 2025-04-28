using Business.Dtos;
using Data.Entities;
using Domain.Models;

namespace Business.Mappers;

public static class StatusMapper
{
    public static StatusEntity ToEntity(AddStatusFormDto? dto)
    {
        if (dto == null) return null!;
        return new StatusEntity
        {
            StatusName = dto.StatusName
        };
    }

    public static StatusEntity ToEntity(UpdateStatusFormDto? dto)
    {
        if (dto == null) return null!;
        return new StatusEntity
        {
            Id = dto.Id,
            StatusName = dto.StatusName
        };
    }

    public static Status ToModel(StatusEntity? entity)
    {
        if (entity == null) return null!; 
        return new Status
        {
            Id = entity.Id,
            StatusName = entity.StatusName,
            Color = entity.Color,
            SortOrder = entity.SortOrder,   
            IsDefault = entity.IsDefault,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}