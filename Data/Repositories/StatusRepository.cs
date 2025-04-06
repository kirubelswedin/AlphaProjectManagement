using Data.Contexts;
using Data.Entities;

namespace Data.Repositories;

public interface IStatusRepository : IBaseRepository<StatusEntity>
{
}

public class StatusRepository(AppDbContext context) : BaseRepository<StatusEntity>(context), IStatusRepository
{
}