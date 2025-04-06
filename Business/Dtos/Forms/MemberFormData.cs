namespace Business.Dtos.Forms;

public class MemberFormData
{
    public string? Image { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    
    public MemberAddressFormData? Address { get; set; }
}

public class MemberAddressFormData
{
    public string? StreetName { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; } = "Sverige";
}