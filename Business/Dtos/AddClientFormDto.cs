using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Dtos;

public class AddClientFormDto
{
    public IFormFile? ImageFile { get; set; }
    
    [Required(ErrorMessage = "Company name is required")]
    [Display(Name = "Company Name", Prompt = "Enter Company name")]
    public string ClientName { get; set; } = null!;

    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Enter Email")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid email address"
    )]
    public string Email { get; set; } = null!;

    [Display(Name = "PhoneNumber", Prompt = "Enter PhoneNumber Number")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Street Address", Prompt = "Enter street address")]
    public string? StreetAddress { get; set; }

    [Display(Name = "Postal Code", Prompt = "Enter postal code")]
    [DataType(DataType.PostalCode)]
    public string? PostalCode { get; set; }

    [Display(Name = "City", Prompt = "Enter city")]
    public string? City { get; set; }
}

