using GoogleFormsClone.Models;
using MongoDB.Driver;

namespace GoogleFormsClone.Services;

public class UserService
{
    private readonly IMongoCollection<User> _users;

    public UserService(MongoDbService db)
    {
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

    public async Task<User?> UpdateUserAsync(string id, User updatedUser)
    {
        updatedUser.UpdatedAt = DateTime.UtcNow;
        var result = await _users.ReplaceOneAsync(u => u.Id == id, updatedUser);
        if (result.MatchedCount == 0)
            return null;

        return updatedUser;
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }
}