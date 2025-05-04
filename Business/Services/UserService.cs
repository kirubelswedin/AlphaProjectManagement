using Business.Dtos;
using Business.Handlers;
using Business.Mappers;
using Data.Entities;
using Data.Repositories;
using Domain.Responses;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IUserService
{
    Task<UserResult<UserDetailsDto>> CreateMemberAsync(AddUserFormDto formDto, string findFirstValue);
    Task<UserResult<IEnumerable<UserDetailsDto>>> GetUsersAsync();
    Task<UserResult<UserDetailsDto>> GetUserByIdAsync(string id);
    Task<UserResult<UserDetailsDto>> GetUserByEmailAsync(string email);
    Task<UserResult> UserExistsByEmailAsync(string email);
    Task<UserResult> AddUserToRoleAsync(UserEntity user, string roleName);
    Task<string> GetDisplayNameAsync(string userId);
    Task<UserResult<UserDetailsDto>> UpdateMemberAsync(UpdateUserFormDto formDto);
    Task<UserResult<UserDetailsDto>> DeleteMemberAsync(string id);
}

public class UserService(
    IUserRepository userRepository,
    UserManager<UserEntity> userManager,
    RoleManager<IdentityRole> roleManager,
    IImageHandler imageHandler,  
    IProjectService projectService,
    ICacheHandler<IEnumerable<UserDetailsDto>> cacheHandler) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IImageHandler _imageHandler = imageHandler;
    private readonly IProjectService _projectService = projectService;
    
    private readonly ICacheHandler<IEnumerable<UserDetailsDto>> _cacheHandler = cacheHandler;
    private const string _cacheKey = "Users";

    // Creates a new user/member, saves profile image, and updates cache.
    public async Task<UserResult<UserDetailsDto>> CreateMemberAsync(AddUserFormDto? formDto, string findFirstValue)
    {
        try
        {
            if (formDto == null)
                return new UserResult<UserDetailsDto> { Succeeded = false, StatusCode = 400, Error = "All required fields are not provided" };

            string? imageUrl = null;
            if (formDto.ImageFile != null)
                imageUrl = await _imageHandler.SaveImageAsync(formDto.ImageFile, "users");

            var userEntity = UserMapper.ToEntity(formDto, imageUrl);
            var result = await _userRepository.AddAsync(userEntity);

            if (!result.Succeeded)
                return new UserResult<UserDetailsDto> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

            await UpdateCacheAsync();

            var dto = UserMapper.ToDetailsDto(userEntity);
            return new UserResult<UserDetailsDto> { Succeeded = result.Succeeded, StatusCode = result.StatusCode, Error = result.Error, Result = dto };
        }
        catch (Exception ex)
        { return new UserResult<UserDetailsDto> { Succeeded = false, StatusCode = 500, Error = $"Failed to create user: {ex.Message}" }; }
    }

    // Gets all users, uses cache if available, otherwise fetches from db and updates cache.
    public async Task<UserResult<IEnumerable<UserDetailsDto>>> GetUsersAsync()
    {
        var cachedUsers = _cacheHandler.GetFromCache(_cacheKey);
        if (cachedUsers != null)
            return new UserResult<IEnumerable<UserDetailsDto>> { Succeeded = true, StatusCode = 200, Result = cachedUsers };

        var result = await _userRepository.GetAllAsync();
        if (!result.Succeeded)
            return new UserResult<IEnumerable<UserDetailsDto>> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        var users = result.Result!.Select(UserMapper.ToDetailsDto).ToList();
        _cacheHandler.SetCache(_cacheKey, users);
        return new UserResult<IEnumerable<UserDetailsDto>> { Succeeded = true, StatusCode = 200, Result = users };
    }

    public async Task<UserResult<UserDetailsDto>> GetUserByIdAsync(string id)
    {
        var result = await _userRepository.GetAsync(x => x.Id == id);
        if (!result.Succeeded)
            return new UserResult<UserDetailsDto> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        var user = UserMapper.ToDetailsDto(result.Result);
        return new UserResult<UserDetailsDto> { Succeeded = true, StatusCode = 200, Result = user };
    }
    
    public async Task<UserResult<UserDetailsDto>> GetUserByEmailAsync(string email)
    {
        var result = await _userRepository.GetAsync(x => x.Email == email);
        if (!result.Succeeded)
            return new UserResult<UserDetailsDto> { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

        var user = UserMapper.ToDetailsDto(result.Result);
        return new UserResult<UserDetailsDto> { Succeeded = true, StatusCode = 200, Result = user };
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

    // Updates an existing user/member, including image,address, and updates cache.
    public async Task<UserResult<UserDetailsDto>> UpdateMemberAsync(UpdateUserFormDto formDto)
    {
        try
        {
            // Fetch user with address included
            var userResult = await _userRepository.GetAsync(x => x.Id == formDto.Id);
            if (!userResult.Succeeded || userResult.Result == null)
                return new UserResult<UserDetailsDto> { Succeeded = false, StatusCode = 404, Error = "User not found" };
            
            var existingUser = userResult.Result;
            
            if (existingUser.Email != formDto.Email)
            {
                var emailExists = await UserExistsByEmailAsync(formDto.Email);
                if (emailExists.Succeeded)
                    return new UserResult<UserDetailsDto>
                        { Succeeded = false, StatusCode = 409, Error = "A user with this email already exists." };
            }

            var imageUrl = existingUser.ImageUrl;
            if (formDto.ImageFile != null)
                imageUrl = await _imageHandler.SaveImageAsync(formDto.ImageFile, "users");

            UserMapper.ApplyUpdatesToEntity(formDto, existingUser, imageUrl);

            // Update user
            var result = await _userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
                return new UserResult<UserDetailsDto> { Succeeded = false, StatusCode = 400, Error = result.Errors.FirstOrDefault()?.Description };

            await UpdateCacheAsync();

            var dto = UserMapper.ToDetailsDto(existingUser);
            return new UserResult<UserDetailsDto> { Succeeded = true, StatusCode = 200, Result = dto };
        }
        catch (Exception ex)
        { return new UserResult<UserDetailsDto> { Succeeded = false, StatusCode = 500, Error = $"Failed to update user: {ex.Message}" }; }
    }
    
    // Deletes a user/member, removes them from all projects, and updates cache.
    public async Task<UserResult<UserDetailsDto>> DeleteMemberAsync(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return new UserResult<UserDetailsDto> { Succeeded = false, StatusCode = 404, Error = "User not found" };

            // remove user from all projects
            await _projectService.RemoveUserFromAllProjectsAsync(id);

            // Delete user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return new UserResult<UserDetailsDto> { Succeeded = false, StatusCode = 400, Error = result.Errors.FirstOrDefault()?.Description };

            await UpdateCacheAsync();

            return new UserResult<UserDetailsDto> { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        { return new UserResult<UserDetailsDto> { Succeeded = false, StatusCode = 500, Error = ex.Message }; }
    }

    // Updates the user cache 
    // Not using the return value for now
    private async Task<IEnumerable<UserDetailsDto>> UpdateCacheAsync()
    {
        var result = await _userRepository.GetAllAsync();
        if (!result.Succeeded)
            return [];
        
        var entities = result.Result!;
        var users = entities.Select(UserMapper.ToDetailsDto).ToList();
        _cacheHandler.SetCache(_cacheKey, users);
        return users;
    }
}
