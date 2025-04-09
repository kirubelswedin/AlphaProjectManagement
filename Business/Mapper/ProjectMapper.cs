using Business.Dtos;
using Data.Entities;
using Domain.Models;

namespace Business.Mapper;

public static class ProjectMapper
{
    public static ProjectEntity ToEntity(AddProjectFormDto? dto, string? newImageUrl = null)
    {
        if (dto == null) return null!;
        return new ProjectEntity
        {
            ImageUrl = newImageUrl,
            ProjectName = dto.ProjectName,
            ClientId = dto.ClientId,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Budget = dto.Budget,
            UserId = dto.UserId,
        };
    }

    public static ProjectEntity ToEntity(UpdateProjectFormDto? dto, string? newImageUrl = null)
    {
        if (dto == null) return null!;
        return new ProjectEntity
        {
            Id = dto.Id,
            ImageUrl = newImageUrl ?? dto.ImageUrl,
            ProjectName = dto.ProjectName,
            ClientId = dto.ClientId,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Budget = dto.Budget,
            UserId = dto.UserId,
            StatusId = dto.StatusId
        };
    }

    public static Project ToModel(ProjectEntity? entity)
    {
        if (entity == null) return null!;
        return new Project
        {
            Id = entity.Id,
            ImageUrl = entity.ImageUrl,
            ProjectName = entity.ProjectName,
            Client = ClientMapper.ToModel(entity.Client),
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Budget = entity.Budget,
            User = UserMapper.ToModel(entity.User),
            Status = StatusMapper.ToModel(entity.Status),
        };
    }
}