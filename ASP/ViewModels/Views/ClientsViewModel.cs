using ASP.ViewModels.Components;
using ASP.ViewModels.forms;
using Domain.Models;

namespace ASP.ViewModels.Views;

public class ClientsViewModel
{
    public PageHeaderViewModel PageHeader { get; set; } = new()
    {
        Title = "Client",
        ButtonText = "Add Client",
        ModalId = "addClientModal"
    };

    public AddClientsViewModel AddClient { get; set; } = new();

    public IEnumerable<Client> Clients { get; set; } = [];
}
