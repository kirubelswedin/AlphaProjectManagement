using ASP.ViewModels.Components;
using ASP.ViewModels.forms;

namespace ASP.ViewModels.Views;

public class MembersViewModel
{
    public PageHeaderViewModel PageHeader { get; set; } = new()
    {
        Title = "Member",
        ButtonText = "Add Member",
        ModalId = "addMemberModal"
    };
    
    public MembersFormViewModel MemberForm { get; set; } = new();
    
    public IEnumerable<MemberCardViewModel> Members { get; set; } = [];
}