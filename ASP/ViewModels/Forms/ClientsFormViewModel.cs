using System.ComponentModel.DataAnnotations;

namespace ASP.ViewModels.forms;

public class ClientsFormViewModel
{
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Client name is required")]
    [StringLength(100, ErrorMessage = "Client name cannot exceed 100 characters")]
    [Display(Name = "Client Name")]
    public string ClientName { get; set; } = null!;

    [Required(ErrorMessage = "Contact person is required")]
    [StringLength(100, ErrorMessage = "Contact person name cannot exceed 100 characters")]
    [Display(Name = "Contact Person")]
    public string ContactPerson { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Phone")]
    public string? Phone { get; set; }
}