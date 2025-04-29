using Business.Dtos;
using Data.Entities;
using Domain.Models;

namespace Business.Mappers;

public static class ClientMapper
{
    public static ClientEntity ToEntity(AddClientFormDto? dto, string? imageUrl = null)
    {
        if (dto == null) return null!;

        return new ClientEntity
        {
            Id = Guid.NewGuid().ToString(),
            ImageUrl = imageUrl,
            ClientName = dto.ClientName,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            StreetAddress = dto.StreetAddress,
            PostalCode = dto.PostalCode,
            City = dto.City,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static ClientEntity ToEntity(UpdateClientFormDto? dto, string? imageUrl = null)
    {
        if (dto == null) return null!;

        return new ClientEntity
        {
            Id = dto.Id,
            ImageUrl = imageUrl ?? dto.ImageUrl,
            ClientName = dto.ClientName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Client ToModel(ClientEntity? entity)
    {
        if (entity == null)
            return null!;

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
    
    public static ClientDetailsDto ToDetailsDto(ClientEntity? entity)
    {
        if (entity == null) return null!;

        return new ClientDetailsDto
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
    
    public static void ApplyUpdatesToEntity(UpdateClientFormDto form, ClientEntity entity, string? imageUrl = null)
    {
        entity.ClientName = form.ClientName;
        entity.FirstName = form.FirstName;
        entity.LastName = form.LastName;
        entity.Email = form.Email;
        entity.PhoneNumber = form.PhoneNumber;
        entity.StreetAddress = form.StreetAddress;
        entity.PostalCode = form.PostalCode;
        entity.City = form.City;
        
        if (imageUrl != null)
            entity.ImageUrl = imageUrl;
    }
}
