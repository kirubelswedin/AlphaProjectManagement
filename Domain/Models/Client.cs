namespace Domain.Models;

public class Client
{
    public string Id { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string ClientName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string? Phone { get; set; }
    
}