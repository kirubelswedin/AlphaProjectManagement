using System.Security.Claims;
using ASP.Extensions;
using Microsoft.AspNetCore.Mvc;
using ASP.ViewModels.Views;
using Business.Dtos;
using Business.Services;
using Domain.Extensions;

namespace ASP.Controllers;

public class AuthController(IAuthService authService, INotificationService notificationService, IUserService userService ) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IUserService _userService = userService;
    
    [Route("auth/signup")]
    public IActionResult SignUp(string returnUrl = "~/")
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = "";
        return View();
    }

    [HttpPost]
    [Route("auth/signup")]
    public async Task<IActionResult> SignUp(SignUpViewModel model, string returnUrl = "~/")
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ErrorMessage = "";
            return View(model);
        }

        var signUpFormData = model.MapTo<SignUpFormDto>();
        var authResult = await _authService.SignUpAsync(signUpFormData);

        if (authResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        ModelState.AddModelError("Unable", "Unable to create user. Try another email or password.");
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = authResult.Error;
        return View(model);
    }


    [Route("auth/login")]
    public IActionResult Login(string returnUrl = "~/")
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = "";
        return View();
    }

    [HttpPost]
    [Route("auth/login")]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "~/")
    {
        if (ModelState.IsValid)
        {
            var loginFormData = model.MapTo<SignInFormDto>();
            var authResult = await _authService.SignInAsync(loginFormData);

            if (authResult.Succeeded)
            {
                // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // getting user from email instead of claims
                var userResult = await _userService.GetUserByEmailAsync(loginFormData.Email!);
                var user = userResult.Result;

                if (user != null)
                {
                    var notificationFormData = new NotificationDetailsDto
                    {
                        Title = "User Login",
                        NotificationTypeId = 1,
                        NotificationTargetId = 1,
                        Message = $"{user.FirstName} {user.LastName} signed in.",
                        ImageUrl = (user.ImageUrl ?? "default-user.svg").GetImageUrl("users"),
                        ImageType = "avatars",
                        CreatedAt = DateTime.UtcNow,
                        UserId = user.Id
                    };

                    var notificationResult = await _notificationService.AddNotificationAsync(notificationFormData);
                    if (!notificationResult.Succeeded)
                    {
                        Console.WriteLine($"[ERROR] Kunde inte skapa notification: {notificationResult.Error}");
                    }
                }
                else
                {
                    Console.WriteLine($"[ERROR] Ingen användare hittades med email: {loginFormData.Email}");
                }

                return LocalRedirect(returnUrl);
            }
        }

        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = "Unable to login. Try another email or password.";
        return View(model);
    }


    [HttpGet]
    [Route("auth/logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return LocalRedirect("~/");
    }
}
