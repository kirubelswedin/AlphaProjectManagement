using Business.Dtos;
using Business.Mappers;
using Data.Repositories;
using Domain.Models;
using Domain.Responses;

namespace Business.Services;

public interface IClientService
{
    Task<ClientResult<IEnumerable<Client>>> GetClientsAsync();
    Task<ClientResult<Client>> GetClientByIdAsync(string id);
    Task<ClientResult> CreateClientAsync(AddClientFormDto formDto);
    Task<ClientResult> UpdateClientAsync(string id, AddClientFormDto formDto);
    Task<ClientResult> DeleteClientAsync(string id);
}


public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<ClientResult<IEnumerable<Client>>> GetClientsAsync()
    {
        var result = await _clientRepository.GetAllAsync();
        if (!result.Succeeded)
            return new ClientResult<IEnumerable<Client>> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        var clients = result.Result!.Select(ClientMapper.ToModel);
        return new ClientResult<IEnumerable<Client>> { Succeeded = true, StatusCode = 200, Result = clients };
    }

    public async Task<ClientResult<Client>> GetClientByIdAsync(string id)
    {
        var result = await _clientRepository.GetAsync(x => x.Id == id);
        if (!result.Succeeded)
            return new ClientResult<Client> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        var client = ClientMapper.ToModel(result.Result);
        return new ClientResult<Client> { Succeeded = true, StatusCode = 200, Result = client };
    }

    public async Task<ClientResult> CreateClientAsync(AddClientFormDto formDto)
    {
        var entity = ClientMapper.ToEntity(formDto);
        var result = await _clientRepository.AddAsync(entity);
        return new ClientResult { Succeeded = result.Succeeded, StatusCode = result.StatusCode, Error = result.Error };
    }

    public async Task<ClientResult> UpdateClientAsync(string id, AddClientFormDto formDto)
    {
        var existingClient = await _clientRepository.GetAsync(x => x.Id == id);
        if (!existingClient.Succeeded)
            return new ClientResult { Succeeded = false, StatusCode = existingClient.StatusCode, Error = existingClient.Error };

        var entity = ClientMapper.ToEntity(formDto);
        entity.Id = id;
        var result = await _clientRepository.UpdateAsync(entity);
        return new ClientResult { Succeeded = result.Succeeded, StatusCode = result.StatusCode, Error = result.Error };
    }

    public async Task<ClientResult> DeleteClientAsync(string id)
    {
        var result = await _clientRepository.DeleteAsync(x => x.Id == id);
        return new ClientResult { Succeeded = result.Succeeded, StatusCode = result.StatusCode, Error = result.Error };
    }
}