using ASP.Extensions;
using ASP.ViewModels.Components;
using ASP.ViewModels.Forms;
using Business.Dtos;

namespace ASP.Mappers;

public class MemberViewModelMapper
{
    public static AddUserFormDto ToAddMemberFormDto(AddMemberViewModel model)
    {
        return new AddUserFormDto
        {
            ImageFile = model.ImageFile,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            JobTitle = model.JobTitle,
            StreetAddress = model.StreetAddress,
            PostalCode = model.PostalCode,
            City = model.City,
            
        };
    }
    
    
    public static UpdateUserFormDto ToUpdateMemberFormDto(EditMemberViewModel model)
    {
        return new UpdateUserFormDto
        {
            Id = model.Id,
            ImageFile = model.ImageFile,
            ImageUrl = model.ImageUrl,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            JobTitle = model.JobTitle,
            StreetAddress = model.StreetAddress,
            PostalCode = model.PostalCode,
            City = model.City
        };
    }
    
    
    
    public static MemberCardViewModel ToMemberCardViewModel(UserDetailsDto user)
    {
        return new MemberCardViewModel
        {
            Id = user.Id,
            ImageUrl = user.ImageUrl.GetImageUrl("members"),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            JobTitle = user.JobTitle,
            StreetAddress = user.StreetAddress,
            PostalCode = user.PostalCode,
            City = user.City,
        };
    }
    
}