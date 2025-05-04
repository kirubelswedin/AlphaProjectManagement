using Business.Dtos;
using Data.Entities;
using Domain.Models;

namespace Business.Mappers;

public static class UserMapper
{
    public static UserEntity ToEntity(SignUpFormDto? dto)
    {
        if (dto == null) return null!;
        return new UserEntity
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static UserEntity ToEntity(AddUserFormDto? dto, string? imageUrl = null)
    {
        if (dto == null) return null!;
        return new UserEntity
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            JobTitle = dto.JobTitle,
            CreatedAt = DateTime.UtcNow,
            ImageUrl = imageUrl,
            Address = new UserAddressEntity
            {
                StreetAddress = dto.StreetAddress,
                PostalCode = dto.PostalCode,
                City = dto.City
            }
        };
    }

    public static UserDetailsDto ToDetailsDto(UserEntity? entity)
    {
        if (entity == null) return null!;
        return new UserDetailsDto
        {
            Id = entity.Id,
            ImageUrl = entity.ImageUrl,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            JobTitle = entity.JobTitle,
            StreetAddress = entity.Address?.StreetAddress ?? string.Empty,
            City = entity.Address?.City ?? string.Empty,
            PostalCode = entity.Address?.PostalCode ?? string.Empty,
            IsAdmin = false
        };
    }
    
    public static void ApplyUpdatesToEntity(UpdateUserFormDto dto, UserEntity entity, string? imageUrl = null)
    {
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.Email = dto.Email;
        entity.UserName = dto.Email;
        entity.PhoneNumber = dto.PhoneNumber;
        entity.JobTitle = dto.JobTitle;
        entity.UpdatedAt = DateTime.UtcNow;
        
        // Manage address
        if (entity.Address == null)
        {
            entity.Address = new UserAddressEntity {
                UserId = entity.Id,
                StreetAddress = dto.StreetAddress,
                PostalCode = dto.PostalCode,
                City = dto.City
            };
        }
        else
        {
            entity.Address.StreetAddress = dto.StreetAddress;
            entity.Address.PostalCode = dto.PostalCode;
            entity.Address.City = dto.City;
        }

        if (imageUrl != null)
            entity.ImageUrl = imageUrl;
    }
    
}
