using Data.Contexts;
using Data.Entities;
using Domain.Responses;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

public interface IProjectRepository : IBaseRepository<ProjectEntity>
{
    Task<RepositoryResult> AddProjectMemberAsync(string projectId, string userId, string roleId = "default");
    Task<RepositoryResult<IEnumerable<ProjectMemberEntity>>> GetProjectMembersAsync(string projectId);
    Task<RepositoryResult> RemoveProjectMemberAsync(string projectId, string userId);
    Task<RepositoryResult> RemoveAllProjectMembershipsForUserAsync(string userId);
}

public class ProjectRepository(AppDbContext context) : BaseRepository<ProjectEntity>(context), IProjectRepository
{
    // Get all, including all related information
    // took some help from chatGPT to get this to work as I wanted
    public override async Task<RepositoryResult<IEnumerable<ProjectEntity>>> GetAllAsync(bool orderByDescending = false, Expression<Func<ProjectEntity, object>>? sortByColumn = null, Expression<Func<ProjectEntity, bool>>? filterBy = null, int take = 0, params Expression<Func<ProjectEntity, object>>[] includes)
    {
        var query = _table
            .Include(x => x.User)
            .Include(x => x.Status)
            .Include(x => x.Client)
            .Include(x => x.ProjectMembers)
                .ThenInclude(pm => pm.User)
            .Include(x => x.ProjectMembers)
                .ThenInclude(pm => pm.Role)
            .AsQueryable();

        if (filterBy != null)
            query = query.Where(filterBy);

        if (sortByColumn != null)
            query = orderByDescending ? query.OrderByDescending(sortByColumn) : query.OrderBy(sortByColumn);

        if (take > 0)
            query = query.Take(take);

        var entities = await query.ToListAsync();

        return new RepositoryResult<IEnumerable<ProjectEntity>> { Succeeded = true, StatusCode = 200, Result = entities };
    }
    
    public override async Task<RepositoryResult<ProjectEntity>> GetAsync(Expression<Func<ProjectEntity, bool>>? findBy, params Expression<Func<ProjectEntity, object>>[]? includes)
    {
        if (findBy == null) throw new ArgumentNullException(nameof(findBy));
        var query = _table
            .Include(x => x.User)
            .Include(x => x.Status)
            .Include(x => x.Client)
            .Include(x => x.ProjectMembers)
                .ThenInclude(pm => pm.User)
            .Include(x => x.ProjectMembers)
                .ThenInclude(pm => pm.Role)
            .AsQueryable();

        var entity = await query.FirstOrDefaultAsync(findBy);

        return new RepositoryResult<ProjectEntity> { Succeeded = true, StatusCode = 200, Result = entity };
    }


    // Add a user to a project, making sure both exist and the user isn’t already a member.
    // took some help from ChatGPT to get this right.
    public async Task<RepositoryResult> AddProjectMemberAsync(string projectId, string userId, string roleId = "default")
    {
        try
        {
            // Console.WriteLine($"AddProjectMemberAsync: projectId={projectId}, userId={userId}, roleId={roleId}");
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
                return new RepositoryResult { Succeeded = false, StatusCode = 404, Error = "Project not found" };
            
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return new RepositoryResult { Succeeded = false, StatusCode = 404, Error = "User not found" };
            
            if (roleId != "default")
            {
                var roleExists = await _context.ProjectRoles.AnyAsync(r => r.Id == roleId);
                if (!roleExists)
                    return new RepositoryResult { Succeeded = false, StatusCode = 404, Error = "Project role not found" };
            }
            
            if (roleId == "default")
            {
                var defaultRole = await _context.ProjectRoles.FirstOrDefaultAsync(r => r.IsDefault);
                roleId = defaultRole?.Id ?? throw new Exception("No default project role found");
            }

            // check if the user is already a member of the project
            var existingMember = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (existingMember != null)
                return new RepositoryResult { Succeeded = false, StatusCode = 400, Error = "User is already a member of this project" };

            // add project member
            var projectMember = new ProjectMemberEntity
            {
                ProjectId = projectId,
                UserId = userId,
                RoleId = roleId,
                JoinedAt = DateTime.UtcNow
            };

            await _context.ProjectMembers.AddAsync(projectMember);
            await _context.SaveChangesAsync();

            return new RepositoryResult { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"Exception in AddProjectMemberAsync: {ex.Message}");
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Error = $"Failed to add project member: {ex.Message}" };
        }
    }

    // Get project members for a specific project, including their roles
    public async Task<RepositoryResult<IEnumerable<ProjectMemberEntity>>> GetProjectMembersAsync(string projectId)
    {
        try
        {
            var members = await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .Include(pm => pm.User)
                .Include(pm => pm.Role)
                .ToListAsync();

            return new RepositoryResult<IEnumerable<ProjectMemberEntity>> { Succeeded = true, StatusCode = 200, Result = members };
        }
        catch (Exception ex)
        { return new RepositoryResult<IEnumerable<ProjectMemberEntity>> { Succeeded = false, StatusCode = 500, Error = $"Failed to get project members: {ex.Message}" }; }
    }
    
    // Remove user from a project if they're actually a member.
    public async Task<RepositoryResult> RemoveProjectMemberAsync(string projectId, string userId)
    {
        try
        {
            var member = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (member == null)
                return new RepositoryResult { Succeeded = false, StatusCode = 404, Error = "Project member not found" };

            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();

            return new RepositoryResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        { return new RepositoryResult { Succeeded = false, StatusCode = 500, Error = $"Failed to remove project member: {ex.Message}" }; }
    }
    
    // Remove a user from all projects at once.
    public async Task<RepositoryResult> RemoveAllProjectMembershipsForUserAsync(string userId)
    {
        try
        {
            var memberships = await _context.ProjectMembers
                .Where(pm => pm.UserId == userId)
                .ToListAsync();

            if (memberships.Any())
            {
                _context.ProjectMembers.RemoveRange(memberships);
                await _context.SaveChangesAsync();
            }

            return new RepositoryResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        { return new RepositoryResult { Succeeded = false, StatusCode = 500, Error = $"Failed to remove project memberships for user: {ex.Message}" }; }
    }
}
