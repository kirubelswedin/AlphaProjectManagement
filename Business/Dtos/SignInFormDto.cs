using System.ComponentModel.DataAnnotations;

namespace Business.Dtos;

public class SignInFormDto
{
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid email address"
    )]
    public string Email { get; set; } = null!;
    
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
    
    public bool IsPersistent { get; set; }
}