using Business.Dtos;
using Business.Handlers;
using Business.Mappers;
using Data.Repositories;
using Domain.Responses;

namespace Business.Services;

public interface IClientService
{
    Task<ClientResult<ClientDetailsDto>>  CreateClientAsync(AddClientFormDto formDto);
    Task<ClientResult<IEnumerable<ClientDetailsDto>>> GetClientsAsync();
    Task<ClientResult<ClientDetailsDto>> GetClientByIdAsync(string id);
    Task<ClientResult<ClientDetailsDto>>  UpdateClientAsync(UpdateClientFormDto formDto);
    Task<ClientResult<ClientDetailsDto>> DeleteClientAsync(string id);
}


public class ClientService(IClientRepository clientRepository, IImageHandler imageHandler, ICacheHandler<IEnumerable<ClientDetailsDto>> cacheHandler ) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IImageHandler _imageHandler = imageHandler;
    
    private readonly ICacheHandler<IEnumerable<ClientDetailsDto>> _cacheHandler = cacheHandler; 
    private const string _cacheKey = "Clients";

    public async Task<ClientResult<ClientDetailsDto>> CreateClientAsync(AddClientFormDto? formDto)
    {
        try
        {
            if (formDto == null)
                return new ClientResult<ClientDetailsDto> { Succeeded = false, StatusCode = 400, Error = "All required fields are not provided" };

            string? imageUrl = null;
            if (formDto.ImageFile != null)
                imageUrl = await _imageHandler.SaveImageAsync(formDto.ImageFile, "clients");

            var entity = ClientMapper.ToEntity(formDto, imageUrl);
            var result = await _clientRepository.AddAsync(entity);
            
            if (!result.Succeeded)
                return new ClientResult<ClientDetailsDto> { Succeeded = false, StatusCode = 500, Error = result.Error };
            
            await UpdateCacheAsync();
            
            var dto = ClientMapper.ToDetailsDto(entity);
            return new ClientResult<ClientDetailsDto> { Succeeded = result.Succeeded, StatusCode = result.StatusCode, Error = result.Error, Result = dto };
        }
        catch (Exception ex)
        { return new ClientResult<ClientDetailsDto> { Succeeded = false, StatusCode = 500, Error = $"Failed to create project: {ex.Message}" }; }
    }

    // TODO implement orderByDescending and cacheHandler
    public async Task<ClientResult<IEnumerable<ClientDetailsDto>>> GetClientsAsync()
    {
        var cachedClients = _cacheHandler.GetFromCache(_cacheKey);
        if (cachedClients != null)
            return new ClientResult<IEnumerable<ClientDetailsDto>> { Succeeded = true, StatusCode = 200, Result = cachedClients };
        
        var result = await _clientRepository.GetAllAsync();
        if (!result.Succeeded)
            return new ClientResult<IEnumerable<ClientDetailsDto>> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        var clients = result.Result!.Select(ClientMapper.ToDetailsDto).ToList();
        _cacheHandler.SetCache(_cacheKey, clients);
        return new ClientResult<IEnumerable<ClientDetailsDto>> { Succeeded = true, StatusCode = 200, Result = clients };
    }

    public async Task<ClientResult<ClientDetailsDto>> GetClientByIdAsync(string id)
    {
        var result = await _clientRepository.GetAsync(x => x.Id == id);
        if (!result.Succeeded)
            return new ClientResult<ClientDetailsDto> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        var client = ClientMapper.ToDetailsDto(result.Result);
        return new ClientResult<ClientDetailsDto> { Succeeded = true, StatusCode = 200, Result = client };
    }

    public async Task<ClientResult<ClientDetailsDto>>  UpdateClientAsync(UpdateClientFormDto formDto)
    {
        try
        {
            var clientResult = await _clientRepository.GetAsync(x => x.Id == formDto.Id);
            if (!clientResult.Succeeded || clientResult.Result == null)
                return new ClientResult<ClientDetailsDto> { Succeeded = false, StatusCode = 404, Error = "Client not found." };

            var existingClient = clientResult.Result;
            
            var imageUrl = existingClient!.ImageUrl;
            if (formDto.ImageFile != null)
                imageUrl = await _imageHandler.SaveImageAsync(formDto.ImageFile, "clients");

            ClientMapper.ApplyUpdatesToEntity(formDto, existingClient, imageUrl);
            
            var result = await _clientRepository.UpdateAsync(existingClient);
            if (!result.Succeeded)
                return new ClientResult<ClientDetailsDto> { Succeeded = false, StatusCode = 500, Error = result.Error };
            
            await UpdateCacheAsync();
            
            var dto = ClientMapper.ToDetailsDto(existingClient);
            return new ClientResult<ClientDetailsDto> { Succeeded = result.Succeeded, StatusCode = 200, Result = dto };
        }
        catch (Exception ex)
        { return new ClientResult<ClientDetailsDto> { Succeeded = false, StatusCode = 500, Error = $"Failed to update client: {ex.Message}" }; }
    }

    public async Task<ClientResult<ClientDetailsDto>> DeleteClientAsync(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return new ClientResult<ClientDetailsDto> { Succeeded = false, StatusCode = 400, Error = "Client ID is required." };
            
            var client = await _clientRepository.GetAsync(x => x.Id == id);
            if (!client.Succeeded)
                return new ClientResult<ClientDetailsDto> { Succeeded = false, StatusCode = client.StatusCode, Error = client.Error };

            var result = await _clientRepository.DeleteAsync(x => x.Id == id);
            var dto = ClientMapper.ToDetailsDto(client.Result);

            await UpdateCacheAsync();
            return new ClientResult<ClientDetailsDto> { Succeeded = result.Succeeded, StatusCode = result.StatusCode, Error = result.Error, Result = dto };
        }
        catch (Exception ex)
        { return new ClientResult<ClientDetailsDto> { Succeeded = false, StatusCode = 500, Error = $"Failed to delete client: {ex.Message}" }; }
    }
    
    private async Task<IEnumerable<ClientDetailsDto>> UpdateCacheAsync()
    {
        var result = await _clientRepository.GetAllAsync();
        if (!result.Succeeded)
            return [];
        
        var entities = result.Result!;
        var clients = entities.Select(ClientMapper.ToDetailsDto).ToList();
        _cacheHandler.SetCache(_cacheKey, clients);
        return clients;
    }
}