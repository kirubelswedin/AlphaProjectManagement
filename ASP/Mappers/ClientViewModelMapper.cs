using ASP.Extensions;
using ASP.ViewModels.Components;
using ASP.ViewModels.Forms;
using Business.Dtos;

namespace ASP.Mappers;

public class ClientViewModelMapper
{
    public static AddClientFormDto ToAddClientFormDto(AddClientViewModel model)
    {
        return new AddClientFormDto
        {
            ImageFile = model.ImageFile,
            ClientName = model.ClientName,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            StreetAddress = model.StreetAddress,
            PostalCode = model.PostalCode,
            City = model.City
        };
    }
    
    public static UpdateClientFormDto ToUpdateClientFormDto(EditClientViewModel model)
    {
        return new UpdateClientFormDto
        {
            Id = model.Id,
            ImageFile = model.ImageFile,
            ImageUrl = (model.ImageUrl ?? "default-client.svg").GetImageUrl("clients"),
            ClientName = model.ClientName,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            StreetAddress = model.StreetAddress,
            PostalCode = model.PostalCode,
            City = model.City
        };
    }
    
    public static ClientListViewModel ToClientListViewModel(ClientDetailsDto client)
    {
        return new ClientListViewModel
        {
            Id = client.Id,
            ImageUrl = (client.ImageUrl ?? "default-client.svg").GetImageUrl("clients"),
            ClientName = client.ClientName,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            StreetAddress = client.StreetAddress,
            PostalCode = client.PostalCode,
            City = client.City
        };
    }
}