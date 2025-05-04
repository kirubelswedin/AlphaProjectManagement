using Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Business.Extensions;

// Set up: Ensures default admin user exists in the system at startup.
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

    public class DefaultAdminAccountMiddleware( RequestDelegate next, string email, string password, string firstName, string lastName, string role)
    {
        private readonly RequestDelegate _next = next;
        private readonly string _email = email;
        private readonly string _password = password;
        private readonly string _firstName = firstName;
        private readonly string _lastName = lastName;
        private readonly string _role = role;

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
                    await userManager.AddToRoleAsync(adminUser, _role);
                
                throw new Exception($"Failed to create default admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await _next(context);
        }
    }
}