using Data.Contexts;
using Data.Entities;

namespace Data.Repositories;

public interface IUserRepository : IBaseRepository<UserEntity>
{
}

public class UserRepository(AppDbContext context) : BaseRepository<UserEntity>(context), IUserRepository
{
}