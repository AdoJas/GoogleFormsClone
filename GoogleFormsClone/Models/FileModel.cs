using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GoogleFormsClone.Models;

public class FileResource
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("originalName")]
    public string OriginalName { get; set; } = string.Empty;

    [BsonElement("fileType")]
    public string FileType { get; set; } = string.Empty;

    [BsonElement("fileSize")]
    public long FileSize { get; set; } = 0;

    [BsonElement("uploadUrl")]
    public string UploadUrl { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("uploadedBy")]
    public string UploadedBy { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("associatedWith")]
    public string AssociatedWith { get; set; } = string.Empty;

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}