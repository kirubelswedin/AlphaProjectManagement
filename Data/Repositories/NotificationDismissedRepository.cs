using Data.Contexts;
using Data.Entities;
using Domain.Responses;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public interface INotificationDismissedRepository : IBaseRepository<NotificationDismissedEntity>
{
    Task<RepositoryResult<IEnumerable<string>>> GetNotificationsIdsAsync(string userId);
}

public class NotificationDismissedRepository(AppDbContext context) : BaseRepository<NotificationDismissedEntity>(context), INotificationDismissedRepository
{
    // Get IDs of notifications dismissed by a specific user.
    public async Task<RepositoryResult<IEnumerable<string>>> GetNotificationsIdsAsync(string userId)
    {
        var ids = await _table.Where(x => x.UserId == userId).Select(x => x.NotificationId).ToListAsync();
        
        return new RepositoryResult<IEnumerable<string>> { Succeeded = true, StatusCode = 200, Result = ids };
    }
}


