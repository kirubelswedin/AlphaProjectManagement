using System.Linq.Expressions;
using Data.Contexts;
using Data.Entities;
using Domain.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Data.Repositories;

public interface IUserRepository : IBaseRepository<UserEntity>
{
}

public class UserRepository(AppDbContext context) : BaseRepository<UserEntity>(context), IUserRepository
{
  public override async Task<RepositoryResult<IEnumerable<UserEntity>>> GetAllAsync(bool orderByDescending = false, Expression<Func<UserEntity, object>>? sortByColumn = null, Expression<Func<UserEntity, bool>>? filterBy = null, int take = 0, params Expression<Func<UserEntity, object>>[] includes)
  {
    var query = _table.Include(x => x.Address);

    if (filterBy != null)
      query = (IIncludableQueryable<UserEntity, UserAddressEntity?>)query.Where(filterBy);

    if (sortByColumn != null)
      query = (IIncludableQueryable<UserEntity, UserAddressEntity?>)(orderByDescending ? query.OrderByDescending(sortByColumn) : query.OrderBy(sortByColumn));

    if (take > 0)
      query = (IIncludableQueryable<UserEntity, UserAddressEntity?>)query.Take(take);

    var entities = await query.ToListAsync();

    return new RepositoryResult<IEnumerable<UserEntity>>
    {
      Succeeded = true,
      StatusCode = 200,
      Result = entities
    };
  }

  public override async Task<RepositoryResult<UserEntity>> GetAsync(Expression<Func<UserEntity, bool>> findBy, params Expression<Func<UserEntity, object>>[] includes)
  {
    var query = _table.Include(x => x.Address);
    var entity = await query.FirstOrDefaultAsync(findBy);

    return new RepositoryResult<UserEntity>
    {
      Succeeded = true,
      StatusCode = 200,
      Result = entity
    };
  }
}