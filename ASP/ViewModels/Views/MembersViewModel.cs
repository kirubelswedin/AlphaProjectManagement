using ASP.ViewModels.Components;
using ASP.ViewModels.Forms;

namespace ASP.ViewModels.Views;

public class MembersViewModel
{
    public PageHeaderViewModel PageHeader { get; set; } = new()
    {
        Title = "Members",
        ButtonText = "Add Member",
        ModalId = "addmembermodal"
    };

    public IEnumerable<MemberCardViewModel> Members { get; set; } = [];
    
    // Forms data
    public AddMemberViewModel AddMember { get; set; } = new();
    public EditMemberViewModel EditMember { get; set; } = new();
}