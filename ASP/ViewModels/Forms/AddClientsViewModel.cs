using System.ComponentModel.DataAnnotations;

namespace ASP.ViewModels.forms;

public class AddClientsViewModel
{
    [Display(Name = "Client Image", Prompt = "Upload Client Image")]
    public IFormFile? ImageFile { get; set; }

    [Required(ErrorMessage = "Company name is required")]
    [Display(Name = "Company Name", Prompt = "Enter Company name")]
    public string ClientName { get; set; } = null!;

    [Required(ErrorMessage = "First name is required")]
    [Display(Name = "First Name", Prompt = "Enter first name")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    [Display(Name = "Last Name", Prompt = "Enter last name")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email", Prompt = "Enter email address")]
    public string Email { get; set; } = null!;

    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Phone Number", Prompt = "Enter phone number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Street StreetAddress", Prompt = "Enter street address")]
    public string? StreetAddress { get; set; }

    [Display(Name = "Postal Code", Prompt = "Enter postal code")]
    public string? PostalCode { get; set; }

    [Display(Name = "City", Prompt = "Enter city")]
    public string? City { get; set; }
}
