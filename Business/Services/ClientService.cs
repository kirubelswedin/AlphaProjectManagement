using Business.Dtos.Forms;
using Business.Interfaces;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;

namespace Business.Services;

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

    public async Task<ClientResult> CreateClientAsync(ClientFormData formData)
    {
        try
        {
            var client = new Client
            {
                Image = formData.Image,
                ClientName = formData.ClientName,
                ContactPerson = formData.ContactPerson,
                Email = formData.Email,
                Phone = formData.Phone
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

    public async Task<ClientResult> UpdateClientAsync(string id, ClientFormData formData)
    {
        try
        {
            var existingClient = await _clientRepository.GetAsync(x => x.Id == id);
            if (!existingClient.Succeeded || existingClient.Result == null)
                return new ClientResult { Succeeded = false, StatusCode = 404, Error = $"Client with id '{id}' was not found." };

            var entity = existingClient.Result;
            entity.Image = formData.Image;
            entity.ClientName = formData.ClientName;
            entity.ContactPerson = formData.ContactPerson;
            entity.Email = formData.Email;
            entity.Phone = formData.Phone;
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