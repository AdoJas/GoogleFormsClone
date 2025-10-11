using GoogleFormsClone.Models;
using MongoDB.Driver;
using GoogleFormsClone.DTOs.User;

namespace GoogleFormsClone.Services;

public class UserService
{
    private readonly IMongoCollection<User> _users;
    private readonly MongoDbService _db;

    public UserService(MongoDbService db)
    {
        _db = db;
        _users = db.GetCollection<User>("Users");
    }

    public async Task<List<User>> GetAllUsersAsync() =>
        await _users.Find(_ => true).ToListAsync();

    public async Task<User?> GetUserByIdAsync(string id) =>
        await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<User> CreateUserAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task<User?> UpdateUserAsync(string id, UserUpdateDto dto)
    {
        var update = Builders<User>.Update;
        var updates = new List<UpdateDefinition<User>>();

        if (!string.IsNullOrWhiteSpace(dto.Name))
            updates.Add(update.Set(u => u.Name, dto.Name));

        if (!string.IsNullOrWhiteSpace(dto.AvatarUrl))
            updates.Add(update.Set(u => u.AvatarUrl, dto.AvatarUrl));

        if (dto.Preferences != null)
        {
            if (dto.Preferences.EmailNotifications.HasValue)
                updates.Add(update.Set(u => u.Preferences.EmailNotifications, dto.Preferences.EmailNotifications.Value));

            if (!string.IsNullOrWhiteSpace(dto.Preferences.Theme))
                updates.Add(update.Set(u => u.Preferences.Theme, dto.Preferences.Theme));

            if (!string.IsNullOrWhiteSpace(dto.Preferences.Language))
                updates.Add(update.Set(u => u.Preferences.Language, dto.Preferences.Language));
        }

        updates.Add(update.Set(u => u.UpdatedAt, DateTime.UtcNow));

        var combinedUpdate = update.Combine(updates);

        var result = await _users.FindOneAndUpdateAsync(u => u.Id == id, combinedUpdate, new FindOneAndUpdateOptions<User>
        {
            ReturnDocument = ReturnDocument.After
        });

        return result;
    }
    
    public async Task<bool> DeleteUserAndDataAsync(string userId)
    {
        var formsCollection = _db.GetCollection<Form>("Forms");
        var userForms = await formsCollection.Find(f => f.CreatedBy == userId).ToListAsync();

        var filesCollection = _db.GetCollection<FileResource>("Files");
        foreach (var form in userForms)
        {
            await filesCollection.DeleteManyAsync(f => f.AssociatedWith == form.Id);
        }

        await formsCollection.DeleteManyAsync(f => f.CreatedBy == userId);

        await filesCollection.DeleteManyAsync(f => f.UploadedBy == userId);

        var refreshTokensCol = _db.GetCollection<RefreshToken>("RefreshTokens");
        await refreshTokensCol.DeleteManyAsync(rt => rt.UserId == userId);

        var result = await _users.DeleteOneAsync(u => u.Id == userId);

        return result.DeletedCount > 0;
    }

}