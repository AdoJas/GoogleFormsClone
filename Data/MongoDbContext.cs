using GoogleFormsClone.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<Form> Forms => _database.GetCollection<Form>("forms");
    public IMongoCollection<Response> Responses => _database.GetCollection<Response>("responses");
    public IMongoCollection<FileResource> Files => _database.GetCollection<FileResource>("files");
}