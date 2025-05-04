using Business.Mappers;
using Data.Repositories;
using Domain.Models;
using Domain.Responses;

namespace Business.Services;

public interface IStatusService
{

    Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync();
    Task<StatusResult<Status>> GetStatusByIdAsync(int id);
    Task<StatusResult<Status>> GetStatusByNameAsync(string statusName);

}

public class StatusService(IStatusRepository statusRepository) : IStatusService
{
    private readonly IStatusRepository _statusRepository = statusRepository;

    public async Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync()
    {
        var result = await _statusRepository.GetAllAsync();
        if (!result.Succeeded)
            return new StatusResult<IEnumerable<Status>> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        var statuses = result.Result!.Select(StatusMapper.ToModel);
        return new StatusResult<IEnumerable<Status>> { Succeeded = true, StatusCode = 200, Result = statuses };
    }

    public async Task<StatusResult<Status>> GetStatusByIdAsync(int id)
    {
        var result = await _statusRepository.GetAsync(x => x.Id == id);
        if (!result.Succeeded)
            return new StatusResult<Status> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        var status = StatusMapper.ToModel(result.Result);
        return new StatusResult<Status> { Succeeded = true, StatusCode = 200, Result = status };
    }

    public async Task<StatusResult<Status>> GetStatusByNameAsync(string statusName)
    {
        var result = await _statusRepository.GetAsync(x => x.StatusName == statusName);
        if (!result.Succeeded)
            return new StatusResult<Status> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        var status = StatusMapper.ToModel(result.Result);
        return new StatusResult<Status> { Succeeded = true, StatusCode = 200, Result = status };
    }
}