namespace Business.Dtos.Forms;

public class ClientFormData
{
    public string? Image { get; set; }
    public string ClientName { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string ButtonText { get; set; } = "Add Client"; 
}