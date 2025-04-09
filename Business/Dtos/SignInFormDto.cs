namespace Business.Dtos;

public class SignInFormDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsPersistent { get; set; }
}