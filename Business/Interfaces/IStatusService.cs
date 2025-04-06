using Domain.Models;
using Domain.Responses;

namespace Business.Interfaces;

public interface IStatusService
{
    Task<StatusResult<Status>> GetStatusByIdAsync(int id);
    Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync();
}
