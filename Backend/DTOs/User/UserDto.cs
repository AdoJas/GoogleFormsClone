namespace GoogleFormsClone.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Role { get; set; } = "user";
        public string? AvatarUrl { get; set; }

        public UserPreferencesDto? Preferences { get; set; }
        public UserStatsDto? Stats { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; } 
        public UserPreferencesDto? Preferences { get; set; }
        public UserStatsDto? Stats { get; set; }
    }

    public class UserPreferencesDto
    {
        public bool? EmailNotifications { get; set; }
        public string? Theme { get; set; }
        public string? Language { get; set; }
    }

    public class UserStatsDto
    {
        public int TotalForms { get; set; }
        public int TotalFiles { get; set; }
        public long TotalFileSize { get; set; } // bytes
    }

    public class UserPasswordUpdateDto
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}