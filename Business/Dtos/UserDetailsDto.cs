namespace Business.Dtos;

public class UserDetailsDto
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
}