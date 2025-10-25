namespace GoogleFormsClone.DTOs.Auth;

public class RegisterUserDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PasswordConfirm { get; set; } = null!;
    public string? Name { get; set; }
}