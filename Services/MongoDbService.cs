using GoogleFormsClone.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GoogleFormsClone.Services;

public sealed class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IOptions<MongoDbSettings> options)
    {
        if (options?.Value == null) 
            throw new ArgumentNullException(nameof(options), "MongoDbSettings is not configured.");

        var client = new MongoClient(options.Value.ConnectionString);
        _database = client.GetDatabase(options.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Collection name cannot be null or empty.", nameof(name));

        return _database.GetCollection<T>(name);
    }
}