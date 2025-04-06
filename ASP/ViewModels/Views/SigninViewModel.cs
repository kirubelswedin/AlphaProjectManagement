using System.ComponentModel.DataAnnotations;

namespace ASP.ViewModels.Views;

public class SigninViewModel
{
    [Required(ErrorMessage = "is required.")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Enter email address")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter password")]
    public string Password { get; set; } = null!;

    [Display(Name = "Keep me logged in")]
    public bool RememberMe { get; set; }
}
