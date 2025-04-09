using System.Linq.Expressions;
using Data.Contexts;
using Data.Entities;
using Domain.Responses;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public interface IProjectRepository : IBaseRepository<ProjectEntity>
{
}

public class ProjectRepository(AppDbContext context) : BaseRepository<ProjectEntity>(context), IProjectRepository
{
    // public override async Task<IEnumerable<ProjectEntity>> GetAllAsync()
    // {
    //     var entities = await _table
    //         .Include(x => x.User)
    //         .Include(x => x.Status)
    //         .Include(x => x.Client)
    //         .ToListAsync();
    //
    //     return entities;
    // }
    //
    //
    // public override async Task<RepositoryResult<ProjectEntity>> GetAsync(Expression<Func<ProjectEntity, bool>> findBy, params Expression<Func<ProjectEntity, object>>[] includes)
    // {
    //     var entity = await _table
    //         .Include(x => x.User)
    //         .Include(x => x.Status)
    //         .Include(x => x.Client)
    //         .FirstOrDefaultAsync(findBy);
    //
    //     return entity;
    // }
}