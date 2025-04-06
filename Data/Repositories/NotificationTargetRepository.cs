using Data.Contexts;
using Data.Entities;

namespace Data.Repositories;

public interface INotificationTargetRepository : IBaseRepository<NotificationTargetEntity>
{
}

public class NotificationTargetRepository(AppDbContext context) : BaseRepository<NotificationTargetEntity>(context), INotificationTargetRepository
{
}

