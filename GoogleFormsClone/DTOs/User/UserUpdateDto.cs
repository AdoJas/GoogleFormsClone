namespace GoogleFormsClone.DTOs.User
{
    public class UserUpdateDto
    {
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }

        public UserPreferencesDto? Preferences { get; set; }
    }

    public class UserPreferencesDto
    {
        public bool? EmailNotifications { get; set; }
        public string? Theme { get; set; }
        public string? Language { get; set; }
    }
}