using Business.Dtos;
using Business.Handlers;
using Business.Mappers;
using Data.Entities;
using Data.Repositories;
using Domain.Responses;

namespace Business.Services;

public interface IProjectService
{
    Task<ProjectResult<ProjectDetailsDto>> CreateProjectAsync(AddProjectFormDto formDto, string? createdById);
    Task<ProjectResult<IEnumerable<ProjectDetailsDto>>> GetProjectsAsync();
    Task<ProjectResult<ProjectDetailsDto>> GetProjectByIdAsync(string id);
    Task<ProjectResult<ProjectDetailsDto>> UpdateProjectAsync(UpdateProjectFormDto formDto);
    Task<ProjectResult> RemoveProjectMemberAsync(string projectId, string userId);
    Task<ProjectResult> RemoveUserFromAllProjectsAsync(string userId);
    Task<ProjectResult<ProjectDetailsDto>> DeleteProjectAsync(string id);
    Task<ProjectResult<ProjectDetailsDto>> GetProjectDetailsAsync(string id);
}

public class ProjectService(
    IProjectRepository projectRepository,
    IImageHandler imageHandler,
    ICacheHandler<IEnumerable<ProjectDetailsDto>> cacheHandler
    )
    : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IImageHandler _imageHandler = imageHandler;
    
    private const string _cacheKey = "Projects";

    public async Task<ProjectResult<ProjectDetailsDto>> CreateProjectAsync(AddProjectFormDto? formDto, string? createdById)
    {
        try
        {
            if (formDto == null)
                return new ProjectResult<ProjectDetailsDto> { Succeeded = false, StatusCode = 400, Error = "All required fields are not provided" };
            
            string? imageUrl = null;
            if (formDto.ImageFile != null)
                imageUrl = await _imageHandler.SaveImageAsync(formDto.ImageFile, "projects");

            var entity = ProjectMapper.ToEntity(formDto, imageUrl, createdById);
            var result = await _projectRepository.AddAsync(entity);

            if (!result.Succeeded)
                return new ProjectResult<ProjectDetailsDto> { Succeeded = false, StatusCode = 500, Error = result.Error };

            var dto = ProjectMapper.ToDetailsDto(entity);
            
            // adds project members
            if (formDto.SelectedMemberIds.Any())
            {
                var failedMembers = new List<string>();
                foreach (var memberId in formDto.SelectedMemberIds.Where(memberId => !string.IsNullOrEmpty(memberId)))
                {
                    var memberResult = await _projectRepository.AddProjectMemberAsync(entity.Id, memberId);
                    if (!memberResult.Succeeded)
                    {
                        failedMembers.Add($"{memberId} ({memberResult.Error})");
                    }
                }
                
                if (failedMembers.Count != 0)
                {
                    return new ProjectResult<ProjectDetailsDto>
                    { Succeeded = true, StatusCode = 201, Result = dto, Error = $"Project created, but some members could not be added: {string.Join(", ", failedMembers)}" };
                }
            }
            await UpdateCacheAsync();
            return new ProjectResult<ProjectDetailsDto> { Succeeded = true, StatusCode = 201, Result = dto };
        }
        catch (Exception ex)
        { return new ProjectResult<ProjectDetailsDto> { Succeeded = false, StatusCode = 500, Error = $"Failed to create project: {ex.Message}" }; }
    }

    public async Task<ProjectResult<IEnumerable<ProjectDetailsDto>>> GetProjectsAsync()
    {
        try
        {
            var cachedProjects = cacheHandler.GetFromCache(_cacheKey);
            if (cachedProjects != null)
                return new ProjectResult<IEnumerable<ProjectDetailsDto>> { Succeeded = true, StatusCode = 200, Result = cachedProjects };

            var result = await _projectRepository.GetAllAsync
            (
                orderByDescending: true,
                sortByColumn: x => x.CreatedAt,
                includes:
                [
                    x => x.Client,
                    x => x.Status,
                    x => x.User,
                    x => x.ProjectMembers,
                    x => x.ProjectMembers.Select(pm => pm.User),
                    x => x.ProjectMembers.Select(pm => pm.Role)
                ]
            );

            if (!result.Succeeded)
                return new ProjectResult<IEnumerable<ProjectDetailsDto>>
                { Succeeded = false, StatusCode = 500, Error = "Failed to retrieve projects" };

            var entities = result.Result;
            var projects = entities?.Select(ProjectMapper.ToDetailsDto).ToList() ?? [];

            cacheHandler.SetCache(_cacheKey, projects);
            return new ProjectResult<IEnumerable<ProjectDetailsDto>> { Succeeded = true, StatusCode = 200, Result = projects };
        }
        catch (Exception ex)
        { return new ProjectResult<IEnumerable<ProjectDetailsDto>> { Succeeded = false, StatusCode = 500, Error = $"Failed to retrieve projects: {ex.Message}" }; }
    }

    public async Task<ProjectResult<ProjectDetailsDto>> GetProjectByIdAsync(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return new ProjectResult<ProjectDetailsDto> { Succeeded = false, StatusCode = 400, Error = "Project ID is required" };

            var cachedProjects = cacheHandler.GetFromCache(_cacheKey);
            var cachedProject = cachedProjects?.FirstOrDefault(x => x.Id == id);
            if (cachedProject != null)
                return new ProjectResult<ProjectDetailsDto> { Succeeded = true, StatusCode = 200, Result = cachedProject };

            var result = await _projectRepository.GetAsync
            (
                x => x.Id == id,
                x => x.Client,
                x => x.Status,
                x => x.User,
                x => x.ProjectMembers
            );

            if (!result.Succeeded || result.Result == null)
                return new ProjectResult<ProjectDetailsDto> { Succeeded = false, StatusCode = 404, Error = $"Project with id '{id}' was not found." };

            var project = ProjectMapper.ToDetailsDto(result.Result);
            return new ProjectResult<ProjectDetailsDto> { Succeeded = true, StatusCode = 200, Result = project };
        }
        catch (Exception ex)
        { return new ProjectResult<ProjectDetailsDto> { Succeeded = false, StatusCode = 500, Error = $"Failed to retrieve project: {ex.Message}" }; }
    }

    // took some help from ChatGPT to refactor this method and get everything right
    public async Task<ProjectResult<ProjectDetailsDto>> UpdateProjectAsync(UpdateProjectFormDto formDto)
    {
        try
        {
            // Fetch the tracked project entity with members included
            var existingProjectResult = await _projectRepository.GetAsync(
                x => x.Id == formDto.Id,
                includes: [
                    x => x.ProjectMembers,
                    x => x.ProjectMembers.Select(pm => pm.User)
                ]);

            if (!existingProjectResult.Succeeded || existingProjectResult.Result == null)
                return new ProjectResult<ProjectDetailsDto> { Succeeded = false, StatusCode = 404, Error = $"Project with id '{formDto.Id}' was not found." };

            var entity = existingProjectResult.Result;

            // Handle image upload, keep old image if no new image is provided
            string? finalImageUrl = entity.ImageUrl;
            if (formDto.ImageFile != null)
            {
                finalImageUrl = await _imageHandler.SaveImageAsync(formDto.ImageFile, "projects");
            }
            
            ProjectMapper.ApplyUpdatesToEntity(formDto, entity, finalImageUrl);

            // Sync project members if any are provided
            if (formDto.SelectedMemberIds?.Any() == true)
            {
                var selectedIds = formDto.SelectedMemberIds;

                // Remove members that are no longer selected
                var membersToRemove = entity.ProjectMembers
                    .Where(pm => !selectedIds.Contains(pm.UserId))
                    .ToList();

                foreach (var member in membersToRemove)
                {
                    entity.ProjectMembers.Remove(member);
                }

                // Add new members that are not already present
                foreach (var userId in selectedIds)
                {
                    if (entity.ProjectMembers.All(pm => pm.UserId != userId))
                    {
                        entity.ProjectMembers.Add(new ProjectMemberEntity
                        {
                            ProjectId = entity.Id,
                            UserId = userId,
                            RoleId = "1",
                            JoinedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            //  Save changes and update cache
            var result = await _projectRepository.UpdateAsync(entity);
            if (!result.Succeeded)
                return new ProjectResult<ProjectDetailsDto> { Succeeded = false, StatusCode = 500, Error = result.Error };

            await UpdateCacheAsync();
            var detailsDto = ProjectMapper.ToDetailsDto(entity);
            return new ProjectResult<ProjectDetailsDto> { Succeeded = true, StatusCode = 200, Result = detailsDto };
        }
        catch (Exception ex)
        { return new ProjectResult<ProjectDetailsDto> { Succeeded = false, StatusCode = 500, Error = $"Failed to update project: {ex.Message}" }; }
    }
    
    /// Helper method to remove a project member
    public async Task<ProjectResult> RemoveProjectMemberAsync(string projectId, string userId)
    {
        var repoResult = await _projectRepository.RemoveProjectMemberAsync(projectId, userId);
        if (repoResult.Succeeded)
            await UpdateCacheAsync();
        
        return new ProjectResult
        { Succeeded = repoResult.Succeeded, StatusCode = repoResult.StatusCode, Error = repoResult.Error };
    }
    
    public async Task<ProjectResult> RemoveUserFromAllProjectsAsync(string userId)
    {
        var repoResult = await _projectRepository.RemoveAllProjectMembershipsForUserAsync(userId);
        if (repoResult.Succeeded)
            await UpdateCacheAsync();
        
        return new ProjectResult
        { Succeeded = repoResult.Succeeded, StatusCode = repoResult.StatusCode, Error = repoResult.Error };
    }

    public async Task<ProjectResult<ProjectDetailsDto>> DeleteProjectAsync(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return new ProjectResult<ProjectDetailsDto> { Succeeded = false, StatusCode = 400, Error = "Project ID is required" };
            
            var project = await _projectRepository.GetAsync(x => x.Id == id);
            if (!project.Succeeded || project.Result == null)
                return new ProjectResult<ProjectDetailsDto> { Succeeded = false, StatusCode = 404, Error = "Project not found." };

            var result = await _projectRepository.DeleteAsync(x => x.Id == id);
            var dto = ProjectMapper.ToDetailsDto(project.Result);

            await UpdateCacheAsync();
            return new ProjectResult<ProjectDetailsDto> { Succeeded = result.Succeeded, StatusCode = result.StatusCode, Error = result.Error, Result = dto};
        }
        catch (Exception ex)
        { return new ProjectResult<ProjectDetailsDto> { Succeeded = false, StatusCode = 500, Error = $"Failed to delete project: {ex.Message}" }; }
    }

    // Updates the project cache
    // Not using the return value for now, but can be used to update the UI or for ex. dashboard statistics
    private async Task<IEnumerable<ProjectDetailsDto>> UpdateCacheAsync()
    {
        var result = await _projectRepository.GetAllAsync
        (
            orderByDescending: true,
            sortByColumn: x => x.CreatedAt,
            includes:
            [
                x => x.Client,
                x => x.ProjectMembers,
                x => x.Status,
                x => x.User
            ]
        );

        if (!result.Succeeded)
            return [];

        var entities = result.Result;
        var projects = entities?.Select(ProjectMapper.ToDetailsDto).ToList() ?? [];
        cacheHandler.SetCache(_cacheKey, projects);
        return projects;
    }

    public async Task<ProjectResult<ProjectDetailsDto>> GetProjectDetailsAsync(string id)
    {
        // logger.LogInformation($"GetProjectDetailsAsync called with id: {id}");
        try
        {
            var result = await _projectRepository.GetAsync(
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
        { return new ProjectResult<ProjectDetailsDto> { Succeeded = false, Error = ex.Message }; }
    }
}