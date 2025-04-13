namespace ASP.ViewModels.Components;

public class MemberCardViewModel
{
    public string Id { get; set; } = null!;
    public string? Avatar { get; set; }
    public string? FirstName { get; set; } = null!;
    public string? LastName { get; set; } = null!;
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
    public string? JobTitle { get; set; } = null!;
    public string StreetAddress { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string FullAddress => $"{StreetAddress}, {PostalCode}, {City}";
    public bool IsAdmin { get; set; }

    public MemberDropdownViewModel Dropdown => new() { Id = Id, IsAdmin = IsAdmin };
}

public class MemberDropdownViewModel
{
    public string Id { get; set; } = null!;
    public bool IsAdmin { get; set; }
}

public class MemberItemViewModel
{
    public string Id { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public string Text { get; set; } = null!;
    public bool IsChecked { get; set; }
}
