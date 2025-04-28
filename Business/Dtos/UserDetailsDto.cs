namespace Business.Dtos;

public class UserDetailsDto
{
  public string Id { get; set; } = null!;
  public string? ImageUrl { get; set; }
  public string FullName => $"{FirstName} {LastName}";
  public string? FirstName { get; set; } 
  public string? LastName { get; set; }
  public string? Email { get; set; } 
  public string? PhoneNumber { get; set; } 
  public string? JobTitle { get; set; } 
  public string StreetAddress { get; set; } = null!;
  public string City { get; set; } = null!;
  public string PostalCode { get; set; } = null!;
  public string FullAddress => $"{StreetAddress}, {PostalCode}, {City}";
  public bool IsAdmin { get; set; }
}