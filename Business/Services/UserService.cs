using Business.Dtos;
using Business.Mappers;
using Data.Entities;
using Data.Repositories;
using Domain.Models;
using Domain.Responses;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IUserService
{
    Task<UserResult> CreateMemberAsync(AddUserFormDto formDto);
    Task<UserResult<IEnumerable<User>>> GetUsersAsync();
    Task<UserResult<User>> GetUserByIdAsync(string id);
    Task<UserResult> UserExistsByEmailAsync(string email);
    Task<UserResult> AddUserToRoleAsync(UserEntity user, string roleName);
    Task<string> GetDisplayNameAsync(string userId);
    Task<UserResult> UpdateMemberAsync(UpdateUserFormDto formDto);
    Task<UserResult> DeleteMemberAsync(string id);
}

public class UserService(IUserRepository userRepository, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public async Task<UserResult> CreateMemberAsync(AddUserFormDto formDto)
    {
        if (formDto == null)
            return new UserResult { Succeeded = false, StatusCode = 400, Error = "Form data is required" };

        var userEntity = UserMapper.ToEntity(formDto);
        var result = await _userRepository.AddAsync(userEntity);

        if (!result.Succeeded)
            return new UserResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        return new UserResult
        {
            Succeeded = true,
            StatusCode = 201,
            Result = userEntity.Id
        };
    }


    public async Task<UserResult<IEnumerable<User>>> GetUsersAsync()
    {
        var result = await _userRepository.GetAllAsync();
        if (!result.Succeeded)
            return new UserResult<IEnumerable<User>> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        var users = result.Result!.Select(entity => UserMapper.ToModel(entity));
        return new UserResult<IEnumerable<User>> { Succeeded = true, StatusCode = 200, Result = users };
    }

    public async Task<UserResult<User>> GetUserByIdAsync(string id)
    {
        var result = await _userRepository.GetAsync(x => x.Id == id);
        if (!result.Succeeded)
            return new UserResult<User> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        var user = UserMapper.ToModel(result.Result);
        return new UserResult<User> { Succeeded = true, StatusCode = 200, Result = user };
    }

    public async Task<UserResult> UserExistsByEmailAsync(string email)
    {
        var result = await _userRepository.ExistsAsync(x => x.Email == email);
        return new UserResult { Succeeded = result.Succeeded, StatusCode = result.StatusCode, Error = result.Error };
    }

    public async Task<UserResult> AddUserToRoleAsync(UserEntity user, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
            return new UserResult { Succeeded = false, StatusCode = 400, Error = $"Role '{roleName}' does not exist." };

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return new UserResult { Succeeded = result.Succeeded, StatusCode = result.Succeeded ? 200 : 400, Error = result.Errors.FirstOrDefault()?.Description };
    }

    public async Task<string> GetDisplayNameAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
            return "Unknown User";

        var user = await _userManager.FindByIdAsync(userId);
        return user != null ? $"{user.FirstName} {user.LastName}" : "Unknown User";
    }

    public async Task<UserResult> UpdateMemberAsync(UpdateUserFormDto formDto)
    {
        try
        {
            // Get existing user
            var user = await _userManager.FindByIdAsync(formDto.Id);
            if (user == null)
                return new UserResult { Succeeded = false, StatusCode = 404, Error = "User not found" };

            // Check if email is being changed and if it already exists
            if (user.Email != formDto.Email)
            {
                var emailExists = await UserExistsByEmailAsync(formDto.Email);
                if (emailExists.Succeeded)
                    return new UserResult { Succeeded = false, StatusCode = 409, Error = "A user with this email already exists." };
            }

            // Update user entity
            var updatedUser = UserMapper.ToEntity(formDto);
            updatedUser.UserName = updatedUser.Email;

            // Update user
            var result = await _userManager.UpdateAsync(updatedUser);
            if (!result.Succeeded)
                return new UserResult { Succeeded = false, StatusCode = 400, Error = result.Errors.FirstOrDefault()?.Description };

            return new UserResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            return new UserResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    public async Task<UserResult> DeleteMemberAsync(string id)
    {
        try
        {
            // Get existing user
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return new UserResult { Succeeded = false, StatusCode = 404, Error = "User not found" };

            // Delete user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return new UserResult { Succeeded = false, StatusCode = 400, Error = result.Errors.FirstOrDefault()?.Description };

            return new UserResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            return new UserResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }
}
