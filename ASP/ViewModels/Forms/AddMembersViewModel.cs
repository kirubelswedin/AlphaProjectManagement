using System.ComponentModel.DataAnnotations;

namespace ASP.ViewModels.forms;


public class MembersFormViewModel
{
    [DataType(DataType.Upload)]
    [Display(Name = "Image")]
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }
    
    [StringLength(100, ErrorMessage = "Job title cannot exceed 100 characters")]
    [Display(Name = "Job Title")]
    public string? JobTitle { get; set; }

}