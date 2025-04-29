using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Dtos;

public class AddClientFormDto
{
    [DataType(DataType.Upload)]
    public IFormFile? ImageFile { get; set; }
    
    [Required(ErrorMessage = "Company name is required")]
    public string ClientName { get; set; } = null!;

    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = null!;
    
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid email address"
    )]
    public string Email { get; set; } = null!;
    
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }
    
    public string? StreetAddress { get; set; }
    
    [DataType(DataType.PostalCode)]
    public string? PostalCode { get; set; }
    
    public string? City { get; set; }
}

