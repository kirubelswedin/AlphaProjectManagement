using Business.Dtos;
using Domain.Responses;

namespace Business.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginFormData formData);
    Task SignOutAsync();
    Task<AuthResult> SignUpAsync(SignUpFormData formData);
}