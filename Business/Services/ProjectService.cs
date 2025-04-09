using Business.Dtos;
using Business.Interfaces;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;


namespace Business.Services;

public interface IProjectService
{
    Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync();
    Task<ProjectResult<Project>> GetProjectByIdAsync(string id);
    Task<ProjectResult> CreateProjectAsync(AddProjectFormDto formDto, string createdById);
    Task<ProjectResult> UpdateProjectAsync(string id, AddProjectFormDto formDto);
    Task<ProjectResult> DeleteProjectAsync(string id);
}

public class ProjectService(IProjectRepository projectRepository, IFileUploadService fileUploadService) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IFileUploadService _fileUploadService = fileUploadService;

    public async Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync()
    {
        var repositoryResult = await _projectRepository.GetAllAsync
        (
            orderByDescending: true,
            sortByColumn: x => x.CreatedAt,
            includes:
            [
                x => x.Client,
                x => x.Status,
                x => x.User
            ]
        );

        var entities = repositoryResult.Result;
        var projects = entities?.Select(entity => entity.MapTo<Project>()) ?? [];

        return new ProjectResult<IEnumerable<Project>>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = projects
        };
    }

    public async Task<ProjectResult<Project>> GetProjectByIdAsync(string id)
    {
        var repositoryResult = await _projectRepository.GetAsync
        (
            x => x.Id == id,
            x => x.Client,
            x => x.Status,
            x => x.User
        );

        var entity = repositoryResult.Result;
        if (entity == null)
            return new ProjectResult<Project>
            {
                Succeeded = false,
                StatusCode = 404,
                Error = $"Project with id '{id}' was not found."
            };

        var project = entity.MapTo<Project>();
        return new ProjectResult<Project>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = project
        };
    }

    public async Task<ProjectResult> CreateProjectAsync(AddProjectFormDto formDto, string userId)
    {
        try
        {
            // Handle image upload
            string? imagePath = null;
            if (formDto.ImageUrl != null)
            {
                imagePath = await _fileUploadService.UploadFileAsync(formDto.ImageUrl, "projects");
            }

            var entity = new Data.Entities.ProjectEntity
            {
                ImageUrl = imagePath,
                ProjectName = formDto.ProjectName,
                Description = formDto.Description,
                StartDate = formDto.StartDate,
                EndDate = formDto.EndDate,
                Budget = formDto.Budget,
                ClientId = formDto.ClientId,
                StatusId = formDto.StatusId,
                UserId = userId,
                IsCompleted = false,
                CompletedOnTime = false,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _projectRepository.AddAsync(entity);

            return new ProjectResult
            {
                Succeeded = result.Succeeded,
                StatusCode = result.Succeeded ? 201 : 500,
                Error = result.Error
            };
        }
        catch (Exception ex)
        {
            return new ProjectResult
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message
            };
        }
    }

    public async Task<ProjectResult> UpdateProjectAsync(string id, AddProjectFormDto formDto)
    {
        try
        {
            var existingProject = await _projectRepository.GetAsync(x => x.Id == id);
            if (!existingProject.Succeeded || existingProject.Result == null)
                return new ProjectResult
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = $"Project with id '{id}' was not found."
                };

            var entity = existingProject.Result;

            // Handle image upload
            if (formDto.ImageUrl != null)
            {
                entity.ImageUrl = await _fileUploadService.UploadFileAsync(formDto.ImageUrl, "projects");
            }

            entity.ProjectName = formDto.ProjectName;
            entity.Description = formDto.Description;
            entity.StartDate = formDto.StartDate;
            entity.EndDate = formDto.EndDate;
            entity.Budget = formDto.Budget;
            entity.ClientId = formDto.ClientId;
            entity.StatusId = formDto.StatusId;
            entity.UpdatedAt = DateTime.UtcNow;

            // Check if status was changed to completed
            bool wasCompleted = entity.StatusId == 4 && entity.IsCompleted == false;
            if (wasCompleted)
            {
                entity.IsCompleted = true;
                entity.CompletedAt = DateTime.UtcNow;
                entity.CompletedOnTime = DateTime.UtcNow <= entity.EndDate;
            }
            // Check if project was re-opened from completed state
            else if (entity.IsCompleted && formDto.StatusId != 4)
            {
                entity.IsCompleted = false;
                entity.CompletedAt = null;
                entity.CompletedOnTime = false;
            }

            var result = await _projectRepository.UpdateAsync(entity);
            return new ProjectResult
            {
                Succeeded = result.Succeeded,
                StatusCode = result.Succeeded ? 200 : 500,
                Error = result.Error
            };
        }
        catch (Exception ex)
        {
            return new ProjectResult
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message
            };
        }
    }

    public async Task<ProjectResult> DeleteProjectAsync(string id)
    {
        var result = await _projectRepository.DeleteAsync(x => x.Id == id);
        return new ProjectResult
        {
            Succeeded = result.Succeeded,
            StatusCode = result.Succeeded ? 200 : 500,
            Error = result.Error
        };
    }
}