using ASP.ViewModels.Components;
using ASP.ViewModels.forms;

namespace ASP.ViewModels.Views;

public class MembersViewModel
{
    public PageHeaderViewModel PageHeader { get; set; } = new()
    {
        Title = "Members",
        ButtonText = "Add Member",
        ModalId = "addmembermodal"
    };

    public AddMembersViewModel AddMember { get; set; } = new();

    public IEnumerable<MemberCardViewModel> Members { get; set; } = [];
}