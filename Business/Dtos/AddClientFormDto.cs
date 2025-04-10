namespace Business.Dtos;

public class AddClientFormDto
{
    public string? ImageUrl { get; set; }
    public string ClientName { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
}