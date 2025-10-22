using GoogleFormsClone.DTOs.File;
using GoogleFormsClone.Models;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Bson;

namespace GoogleFormsClone.Services;

public class FileService
{
    private readonly IMongoCollection<FileResource> _files;
    private readonly GridFSBucket _gridFS;
    private readonly UserService _userService;

    public FileService(MongoDbService db, UserService userService)
    {
        _files = db.GetCollection<FileResource>("Files");
        _gridFS = new GridFSBucket(db.Database, new GridFSBucketOptions
        {
            BucketName = "fileUploads",
            ChunkSizeBytes = 1048576,
            WriteConcern = WriteConcern.WMajority,
            ReadPreference = ReadPreference.Primary
        });
		_userService = userService;
    }

    // ---------------- Upload/Download ----------------
    public async Task<string> UploadFileAsync(
        Stream fileStream, 
        string fileName, 
        string contentType, 
        string uploadedBy, 
        string? associatedWith,
        string? associatedEntityType) // <- new parameter
    {
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
            {
                { "uploadedBy", uploadedBy },
                { "associatedWith", associatedWith != null ? new BsonString(associatedWith) : BsonNull.Value },
                { "associatedEntityType", associatedEntityType != null ? new BsonString(associatedEntityType) : BsonNull.Value },
                { "originalName", fileName },
                { "fileType", contentType },
                { "createdAt", DateTime.UtcNow }
            }
        };

        var fileId = await _gridFS.UploadFromStreamAsync(fileName, fileStream, options);

        var fileResource = new FileResource
        {
            Id = fileId.ToString(),
            OriginalName = fileName,
            FileType = contentType,
            FileSize = fileStream.Length,
            UploadedBy = uploadedBy,
            AssociatedWith = associatedWith,
            AssociatedEntityType = associatedEntityType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _files.InsertOneAsync(fileResource);


        return fileResource.Id;
    }

    public async Task<Stream?> DownloadFileAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId)) return null;

        try
        {
            var memoryStream = new MemoryStream();
            await _gridFS.DownloadToStreamAsync(objectId, memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (GridFSFileNotFoundException)
        {
            return null;
        }
    }
    
    public async Task<FileResource?> GetFileByIdAsync(string id)
    {
        return await _files.Find(f => f.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteFileAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId)) return false;

        await _gridFS.DeleteAsync(objectId);
        var result = await _files.DeleteOneAsync(f => f.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> UpdateFileAsync(string id, FileResource updatedFile)
    {
        updatedFile.UpdatedAt = DateTime.UtcNow;
        var result = await _files.ReplaceOneAsync(f => f.Id == id, updatedFile);
        return result.ModifiedCount > 0;
    }

    // ---------------- List / Filter ----------------
    public async Task<List<FileResource>> GetFilesByUploaderIdAsync(string userId)
    {
        return await _files.Find(f => f.UploadedBy == userId).ToListAsync();
    }

    public async Task<List<FileResource>> GetFilesByAssociatedEntityAsync(string associatedId, string associatedEntityType)
    {
        if (string.IsNullOrEmpty(associatedId) || string.IsNullOrEmpty(associatedEntityType))
            return new List<FileResource>();

        return await _files
            .Find(f => f.AssociatedWith == associatedId && f.AssociatedEntityType == associatedEntityType)
            .ToListAsync();
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

    // ---------------- Aggregates ----------------
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

// ---------------- Helper Classes ----------------
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
