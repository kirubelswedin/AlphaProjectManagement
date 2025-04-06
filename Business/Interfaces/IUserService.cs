using Data.Entities;
using Domain.Models;
using Domain.Responses;

namespace Business.Interfaces;

public interface IUserService
{
    Task<UserResult> AddUserToRoleAsync(UserEntity user, string roleName);
    Task<string> GetDisplayNameAsync(string userId);
    Task<UserResult<User>> GetUserByIdAsync(string id);
    Task<UserResult<IEnumerable<User>>> GetUsersAsync();
    Task<UserResult> UserExistsByEmailAsync(string email);
}