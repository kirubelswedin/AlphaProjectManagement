using Microsoft.AspNetCore.Mvc;
using ASP.ViewModels.Views;
using Business.Dtos;
using Business.Interfaces;
using Domain.Extensions;
using System.Security.Claims;
using Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ASP.Controllers;

public class AuthController(IAuthService authService, INotificationService notificationService, IUserService userService, IHubContext<NotificationHub> notificationHub) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly IUserService _userService = userService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
    
    #region Local SignUp
    
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

        var signUpFormData = model.MapTo<SignUpFormData>();
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
    #endregion


    #region Sign In
    
    [HttpGet]
    [Route("auth/signin")]
    public IActionResult Signin(string returnUrl = "~/")
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = "";

        return View();
    }

    [HttpPost]
    [Route("auth/signin")]
    public async Task<IActionResult> Signin(SigninViewModel model, string returnUrl = "~/")
    {
        if (ModelState.IsValid)
        {
            var signInFormData = model.MapTo<SignInFormData>();
            var authResult = await _authService.SignInAsync(signInFormData);

            if (authResult.Succeeded)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userResult = await _userService.GetUserByIdAsync(userId!);
                var user = userResult.Result;

                if (user != null)
                {
                    var notificationFormData = new NotificationFormData
                    {
                        Title = "User Signin",
                        NotificationTypeId = 1,
                        NotificationTargetId = 1,
                        Message = $"{user.FirstName} {user.LastName} signed in.",
                        Image = user.Image,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _notificationService.AddNotificationAsync(notificationFormData, user.Id); 
                    var notifications = await _notificationService.GetNotificationsAsync("");
                    var newNotification = notifications.Result?.OrderByDescending(x => x.CreatedAt).FirstOrDefault();

                    if (newNotification != null)
                    {
                        await _notificationHub.Clients.All.SendAsync("ReceiveNotification", newNotification);
                    }
                    
                }

                if (string.IsNullOrEmpty(returnUrl) || returnUrl == "~/")
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "admin" });
                }

                return LocalRedirect(returnUrl);
            }
        }

        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = "Unable to signin. Try another email or password.";
        return View(model);
    }  
    
    #endregion
    

    #region Sign Out
    
    [Route("auth/signout")]
    public async Task<IActionResult> Signout()
    {
        await _authService.SignOutAsync();
        return LocalRedirect("~/");
    }
    
    #endregion
  }
