namespace Domain.Models;

public class Client
{
    public string Id { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string ClientName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string ContactPerson => $"{FirstName} {LastName}"; 
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? StreetAddress { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
}