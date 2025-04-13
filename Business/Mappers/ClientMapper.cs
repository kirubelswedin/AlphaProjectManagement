using Business.Dtos;
using Data.Entities;
using Domain.Models;

namespace Business.Mappers;

public static class ClientMapper
{
    public static ClientEntity ToEntity(AddClientFormDto? dto, string? newImageUrl = null)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        return new ClientEntity
        {
            ImageUrl = newImageUrl,
            ClientName = dto.ClientName,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            StreetAddress = dto.StreetAddress,
            PostalCode = dto.PostalCode,
            City = dto.City,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
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
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        return new Client
        {
            Id = entity.Id,
            ImageUrl = entity.ImageUrl,
            ClientName = entity.ClientName,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            StreetAddress = entity.StreetAddress,
            PostalCode = entity.PostalCode,
            City = entity.City
        };
    }
}
