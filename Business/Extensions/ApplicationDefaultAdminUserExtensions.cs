using Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Business.Extensions;

public static class ApplicationDefaultAdminUserExtensions
{
    public static IApplicationBuilder UseDefaultAdminAccount(
        this IApplicationBuilder app, 
        string email = "admin@domain.com", 
        string password = "BytMig123!", 
        string firstName = "System", 
        string lastName = "Administrator", 
        string role = "Admin")
    {
        return app.UseMiddleware<DefaultAdminAccountMiddleware>(email, password, firstName, lastName, role);
    }

    public class DefaultAdminAccountMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _email;
        private readonly string _password;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _role;

        public DefaultAdminAccountMiddleware(
            RequestDelegate next, 
            string email, 
            string password, 
            string firstName, 
            string lastName, 
            string role)
        {
            _next = next;
            _email = email;
            _password = password;
            _firstName = firstName;
            _lastName = lastName;
            _role = role;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<UserEntity> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync(_email);
            if (adminUser == null)
            {
                adminUser = new UserEntity
                {
                    UserName = _email,
                    Email = _email,
                    FirstName = _firstName,
                    LastName = _lastName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, _password);
                if (result.Succeeded)
                {
                    if (_role != null)
                        await userManager.AddToRoleAsync(adminUser, _role);
                }
            }

            await _next(context);
        }
    }
}