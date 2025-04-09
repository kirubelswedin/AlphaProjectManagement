using Business.Dtos;
using Data.Entities;
using Domain.Extensions;
using Domain.Responses;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IAuthService
{
    Task<AuthResult> SignInAsync(SignInFormDto formDto);
    Task<AuthResult> SignUpAsync(SignUpFormDto formDto);
    Task SignOutAsync();
}

public class AuthService(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager, IUserService userService) : IAuthService
{
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly IUserService _userService = userService;


    public async Task<AuthResult> SignUpAsync(SignUpFormDto formDto)
    {
        if (formDto == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "form data can't be null." };

        var userResult = await _userService.UserExistsByEmailAsync(formDto.Email);
        if (userResult.Succeeded)
            return new AuthResult { Succeeded = false, StatusCode = 409, Error = userResult.Error };

        try
        {
            var userEntity = formDto.MapTo<UserEntity>();
            userEntity.UserName = userEntity.Email;

            var identityResult = await _userManager.CreateAsync(userEntity, formDto.Password);
            if (identityResult.Succeeded)
            {
                if (formDto.RoleName != null)
                {
                    var result = await _userService.AddUserToRoleAsync(userEntity, formDto.RoleName);
                }

                return new AuthResult { Succeeded = true, StatusCode = 201, SuccessMessage = $"User was created successfully." };
            }

            throw new Exception("Unable to sign up user");

        }
        catch (Exception ex)
        {
            return new AuthResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    public async Task<AuthResult> SignInAsync(SignInFormDto formDto)
    {
        if (formDto == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "form data can't be null." };

        var signInResult = await _signInManager.PasswordSignInAsync(formDto.Email, formDto.Password, formDto.IsPersistent, false);
        return signInResult.Succeeded
            ? new AuthResult { Succeeded = true, StatusCode = 200 }
            : new AuthResult { Succeeded = false, StatusCode = 401 };
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
    
    // public async Task<IEnumerable<User>> UpdateCacheAsync()
    // {
    //     var entities = await _userManager.Users.ToListAsync();
    //     var models = entities.Select(UserMapper.ToModel).ToList();
    //
    //     _cacheHandler.SetCache(_cacheKey, models);
    //     return models;
    // }
}
