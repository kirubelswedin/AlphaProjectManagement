using ASP.ViewModels.Components;
using ASP.ViewModels.Forms;

namespace ASP.ViewModels.Views;

public class ClientsViewModel
{
    public PageHeaderViewModel PageHeader { get; set; } = new()
    {
        Title = "Client",
        ButtonText = "Add Client",
        ModalId = "addClientModal"
    };

    public IEnumerable<ClientListViewModel> Clients { get; set; } = [];
    
    // Forms data
    public AddClientViewModel AddClient { get; set; } = new();
    public EditClientViewModel EditClient { get; set; } = new();

}
