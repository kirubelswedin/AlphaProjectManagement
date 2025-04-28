using System.ComponentModel.DataAnnotations;

namespace ASP.ViewModels.Forms;


public class AddMemberViewModel
{
    [DataType(DataType.Upload)]
    [Display(Name = "Image")]
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "PhoneNumber")]
    public string? PhoneNumber { get; set; }
    
    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }
    
    [Display(Name = "Address", Prompt = "Enter street address")]
    public string? StreetAddress { get; set; }
    
    [Display(Name = "Postal Code", Prompt = "Enter postal code")]
    public string? PostalCode { get; set; }
    
    [Display(Name = "City", Prompt = "Enter city")]
    public string? City { get; set; }
}