using System.ComponentModel.DataAnnotations;

namespace ASP.ViewModels.Forms;

public class EditClientViewModel
{
    [Required]
    public string Id { get; set; } = null!;
    
    [DataType(DataType.Upload)]
    [Display(Name = "Client Image", Prompt = "Upload Client Image")]
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
    
    [Required(ErrorMessage = "Company name is required")]
    [Display(Name = "Company Name", Prompt = "Enter Company name")]
    public string ClientName { get; set; } = null!;

    [Required(ErrorMessage = "First name is required")]
    [Display(Name = "First Name", Prompt = "Enter first name")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    [Display(Name = "Last Name", Prompt = "Enter last name")]
    public string LastName { get; set; } = null!;
    
    public string ContactPerson => $"{FirstName} {LastName}".Trim();

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email", Prompt = "Enter email address")]
    public string Email { get; set; } = null!;

    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Phone Number", Prompt = "Enter phone number")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Address", Prompt = "Enter street address")]
    public string? StreetAddress { get; set; }

    [Display(Name = "Postal Code", Prompt = "Enter ZIP / Postal Code")]
    [DataType(DataType.PostalCode)]
    public string? PostalCode { get; set; }

    [Display(Name = "City", Prompt = "Enter city")]
    public string? City { get; set; }
}