namespace GoogleFormsClone.Models;

public class JwtSettings
{
    public string SecretKey { get; set; } = null!;
    public int ExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 1;
    public string Issuer { get; set; } = "GoogleFormsClone";
    public string Audience { get; set; } = "GoogleFormsCloneUsers";
}