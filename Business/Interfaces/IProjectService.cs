using Business.Dtos.Forms;
using Domain.Models;
using Domain.Responses;

namespace Business.Interfaces;

public interface IProjectService
{
    Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync();
    Task<ProjectResult<Project>> GetProjectByIdAsync(string id);
    Task<ProjectResult> CreateProjectAsync(ProjectFormData formData, string createdById);
    Task<ProjectResult> UpdateProjectAsync(string id, ProjectFormData formData);
    Task<ProjectResult> DeleteProjectAsync(string id);
}