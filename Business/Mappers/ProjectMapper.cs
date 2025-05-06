using Business.Dtos;
using Data.Entities;
using Domain.Models;

namespace Business.Mappers;

public static class ProjectMapper
{
    public static ProjectEntity ToEntity(AddProjectFormDto? dto, string? newImageUrl = null, string? createdById = null)
    {
        if (dto == null) return null!;

        if (string.IsNullOrEmpty(createdById))
            throw new ArgumentException("CreatedById is required", nameof(createdById));

        return new ProjectEntity
        {
            Id = Guid.NewGuid().ToString(),
            ImageUrl = newImageUrl,
            ProjectName = dto.ProjectName,
            ClientId = dto.ClientId,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Budget = dto.Budget,
            UserId = createdById,
            StatusId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    
    // took some help from ChatGPT to get the mapping correct
    public static ProjectDetailsDto ToDetailsDto(ProjectEntity? entity)
    {
        if (entity == null) return null!;
        // Console.WriteLine("[ProjectMapper.ToDetailsDto] entity.Id=" + entity.Id + ", entity.ImageUrl=" + entity.ImageUrl);
        
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
            
            Status = new Status
            {
                Id = entity.Status.Id,
                StatusName = entity.Status.StatusName,
                Color = entity.Status.Color,
                SortOrder = entity.Status.SortOrder,
                IsDefault = entity.Status.IsDefault,
                CreatedAt = entity.Status.CreatedAt,
                UpdatedAt = entity.Status.UpdatedAt
            },

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
                RoleId = member.RoleId,
                RoleName = member.Role.Name,
                JoinedAt = member.JoinedAt,
                User = new UserDetailsDto
                {
                    Id = member.User.Id,
                    FirstName = member.User.FirstName,
                    LastName = member.User.LastName,
                    ImageUrl = member.User.ImageUrl,
                    Email = member.User.Email,
                    PhoneNumber = member.User.PhoneNumber,
                    JobTitle = member.User.JobTitle,
                    StreetAddress = member.User.Address?.StreetAddress ?? "",
                    City = member.User.Address?.City ?? "",
                    PostalCode = member.User.Address?.PostalCode ?? "",
                }
            }).ToList(),
            
            // Time Tracking
            IsCompleted = entity.IsCompleted,
            CompletedOnTime = entity.CompletedOnTime,
            CompletedAt = entity.CompletedAt,
            
            // Statistics for dashboard etc
            TotalMembers = entity.ProjectMembers.Count,
            // CompletedTasks = 0,
            // TotalTasks = 0,
        };
    }
    
    public static void ApplyUpdatesToEntity(UpdateProjectFormDto? dto, ProjectEntity? entity, string? newImageUrl = null)
    {
        if (dto == null || entity == null) return;

        entity.ImageUrl = newImageUrl ?? entity.ImageUrl;
        entity.ProjectName = dto.ProjectName;
        entity.ClientId = dto.ClientId;
        entity.Description = dto.Description;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.Budget = dto.Budget;
        entity.StatusId = dto.StatusId; 
        entity.UpdatedAt = DateTime.UtcNow;

        // Handle completion logic
        var wasCompleted = entity.StatusId == 4 && !entity.IsCompleted;
        if (wasCompleted)
        {
            entity.IsCompleted = true;
            entity.CompletedAt = DateTime.UtcNow;
            entity.CompletedOnTime = DateTime.UtcNow <= entity.EndDate;
        }
        else if (entity.IsCompleted && dto.StatusId != 4)
        {
            entity.IsCompleted = false;
            entity.CompletedAt = null;
            entity.CompletedOnTime = false;
        }
    }
}