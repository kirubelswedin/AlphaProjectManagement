using Business.Factories;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;

namespace Business.Services;

public interface IStatusService
{
    Task<StatusResult<Status>> GetStatusByIdAsync(string id);
    Task<StatusResult<Status>> GetStatusByNameAsync(string statusName);
    Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync();
}


public class StatusService(IStatusRepository statusRepository, IStatusFactory  statusFactory) : IStatusService
{
    private readonly IStatusRepository _statusRepository = statusRepository;
    private readonly IStatusFactory _statusFactory = statusFactory;

    public async Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync()
    {
        var result = await _statusRepository.GetAllAsync
        (
            orderByDescending: false,
            sortByColumn: x => x.Id
        );

        var entities = result.Result;
        var statuses = entities?.Select(entity => entity.MapTo<Status>()) ?? [];

        return new StatusResult<IEnumerable<Status>> { Succeeded = true, StatusCode = 200, Result = statuses };
    }

    public async Task<StatusResult<Status>> GetStatusByIdAsync(string id)
    {
        var result = await _statusRepository.GetAsync(x => x.Id == id);

        var entity = result.Result;
        if (entity == null)
            return new StatusResult<Status> { Succeeded = false, StatusCode = 404, Error = $"Status with id '{id}' was not found." };

        var status = entity.MapTo<Status>();
        return new StatusResult<Status> { Succeeded = true, StatusCode = 200, Result = status };
    }
    
    public async Task<StatusResult<Status>> GetStatusByNameAsync(string statusName)
    {
        var result = await _statusRepository.GetAsync(x => x.StatusName == statusName);

        var entity = result.Result;
        if (entity == null)
            return new StatusResult<Status> { Succeeded = false, StatusCode = 404, Error = $"Status with name '{statusName}' was not found." };

        var status = entity.MapTo<Status>();
        return new StatusResult<Status> { Succeeded = true, StatusCode = 200, Result = status };
       
    }
}