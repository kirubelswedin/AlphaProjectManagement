using Business.Dtos;
using Business.Interfaces;
using Data.Repositories;
using Domain.Extensions;
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
        var repositoryResult = await _clientRepository.GetAllAsync
        (
            orderByDescending: false,
            sortByColumn: x => x.ClientName
        );

        var entities = repositoryResult.Result;
        var clients = entities?.Select(entity => entity.MapTo<Client>()) ?? [];

        return new ClientResult<IEnumerable<Client>> { Succeeded = true, StatusCode = 200, Result = clients };
    }

    public async Task<ClientResult<Client>> GetClientByIdAsync(string id)
    {
        var repositoryResult = await _clientRepository.GetAsync(x => x.Id == id);

        var entity = repositoryResult.Result;
        if (entity == null)
            return new ClientResult<Client> { Succeeded = false, StatusCode = 404, Error = $"Client with id '{id}' was not found." };

        var client = entity.MapTo<Client>();
        return new ClientResult<Client> { Succeeded = true, StatusCode = 200, Result = client };
    }
    
    public async Task<ClientResult> CreateClientAsync(AddClientFormDto formDto)
    {
        try
        {
            var client = new Client
            {
                ImageUrl = formDto.ImageUrl,
                ClientName = formDto.ClientName,
                ContactPerson = formDto.ContactPerson,
                Email = formDto.Email,
                Phone = formDto.PhoneNumber
            };

            var entity = client.MapTo<Data.Entities.ClientEntity>();
            var result = await _clientRepository.AddAsync(entity);

            return new ClientResult { Succeeded = result.Succeeded, StatusCode = result.Succeeded ? 201 : 500, Error = result.Error };
        }
        catch (Exception ex)
        {
            return new ClientResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    public async Task<ClientResult> UpdateClientAsync(string id, AddClientFormDto formDto)
    {
        try
        {
            var existingClient = await _clientRepository.GetAsync(x => x.Id == id);
            if (!existingClient.Succeeded || existingClient.Result == null)
                return new ClientResult { Succeeded = false, StatusCode = 404, Error = $"Client with id '{id}' was not found." };

            var entity = existingClient.Result;
            entity.ImageUrl = formDto.ImageUrl;
            entity.ClientName = formDto.ClientName;
            entity.ContactPerson = formDto.ContactPerson;
            entity.Email = formDto.Email;
            entity.Phone = formDto.PhoneNumber;
            entity.UpdatedAt = DateTime.UtcNow;

            var result = await _clientRepository.UpdateAsync(entity);
            return new ClientResult { Succeeded = result.Succeeded, StatusCode = result.Succeeded ? 200 : 500, Error = result.Error };
        }
        catch (Exception ex)
        {
            return new ClientResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    public async Task<ClientResult> DeleteClientAsync(string id)
    {
        var result = await _clientRepository.DeleteAsync(x => x.Id == id);
        return new ClientResult { Succeeded = result.Succeeded, StatusCode = result.Succeeded ? 200 : 500, Error = result.Error };
    }
}