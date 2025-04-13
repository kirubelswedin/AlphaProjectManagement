using Business.Dtos;
using Data.Entities;
using Domain.Models;

namespace Business.Mappers;

public static class ProjectMapper
{
    // AddProjectFormDto to ProjectEntity
    public static ProjectEntity ToEntity(AddProjectFormDto? dto, string? newImageUrl = null, string? createdById = null)
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
            UserId = createdById ?? dto.UserId,
            Budget = dto.Budget,
            StatusId = 1, // default status
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // UpdateProjectFormDto to ProjectEntity
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
            UserId = dto.UserId,
            Budget = dto.Budget,
            StatusId = dto.StatusId
        };
    }

    // ProjectEntity to a Project
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
            IsCompleted = entity.IsCompleted,
            CompletedOnTime = entity.CompletedOnTime,
            CompletedAt = entity.CompletedAt,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            ProjectMembers = entity.ProjectMembers.Select(pm => new ProjectMember
            {
                Id = pm.Id,
                ProjectId = pm.ProjectId,
                UserId = pm.UserId,
                User = UserMapper.ToModel(pm.User),
                RoleId = pm.RoleId,
                Role = new ProjectRole
                {
                    Id = pm.Role.Id,
                    Name = pm.Role.Name,
                    Description = pm.Role.Description,
                    IsDefault = pm.Role.IsDefault
                },
                JoinedAt = pm.JoinedAt,
                UpdatedAt = pm.UpdatedAt
            }).ToList()
        };
    }

    // ProjectEntity to a ProjectDetailsDto
    public static ProjectDetailsDto ToDetailsDto(ProjectEntity? entity)
    {
        if (entity == null)
            return null!;

        return new ProjectDetailsDto
        {
            // Basic Information
            Id = entity.Id,
            ImageUrl = entity.ImageUrl,
            ProjectName = entity.ProjectName,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Budget = entity.Budget,

            // Status Information
            StatusId = entity.Status.Id,
            StatusName = entity.Status.StatusName,
            StatusColor = entity.Status.Color,

            // Client Information
            ClientId = entity.Client.Id,
            ClientName = entity.Client.ClientName,
            ClientEmail = entity.Client.Email,
            ClientContactPerson = $"{entity.Client.FirstName} {entity.Client.LastName}",

            // Creator Information
            CreatedById = entity.UserId,
            CreatedByName = $"{entity.User.FirstName} {entity.User.LastName}",
            CreatedByImageUrl = entity.User.ImageUrl,

            // Members
            Members = entity.ProjectMembers.Select(member => new ProjectMemberDto
            {
                UserId = member.UserId,
                FullName = $"{member.User.FirstName} {member.User.LastName}",
                ImageUrl = member.User.ImageUrl,
                RoleId = member.RoleId,
                RoleName = member.Role.Name,
                JoinedAt = member.JoinedAt
            }).ToList(),

            // Statistics for dashboard etc
            TotalMembers = entity.ProjectMembers.Count,
            CompletedTasks = 0,
            TotalTasks = 0,

            // Time Tracking
            IsCompleted = entity.IsCompleted,
            CompletedOnTime = entity.CompletedOnTime,
            CompletedAt = entity.CompletedAt
        };
    }

    public static void UpdateFromDto(ProjectEntity entity, UpdateProjectFormDto dto)
    {
        entity.ProjectName = dto.ProjectName;
        entity.Description = dto.Description;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.Budget = dto.Budget;
        entity.StatusId = dto.StatusId;
        entity.ClientId = dto.ClientId;
    }
}