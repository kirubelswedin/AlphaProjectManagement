namespace Business.Dtos;

public class AddClientDto
{
    public string? ImageUrl { get; set; }
    public string ClientName { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string ButtonText { get; set; } = "Add Client"; 
}