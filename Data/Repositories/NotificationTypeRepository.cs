using Data.Contexts;
using Data.Entities;

namespace Data.Repositories;

public interface INotificationTypeRepository : IBaseRepository<NotificationTypeEntity>
{
}

public class NotificationTypeRepository(AppDbContext context) : BaseRepository<NotificationTypeEntity>(context), INotificationTypeRepository
{
    
}

