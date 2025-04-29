using ASP.ViewModels.Components;
using ASP.ViewModels.Views;
using Business.Dtos;
using Business.Services;
using ASP.Extensions;
using ASP.Mappers;
using ASP.ViewModels.Forms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Controllers;

[Authorize(Roles = "Admin")]
public class ClientsController(IClientService clientService, INotificationService notificationService)
    : Controller
{
    private readonly IClientService _clientService = clientService;
    private readonly INotificationService _notificationService = notificationService;

    [Route("admin/clients")]
    public async Task<IActionResult> Index()
    {
        var viewModel = new ClientsViewModel
        {
            PageHeader = CreatePageHeader(),
            Clients = await GetClientsAsync(),
            AddClient = new AddClientViewModel(),
            EditClient = new EditClientViewModel(),
        };

        return View(viewModel);
    }

    private static PageHeaderViewModel CreatePageHeader() => new()
    {
        Title = "Clients",
        ButtonText = "Add Client",
        ModalId = "addclientmodal"
    };

    [HttpPost]
    public async Task<IActionResult> AddClient([FromForm] AddClientViewModel model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetModelErrors() });

        var dto = ClientViewModelMapper.ToAddClientFormDto(model);

        try
        {
            var result = await _clientService.CreateClientAsync(dto);
            if (!result.Succeeded)
                return Json(new { success = false, error = result.Error ?? "Failed to create client" });
            
            var clientViewModel = ClientViewModelMapper.ToClientListViewModel(result.Result!);
            await CreateClientNotification(clientViewModel.ImageUrl, "Client Added", $"Client {dto.ClientName} has been added.");
            return Json(new { success = true, client = clientViewModel });
        }
        catch (Exception ex)
        { return Json(new { success = false, error = ex.Message }); }
    }
    
    [HttpGet("clients/{id}")]
    public async Task<IActionResult> GetClient(string id)
    {
        var result = await _clientService.GetClientByIdAsync(id);
        if (!result.Succeeded || result.Result == null)
            return Json(new { success = false, error = result.Error });

        var clientViewModel = ClientViewModelMapper.ToClientListViewModel(result.Result!);
        return Json(new { success = true, client = clientViewModel });
    }
    
    private async Task<IEnumerable<ClientListViewModel>> GetClientsAsync()
    {
        var result = await _clientService.GetClientsAsync();
        if (!result.Succeeded) return [];
        
        var clients = result.Result!.ToList();
        var clientList = new List<ClientListViewModel>();

        foreach (var client in clients)
        {
            Console.WriteLine($"Client: {client.ClientName}, ImageUrl: {client.ImageUrl}");
            var clientViewModel = ClientViewModelMapper.ToClientListViewModel(client);
            clientList.Add(clientViewModel);
        }

        return clientList;
    }

    [HttpPost]
    public async Task<IActionResult> EditClient([FromForm] EditClientViewModel model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, errors = GetModelErrors() });

        var dto = ClientViewModelMapper.ToUpdateClientFormDto(model);
        try
        {
            var result = await _clientService.UpdateClientAsync(dto);
            if (!result.Succeeded)
                return Json(new { success = false, error = result.Error ?? "Failed to update client" });

            var clientViewModel = ClientViewModelMapper.ToClientListViewModel(result.Result!);
            await CreateClientNotification(clientViewModel.ImageUrl, "Client Updated", $"Client {dto.ClientName} has been updated.");
            return Json(new { success = true, client = clientViewModel });
        }
        catch (Exception ex)
        { return Json(new { success = false, error = ex.Message }); }
    }

    [HttpDelete("clients/{id}")]
    public async Task<IActionResult> DeleteClient(string id)
    {
        var client = await _clientService.GetClientByIdAsync(id);
        if (!client.Succeeded)
            return Json(new { success = false, error = client.Error });

        var result = await _clientService.DeleteClientAsync(id);
        if (!result.Succeeded)
            return Json(new { success = false, error = result.Error });

        var clientViewModel = ClientViewModelMapper.ToClientListViewModel(client.Result!);
        await CreateClientNotification(clientViewModel.ImageUrl, "Client Deleted", $"Client {client.Result!.ClientName} has been deleted");
        return Json(new { success = true, client = clientViewModel });
    }

    [HttpGet("clients/search")]
    public async Task<IActionResult> SearchClients(string term)
    {
        var result = await _clientService.GetClientsAsync();
        if (!result.Succeeded)
        {
            return Json(new List<object>());
        }

        var filteredClients = result.Result!
            .Where(c => c.ClientName != null && c.ClientName.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(c => new
            {
                id = c.Id,
                text = c.ClientName,
                image = (c.ImageUrl ?? "default-client.svg").GetImageUrl("clients")
            })
            .ToList();

        return Json(filteredClients);
    }

    private async Task CreateClientNotification(string? imageUrl, string title, string message)
    {
        var notificationDto = new NotificationDetailsDto
        {
            NotificationTypeId = 3, // Client type
            NotificationTargetId = 2, // Admins
            Title = title,
            Message = message,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationService.AddNotificationAsync(notificationDto);
    }
    
    private Dictionary<string, string[]> GetModelErrors()
    {
        return ModelState
            .Where(x => x.Value!.Errors.Any())
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );
    }
}