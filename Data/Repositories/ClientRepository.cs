using Data.Contexts;
using Data.Entities;

namespace Data.Repositories;

public interface IClientRepository : IBaseRepository<ClientEntity>
{
}

public class ClientRepository(AppDbContext context) : BaseRepository<ClientEntity>(context), IClientRepository
{
}