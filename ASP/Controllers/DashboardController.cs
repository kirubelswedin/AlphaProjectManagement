using ASP.ViewModels.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Controllers;

[Authorize]
public class DashboardController : Controller
{
    [Route("admin/dashboard")]
    public IActionResult Index()
    {
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
        }

        var viewModel = new DashboardViewModel
        {
            PageHeader = new()
            {
                Title = "Dashboard"
            }
        };
        
        ViewBag.IsAdmin = User.IsInRole("Admin");
        ViewBag.UserEmail = User.Identity?.Name;

        return View(viewModel);
    }
}