namespace Domain.Models;

public class UserAddress
{
    public string Id { get; set; }
    public string? StreetAddress { get; set; }
    public string? PostalCode { get; set; } 
    public string? City { get; set; } 
    public string? Country { get; set; } 
}