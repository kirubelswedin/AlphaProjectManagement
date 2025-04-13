using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Dtos;

public class UpdateUserFormDto
{
    public string Id { get; set; } = null!;
    
    [Display(Name = "Profile Image", Prompt = "Upload Profile Image")]
    [DataType(DataType.Upload)]
    public IFormFile? NewImageFile { get; set; }
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