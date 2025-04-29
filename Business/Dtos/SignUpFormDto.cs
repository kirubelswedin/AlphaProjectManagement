using System.ComponentModel.DataAnnotations;

namespace Business.Dtos;

public class SignUpFormDto
{
    [Required(ErrorMessage = "First Name is required")]
    public string FirstName { get; set; } = null!;
    
    [Required(ErrorMessage = "Last Name is required")]
    public string LastName { get; set; } = null!;
    
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid email address"
    )]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, and one number."
    )]
    public string Password { get; set; } = null!;
    
    public string? RoleName { get; set; }
}