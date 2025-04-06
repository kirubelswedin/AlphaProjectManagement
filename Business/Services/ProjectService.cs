using Business.Dtos.Forms;
using Business.Interfaces;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;


namespace Business.Services;

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

    public async Task<ProjectResult> CreateProjectAsync(ProjectFormData formData, string userId)
    {
        try
        {
            // Handle image upload
            string? imagePath = null;
            if (formData.Image != null)
            {
                imagePath = await _fileUploadService.UploadFileAsync(formData.Image, "projects");
            }

            var entity = new Data.Entities.ProjectEntity
            {
                Image = imagePath,
                ProjectName = formData.ProjectName,
                Description = formData.Description,
                StartDate = formData.StartDate,
                EndDate = formData.EndDate,
                Budget = formData.Budget,
                ClientId = formData.ClientId,
                StatusId = formData.StatusId,
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

    public async Task<ProjectResult> UpdateProjectAsync(string id, ProjectFormData formData)
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
            if (formData.Image != null)
            {
                entity.Image = await _fileUploadService.UploadFileAsync(formData.Image, "projects");
            }

            entity.ProjectName = formData.ProjectName;
            entity.Description = formData.Description;
            entity.StartDate = formData.StartDate;
            entity.EndDate = formData.EndDate;
            entity.Budget = formData.Budget;
            entity.ClientId = formData.ClientId;
            entity.StatusId = formData.StatusId;
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
            else if (entity.IsCompleted && formData.StatusId != 4)
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