namespace Domain.Models;

public class User
{
    public string Id { set; get; } = null!;
    public string? ImageUrl { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? JobTitle { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public UserAddress? Address { get; set; }
}