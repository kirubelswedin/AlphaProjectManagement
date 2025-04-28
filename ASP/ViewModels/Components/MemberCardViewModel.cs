namespace ASP.ViewModels.Components;

public class MemberCardViewModel
{
    public string Id { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; } 
    public string? JobTitle { get; set; }
    public string? StreetAddress { get; set; } = null!;
    public string? City { get; set; } = null!;
    public string? PostalCode { get; set; } = null!;
    public string FullAddress => $"{StreetAddress}, {PostalCode}, {City}";
    public bool IsAdmin { get; set; }
    
    public MemberDropdownViewModel Dropdown => new() { Id = Id, IsAdmin = IsAdmin };
}

public class MemberDropdownViewModel
{
    public string Id { get; set; } = null!;
    public bool IsAdmin { get; set; }
}

