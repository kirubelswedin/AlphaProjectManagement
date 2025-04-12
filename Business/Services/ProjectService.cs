using Business.Dtos;
using Business.Handlers;
using Business.Mappers;
using Data.Repositories;
using Domain.Models;
using Domain.Responses;

namespace Business.Services;

public interface IProjectService
{
    Task<ProjectResult> CreateProjectAsync(AddProjectFormDto formDto, string? createdById);
    Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync();
    Task<ProjectResult<Project>> GetProjectByIdAsync(string id);
    Task<ProjectResult> UpdateProjectAsync(UpdateProjectFormDto formDto);
    Task<ProjectResult> DeleteProjectAsync(string id);
    Task<ProjectResult<ProjectDetailsDto>> GetProjectDetailsAsync(string id);
}

public class ProjectService(IProjectRepository projectRepository, IImageHandler imageHandler, ICacheHandler<IEnumerable<Project>> cacheHandler) : IProjectService
{
    private const string _cacheKey = "Projects";

    public async Task<ProjectResult> CreateProjectAsync(AddProjectFormDto? formDto, string? createdById)
    {
        try
        {
            if (formDto == null)
                return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "All required fields are not provided" };

            string? ImageFile = null;
            if (formDto.ImageFile != null)
                ImageFile = await imageHandler.SaveImageAsync(formDto.ImageFile, "projects");

            var entity = ProjectMapper.ToEntity(formDto, ImageFile, createdById);
            var result = await projectRepository.AddAsync(entity);

            if (!result.Succeeded)
                return new ProjectResult { Succeeded = false, StatusCode = 500, Error = result.Error };

            await UpdateCacheAsync();
            return new ProjectResult { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        { return new ProjectResult { Succeeded = false, StatusCode = 500, Error = $"Failed to create project: {ex.Message}" }; }
    }

    public async Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync()
    {
        try
        {
            var cachedProjects = cacheHandler.GetFromCache(_cacheKey);
            if (cachedProjects != null)
                return new ProjectResult<IEnumerable<Project>> { Succeeded = true, StatusCode = 200, Result = cachedProjects };

            var result = await projectRepository.GetAllAsync
            (
                orderByDescending: true,
                sortByColumn: x => x.CreatedAt,
                includes:
                [
                    x => x.Client,
                    x => x.Status,
                    x => x.User,
                    x => x.ProjectMembers
                ]
            );

            if (!result.Succeeded)
                return new ProjectResult<IEnumerable<Project>>
                { Succeeded = false, StatusCode = 500, Error = "Failed to retrieve projects" };

            var entities = result.Result;
            var projects = entities?.Select(ProjectMapper.ToModel) ?? [];

            cacheHandler.SetCache(_cacheKey, projects);
            return new ProjectResult<IEnumerable<Project>> { Succeeded = true, StatusCode = 200, Result = projects };
        }
        catch (Exception ex)
        { return new ProjectResult<IEnumerable<Project>> { Succeeded = false, StatusCode = 500, Error = $"Failed to retrieve projects: {ex.Message}" }; }
    }

    public async Task<ProjectResult<Project>> GetProjectByIdAsync(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return new ProjectResult<Project> { Succeeded = false, StatusCode = 400, Error = "Project ID is required" };

            var cachedProjects = cacheHandler.GetFromCache(_cacheKey);
            var cachedProject = cachedProjects?.FirstOrDefault(x => x.Id == id);
            if (cachedProject != null)
                return new ProjectResult<Project> { Succeeded = true, StatusCode = 200, Result = cachedProject };

            var result = await projectRepository.GetAsync
            (
                x => x.Id == id,
                x => x.Client,
                x => x.Status,
                x => x.User,
                x => x.ProjectMembers
            );

            if (!result.Succeeded || result.Result == null)
                return new ProjectResult<Project> { Succeeded = false, StatusCode = 404, Error = $"Project with id '{id}' was not found." };

            var project = ProjectMapper.ToModel(result.Result);
            return new ProjectResult<Project> { Succeeded = true, StatusCode = 200, Result = project };
        }
        catch (Exception ex)
        { return new ProjectResult<Project> { Succeeded = false, StatusCode = 500, Error = $"Failed to retrieve project: {ex.Message}" }; }
    }

