namespace GoogleFormsClone.DTOs.User;

public class UserDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Role { get; set; } = "user";
    public string? AvatarUrl { get; set; }
}