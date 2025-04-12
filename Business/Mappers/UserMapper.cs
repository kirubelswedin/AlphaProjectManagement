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
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };
    }

    public static UserEntity ToEntity(AddUserFormDto? dto, string? newImageUrl = null)
    {
        if (dto == null) return null!;
        return new UserEntity
        {
            UserName = dto.Email,
            ImageUrl = newImageUrl,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };
    }

    public static UserEntity ToEntity(UpdateUserFormDto? dto, string? newImageUrl = null)
    {
        if (dto == null) return null!;
        return new UserEntity
        {
            Id = dto.Id,
            UserName = dto.Email,
            ImageUrl = newImageUrl ?? dto.ImageUrl,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = new UserAddressEntity
            {
                UserId = dto.Id,
                StreetAddress = dto.StreetName,
                PostalCode = dto.PostalCode,
                City = dto.City,
                Country = dto.Country
            }
        };
    }

    public static User ToModel(UserEntity? entity)
    {
        if (entity == null) return null!;
        return new User
        {
            Id = entity.Id,
            ImageUrl = entity.ImageUrl,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email!,
            PhoneNumber = entity.PhoneNumber,
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
}
