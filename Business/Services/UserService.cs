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
    Task<UserResult> AddUserToRoleAsync(UserEntity user, string roleName);
    Task<string> GetDisplayNameAsync(string userId);
    Task<UserResult<User>> GetUserByIdAsync(string id);
    Task<UserResult<IEnumerable<User>>> GetUsersAsync();
    Task<UserResult> UserExistsByEmailAsync(string email);
}

public class UserService(IUserRepository userRepository, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

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
}
