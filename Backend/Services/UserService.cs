using GoogleFormsClone.Models;
using GoogleFormsClone.DTOs.User;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GoogleFormsClone.Services;

public class UserService
{
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<Form> _forms;
    private readonly IMongoCollection<FileResource> _files;
    private readonly IMongoCollection<RefreshToken> _refreshTokens;

    public UserService(MongoDbService db)
    {
        _users = db.GetCollection<User>("Users");
        _forms = db.GetCollection<Form>("Forms");
        _files = db.GetCollection<FileResource>("Files");
        _refreshTokens = db.GetCollection<RefreshToken>("RefreshTokens");
    }

    // ---------------- GET ALL USERS ----------------
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _users.Find(_ => true).ToListAsync();
        return users.Select(u => MapToUserDto(u)).ToList();
    }

    // ---------------- GET USER BY ID ----------------
    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        if (user == null) return null;
        return MapToUserDto(user);
    }

    // ---------------- CREATE USER ----------------
    public async Task<UserDto> CreateUserAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await _users.InsertOneAsync(user);
        return MapToUserDto(user);
    }

    // ---------------- UPDATE USER ----------------
    public async Task<UserDto?> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        if (user == null) return null;

        var updateDefs = new List<UpdateDefinition<User>>();
        var update = Builders<User>.Update;

        if (!string.IsNullOrWhiteSpace(dto.Name))
            updateDefs.Add(update.Set(u => u.Name, dto.Name));

        if (dto.AvatarUrl != null)
            updateDefs.Add(update.Set(u => u.AvatarUrl, dto.AvatarUrl));

        if (dto.Preferences != null)
        {
            if (dto.Preferences.EmailNotifications.HasValue)
                updateDefs.Add(update.Set(u => u.Preferences.EmailNotifications, dto.Preferences.EmailNotifications.Value));

            if (!string.IsNullOrWhiteSpace(dto.Preferences.Theme))
                updateDefs.Add(update.Set(u => u.Preferences.Theme, dto.Preferences.Theme));

            if (!string.IsNullOrWhiteSpace(dto.Preferences.Language))
                updateDefs.Add(update.Set(u => u.Preferences.Language, dto.Preferences.Language));
        }

        updateDefs.Add(update.Set(u => u.UpdatedAt, DateTime.UtcNow));

        var combinedUpdate = update.Combine(updateDefs);

        var updatedUser = await _users.FindOneAndUpdateAsync(
            u => u.Id == id,
            combinedUpdate,
            new FindOneAndUpdateOptions<User>
            {
                ReturnDocument = ReturnDocument.After
            }
        );

        if (updatedUser == null) return null;

        var dtoResult = MapToUserDto(updatedUser);
        dtoResult.Stats = MapToUserStatsDto(updatedUser.Stats);

        return dtoResult;
    }

    // ---------------- DELETE USER AND ASSOCIATED DATA ----------------
    public async Task<bool> DeleteUserAndDataAsync(string userId)
    {
        var userForms = await _forms.Find(f => f.CreatedBy == userId).ToListAsync();
        foreach (var form in userForms)
            await _files.DeleteManyAsync(f => f.AssociatedWith == form.Id);

        await _forms.DeleteManyAsync(f => f.CreatedBy == userId);
        await _files.DeleteManyAsync(f => f.UploadedBy == userId);
        await _refreshTokens.DeleteManyAsync(rt => rt.UserId == userId);

        var result = await _users.DeleteOneAsync(u => u.Id == userId);
        return result.DeletedCount > 0;
    }

    // ---------------- MAP TO DTO ----------------
    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            AvatarUrl = user.AvatarUrl,
            Preferences = user.Preferences != null
                ? new UserPreferencesDto
                {
                    EmailNotifications = user.Preferences.EmailNotifications,
                    Theme = user.Preferences.Theme,
                    Language = user.Preferences.Language
                }
                : null,
            CreatedAt = user.CreatedAt
        };
    }
    private static UserStatsDto MapToUserStatsDto(UserStats stats)
    {
        if (stats == null) return new UserStatsDto();
        return new UserStatsDto
        {
            TotalForms = stats.FormsCreated,
            TotalFiles = stats.ResponsesReceived,
            TotalFileSize = stats.TotalStorageUsed
        };
    }
}
