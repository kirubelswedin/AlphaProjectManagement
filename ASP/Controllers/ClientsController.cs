using ASP.ViewModels.Components;
using ASP.ViewModels.Views;
using Business.Dtos;
using Business.Services;
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
        var clients = await _clientService.GetClientsAsync();
        var viewModel = new ClientsViewModel
        {
            PageHeader = new PageHeaderViewModel
            {
                Title = "Clients",
                ButtonText = "Add Client",
                ModalId = "addclientmodal"
            },
            Clients = clients.Result ?? []
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddClient(AddClientFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        // Set server-side timestamps
        dto.CreatedAt = DateTime.UtcNow;
        dto.UpdatedAt = DateTime.UtcNow;

        var result = await _clientService.CreateClientAsync(dto);
        if (!result.Succeeded)
        {
            return Json(new { success = false, error = result.Error });
        }

        await CreateClientNotification("New Client Added", $"Client {dto.ClientName} has been added successfully.");
        return Json(new { success = true });
    }

    [HttpGet("clients/list")]
    public async Task<IActionResult> GetClients()
    {
        var result = await _clientService.GetClientsAsync();
        if (!result.Succeeded)
        {
            return Json(new { success = false, error = result.Error });
        }

        return Json(new { success = true, clients = result.Result });
    }

    [HttpGet("clients/{id}")]
    public async Task<IActionResult> GetClient(string id)
    {
        var result = await _clientService.GetClientByIdAsync(id);
        if (!result.Succeeded || result.Result == null)
        {
            return NotFound();
        }

        return Json(new { success = true, client = result.Result });
    }

    [HttpPut("clients/{id}")]
    public async Task<IActionResult> UpdateClient(string id, UpdateClientFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        var result = await _clientService.UpdateClientAsync(id, dto);
        if (!result.Succeeded)
        {
            return Json(new { success = false, error = result.Error });
        }

        await CreateClientNotification("Client Updated", $"Client {dto.ClientName} has been updated successfully.");
        return Json(new { success = true });
    }

    [HttpDelete("clients/{id}")]
    public async Task<IActionResult> DeleteClient(string id)
    {
        var result = await _clientService.DeleteClientAsync(id);
        if (!result.Succeeded)
        {
            return Json(new { success = false, error = result.Error });
        }

        await CreateClientNotification("Client Deleted", "A client has been deleted");
        return Json(new { success = true });
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
            .Where(c => c.ClientName.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(c => new
            {
                id = c.Id,
                text = c.ClientName,
                image = c.ImageUrl
            })
            .ToList();

        return Json(filteredClients);
    }

    private async Task CreateClientNotification(string title, string message)
    {
        var notificationDto = new NotificationDetailsDto
        {
            NotificationTypeId = 3, // Client type
            NotificationTargetId = 2, // Admins
            Title = title,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationService.AddNotificationAsync(notificationDto);
    }
}