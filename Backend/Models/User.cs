using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GoogleFormsClone.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("email")]
    public string Email { get; set; } = null!;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("avatarUrl")]
    public string AvatarUrl { get; set; } = null!;

    [BsonElement("role")]
    public string Role { get; set; } = "user"; // user, admin

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("preferences")]
    public UserPreferences Preferences { get; set; } = new();
    
    [BsonElement("stats")]
    public UserStats Stats { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class UserPreferences
{
    [BsonElement("emailNotifications")]
    public bool EmailNotifications { get; set; } = true;

    [BsonElement("theme")]
    public string Theme { get; set; } = "light"; // light, dark, auto

    [BsonElement("language")]
    public string Language { get; set; } = "en";
}

public class UserStats
{
    [BsonElement("formsCreated")]
    public int FormsCreated { get; set; } = 0;

    [BsonElement("responsesReceived")]
    public int ResponsesReceived { get; set; } = 0;

    [BsonElement("totalStorageUsed")]
    public long TotalStorageUsed { get; set; } = 0;
}
