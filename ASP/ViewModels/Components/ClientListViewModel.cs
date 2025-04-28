namespace ASP.ViewModels.Components;

public class ClientListViewModel
{
    public string Id { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? ClientName { get; set; }
    public string? ContactPerson { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StreetAddress { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    
    public ClientDropdownViewModel Dropdown => new() { Id = Id };
}

public class ClientDropdownViewModel
{
    public string Id { get; set; } = null!;
}