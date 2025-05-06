using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Dtos;

public class UpdateUserDto
{
    public string Id { get; set; } = null!;
    
    [Display(Name = "Profile Image", Prompt = "Upload Profile Image")]
    [DataType(DataType.Upload)]
    public IFormFile? NewImageUrl { get; set; }
    public string? ImageUrl { get; set; }

    [Display(Name = "First Name", Prompt = "Enter First Name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "First Name is required")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Last Name", Prompt = "Enter Last Name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Last Name is required")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Enter Email")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
        ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = null!;

    [Display(Name = "Job Title", Prompt = "Enter Job Title")]
    [DataType(DataType.Text)]
    public string? JobTitle { get; set; }

    [Display(Name = "Phone", Prompt = "Enter Phone Number")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    
    public string? StreetName { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; } 
}