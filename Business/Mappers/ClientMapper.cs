using Business.Dtos;
using Data.Entities;
using Domain.Models;

namespace Business.Mappers;

public static class ClientMapper
{
    public static ClientEntity ToEntity(AddClientFormDto? dto, string? newImageUrl = null)
    {
        if (dto == null) return null!;
        return new ClientEntity
        {
            ImageUrl = newImageUrl,
            ClientName = dto.ClientName
        };
    }

    public static ClientEntity ToEntity(UpdateClientFormDto? dto, string? newImageUrl = null)
    {
        if (dto == null) return null!;
        return new ClientEntity
        {
            Id = dto.Id,
            ImageUrl = newImageUrl ?? dto.ImageUrl,
            ClientName = dto.ClientName
        };
    }

    public static Client ToModel(ClientEntity? entity)
    {
        if (entity == null) return null!;
        return new Client
        {
            Id = entity.Id,
            ImageUrl = entity.ImageUrl,
            ClientName = entity.ClientName
            
        };
    }
}