    public async Task<ProjectResult> UpdateProjectAsync(UpdateProjectFormDto? formDto)
    {
        try
        {
            if (formDto == null)
                return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Project data is required" };

            var existingProject = await projectRepository.GetAsync(x => x.Id == formDto.Id);
            if (!existingProject.Succeeded || existingProject.Result == null)
                return new ProjectResult { Succeeded = false, StatusCode = 404, Error = $"Project with id '{formDto.Id}' was not found." };

            // Handle image upload
            string? newImageUrl = null;
            if (formDto.NewImageFile != null)
                newImageUrl = await imageHandler.SaveImageAsync(formDto.NewImageFile, "projects");

            // Map DTO to entity using ProjectMapper
            var entity = ProjectMapper.ToEntity(formDto, newImageUrl);

            // Preserve existing timestamps and completion status
            entity.CreatedAt = existingProject.Result.CreatedAt;
            entity.UpdatedAt = DateTime.UtcNow;

            // Check if status was changed to completed
            bool wasCompleted = entity.StatusId == 4 && existingProject.Result.IsCompleted == false;
            if (wasCompleted)
            {
                entity.IsCompleted = true;
                entity.CompletedAt = DateTime.UtcNow;
                entity.CompletedOnTime = DateTime.UtcNow <= entity.EndDate;
            }
            // Check if project was re-opened from completed state
            else if (existingProject.Result.IsCompleted && formDto.StatusId != 4)
            {
                entity.IsCompleted = false;
                entity.CompletedAt = null;
                entity.CompletedOnTime = false;
            }

            var result = await projectRepository.UpdateAsync(entity);
            if (!result.Succeeded)
                return new ProjectResult { Succeeded = false, StatusCode = 500, Error = result.Error };

            await UpdateCacheAsync();
            return new ProjectResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        { return new ProjectResult { Succeeded = false, StatusCode = 500, Error = $"Failed to update project: {ex.Message}" }; }
    }

    public async Task<ProjectResult> DeleteProjectAsync(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Project ID is required" };

            var result = await projectRepository.DeleteAsync(x => x.Id == id);
            if (!result.Succeeded)
                return new ProjectResult { Succeeded = false, StatusCode = 500, Error = result.Error };

            await UpdateCacheAsync();
            return new ProjectResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        { return new ProjectResult { Succeeded = false, StatusCode = 500, Error = $"Failed to delete project: {ex.Message}" }; }
    }

    private async Task<IEnumerable<Project>> UpdateCacheAsync()
    {
        var result = await projectRepository.GetAllAsync
        (
            orderByDescending: true,
            sortByColumn: x => x.CreatedAt,
            includes:
            [
                x => x.Client,
                x => x.Status,
                x => x.User,
                x => x.ProjectMembers
            ]
        );

        if (!result.Succeeded)
            return [];

        var entities = result.Result;
        var projects = entities?.Select(ProjectMapper.ToModel) ?? [];
        cacheHandler.SetCache(_cacheKey, projects);
        return projects;
    }

    public async Task<ProjectResult<ProjectDetailsDto>> GetProjectDetailsAsync(string id)
    {
        try
        {
            var result = await projectRepository.GetAsync(
                x => x.Id == id,
                x => x.Status,
                x => x.Client,
                x => x.User,
                x => x.ProjectMembers,
                x => x.ProjectMembers.Select(m => m.User),
                x => x.ProjectMembers.Select(m => m.Role)
            );

            if (!result.Succeeded || result.Result == null)
                return new ProjectResult<ProjectDetailsDto> { Succeeded = false, Error = "Project not found" };

            var projectDetails = ProjectMapper.ToDetailsDto(result.Result);
            return new ProjectResult<ProjectDetailsDto> { Succeeded = true, Result = projectDetails };
        }
        catch (Exception ex)
        {
            return new ProjectResult<ProjectDetailsDto> { Succeeded = false, Error = ex.Message };
        }
    }
}