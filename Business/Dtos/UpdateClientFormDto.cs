using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Dtos;

public class UpdateClientFormDto
{
    [Required]
    public string Id { get; set; } = null!;

    [Display(Name = "Client Image", Prompt = "Upload Client Image")]
    [DataType(DataType.Upload)]
    public string? ImageUrl { get; set; }
    public IFormFile? NewImageUrl { get; set; }

    [Display(Name = "Client Name", Prompt = "Enter Client Name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Client Name is required")]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Enter Email")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address"
    )]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone", Prompt = "Enter Phone")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; } = null!;
}