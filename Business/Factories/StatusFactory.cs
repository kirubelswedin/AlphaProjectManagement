using Data.Entities;
using Domain.Models;

namespace Business.Factories;

public interface IStatusFactory
{
    StatusEntity CreateStatusEntity(Status status);
    Status CreateStatusModel(StatusEntity entity);
}

public class StatusFactory : IStatusFactory
{
    public StatusEntity CreateStatusEntity(Status model)
    {
        return new StatusEntity
        {
            StatusName = model.StatusName,
            Color = model.Color,
            SortOrder = model.SortOrder,
            IsDefault = model.IsDefault,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public Status CreateStatusModel(StatusEntity entity)
    {
        return new Status
        {
            Id = entity.Id,
            StatusName = entity.StatusName,
            Color = entity.Color,
            SortOrder = entity.SortOrder,
            IsDefault = entity.IsDefault,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }
}