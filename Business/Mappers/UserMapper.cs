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

    public static UserEntity ToEntity(AddUserFormDto? dto, string? newImageUrl = null)
    {
        if (dto == null)
            return null!;

        return new UserEntity
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            JobTitle = dto.JobTitle,
            ImageUrl = newImageUrl ?? dto.ImageUrl,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static UserEntity ToEntity(UpdateUserFormDto? dto, string? newImageUrl = null)
    {
        if (dto == null)
            return null!;

        return new UserEntity
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            JobTitle = dto.JobTitle,
            ImageUrl = newImageUrl ?? dto.ImageUrl,
            UpdatedAt = DateTime.UtcNow
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

    public static UserDetailsDto ToUserDetailsDto(User? user)
    {
        if (user == null)
            return null!;

        return new UserDetailsDto
        {
            Id = user.Id,
            Avatar = user.ImageUrl,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            JobTitle = user.JobTitle,
            StreetAddress = user.Address?.StreetAddress ?? string.Empty,
            City = user.Address?.City ?? string.Empty,
            PostalCode = user.Address?.PostalCode ?? string.Empty,
            IsAdmin = false
        };
    }
}
