using ASP.ViewModels.Components;
using ASP.ViewModels.forms;
using Business.Dtos.Forms;

namespace ASP.ViewModels.Views;

public class MembersViewModel
{
    public PageHeaderViewModel PageHeader { get; set; } = new()
    {
        Title = "Members",
        ButtonText = "Add Member",
        ModalId = "addMemberModal"
    };
    
    public MembersFormViewModel MemberForm { get; set; } = new();
    
    public IEnumerable<MemberCardViewModel> Members { get; set; } = [];
}