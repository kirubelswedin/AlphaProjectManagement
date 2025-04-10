namespace ASP.ViewModels.Components;

public class MemberCardViewModel
{
    public string Id { get; set; } = null!;
    public string? Avatar { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string StreetName { get; set; } = null!;
    public bool IsAdmin { get; set; }
}