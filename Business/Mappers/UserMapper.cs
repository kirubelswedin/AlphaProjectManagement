using Business.Dtos;
using Data.Entities;
using Domain.Models;

namespace Business.Mappers;

public static class UserMapper
{
    public static UserEntity ToEntity(SignUpFormDto? dto)
    {
        if (dto == null)
            return null!;

        return new UserEntity
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static UserEntity ToEntity(AddUserFormDto dto, string? imageUrl = null)
    {
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

    public static UserEntity ToEntity(UpdateUserFormDto? dto, string? newImageUrl = null)
    {
        if (dto == null)
            return null!;

        return new UserEntity
        {
            Id = dto.Id,
            ImageUrl = newImageUrl ?? dto.ImageUrl,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            JobTitle = dto.JobTitle,
            UpdatedAt = DateTime.UtcNow,
            Address = new UserAddressEntity
            {
                StreetAddress = dto.StreetAddress,
                PostalCode = dto.PostalCode,
                City = dto.City
            }
        };
    }

    public static User ToModel(UserEntity? entity)
    {
        if (entity == null)
            return null!;

        return new User
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            JobTitle = entity.JobTitle,
            ImageUrl = entity.ImageUrl,
            Address = entity.Address != null ? new UserAddress
            {
                Id = entity.Address.UserId,
                StreetAddress = entity.Address.StreetAddress,
                PostalCode = entity.Address.PostalCode,
                City = entity.Address.City,
                Country = entity.Address.Country
            } : null
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
    
}
