using Business.Dtos.Forms;
using Domain.Models;
using Domain.Responses;

namespace Business.Interfaces;

public interface IClientService
{
    Task<ClientResult<IEnumerable<Client>>> GetClientsAsync();
    Task<ClientResult<Client>> GetClientByIdAsync(string id);
    Task<ClientResult> CreateClientAsync(ClientFormData formData);
    Task<ClientResult> UpdateClientAsync(string id, ClientFormData formData);
    Task<ClientResult> DeleteClientAsync(string id);
}