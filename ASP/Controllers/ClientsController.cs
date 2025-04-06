using ASP.ViewModels.forms;
using ASP.ViewModels.MockData;
using ASP.ViewModels.Views;
using Business.Dtos.Forms;
using Business.Interfaces;
using Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Controllers;

[Authorize(Roles = "Admin")]
public class ClientsController(IClientService clientService) : Controller
{
    private readonly IClientService _clientService = clientService;

    [Route("admin/clients")]
    public IActionResult Index()
    {
        var viewModel = new ClientsViewModel
        {
            PageHeader = new()
            {
                Title = "Client",
                ButtonText = "Add Client",
                ModalId = "addClientModal"
            },
            Clients = ClientsMockData.GetClients()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddClient(ClientsFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            return Json(new { success = false, errors });
        }

        // Map from ViewModel to FormData (DTO)
        var formData = model.MapTo<ClientFormData>();
        var result = await _clientService.CreateClientAsync(formData);

        if (result.Succeeded)
        {
            return Json(new { success = true, message = "Client created successfully" });
        }

        return Json(new { success = false, message = result.Error });
    }

    [HttpGet("clients/list")]
    public IActionResult GetClients()
    {
        var clients = ClientsMockData.GetClients();
        return Json(new { success = true, clients });
    }

    [HttpGet("clients/{id}")]
    public IActionResult GetClient(string id)
    {
        var client = ClientsMockData.GetClientById(id);
        if (client == null)
        {
            return NotFound();
        }

        return Json(new { success = true, client });
    }

    [HttpPut("clients/{id}")]
    public async Task<IActionResult> UpdateClient(string id, ClientsFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            return Json(new { success = false, errors });
        }

        var formData = model.MapTo<ClientFormData>();
        var result = await _clientService.UpdateClientAsync(id, formData);

        if (result.Succeeded)
        {
            return Json(new { success = true, message = "Client updated successfully" });
        }

        return Json(new { success = false, message = result.Error });
    }

    [HttpDelete("clients/{id}")]
    public async Task<IActionResult> DeleteClient(string id)
    {
        var result = await _clientService.DeleteClientAsync(id);
        if (result.Succeeded)
        {
            return Json(new { success = true, message = "Client deleted successfully" });
        }

        return Json(new { success = false, message = result.Error });
    }
}