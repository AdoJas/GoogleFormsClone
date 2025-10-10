using GoogleFormsClone.Models;
using MongoDB.Driver;

namespace GoogleFormsClone.Services;

public class FileService
{
    private readonly IMongoCollection<FileResource> _files;

    public FileService(MongoDbService db)
    {
        _files = db.GetCollection<FileResource>("Files");
    }

    public async Task<List<FileResource>> GetAllFilesAsync()
    {
        return await _files.Find(f => true).ToListAsync();
    }

    public async Task<FileResource?> GetFileByIdAsync(string id)
    {
        return await _files.Find(f => f.Id == id).FirstOrDefaultAsync();
    }

    public async Task<FileResource> CreateFileAsync(FileResource file)
    {
        file.CreatedAt = DateTime.UtcNow;
        file.UpdatedAt = DateTime.UtcNow;
        await _files.InsertOneAsync(file);
        return file;
    }

    public async Task<bool> UpdateFileAsync(string id, FileResource updatedFile)
    {
        updatedFile.UpdatedAt = DateTime.UtcNow;
        var result = await _files.ReplaceOneAsync(f => f.Id == id, updatedFile);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteFileAsync(string id)
    {
        var result = await _files.DeleteOneAsync(f => f.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<FileResource>> GetFilesByUploaderIdAsync(string userId)
    {
        return await _files.Find(f => f.UploadedBy == userId).ToListAsync();
    }

    public async Task<List<FileResource>> GetFilesByAssociatedEntityIdAsync(string associatedId)
    {
        return await _files.Find(f => f.AssociatedWith == associatedId).ToListAsync();
    }

    public async Task<List<FileResource>> GetFilesByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _files.Find(f => f.CreatedAt >= from && f.CreatedAt <= to).ToListAsync();
    }

    public async Task<List<FileResource>> GetAllFilesSortedByFieldAsync(string sortBy = "CreatedAt", bool ascending = true)
    {
        var sortDefinition = ascending 
            ? Builders<FileResource>.Sort.Ascending(sortBy) 
            : Builders<FileResource>.Sort.Descending(sortBy);

        return await _files.Find(f => true).Sort(sortDefinition).ToListAsync();
    }

    public async Task<List<UserFileCount>> GetFileCountPerUserAsync()
    {
        var pipeline = _files.Aggregate()
            .Group(f => f.UploadedBy, g => new UserFileCount
            {
                UserId = g.Key,
                FileCount = g.Count()
            });

        return await pipeline.ToListAsync();
    }

    public async Task<List<UserStorageUsage>> GetTotalStorageUsedPerUserAsync()
    {
        var pipeline = _files.Aggregate()
            .Group(f => f.UploadedBy, g => new UserStorageUsage
            {
                UserId = g.Key,
                TotalStorage = g.Sum(f => f.FileSize)
            });

        return await pipeline.ToListAsync();
    }
}

public class UserFileCount
{
    public string UserId { get; set; } = null!;
    public int FileCount { get; set; }
}

public class UserStorageUsage
{
    public string UserId { get; set; } = null!;
    public long TotalStorage { get; set; }
}
