using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GoogleFormsClone.Models;

public class Response
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string FormId { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? SubmittedBy { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public ResponseMetadata Metadata { get; set; } = new();
    public string Status { get; set; } = "submitted";
    public List<Answer> Answers { get; set; } = new();

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class ResponseMetadata
{
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public double DurationSeconds { get; set; } = 0;
    public string Language { get; set; } = string.Empty;
    public string Referrer { get; set; } = string.Empty;
}

public class Answer
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string QuestionId { get; set; } = string.Empty;
    public string? AnswerText { get; set; }
    public List<string>? SelectedOptions { get; set; }
    public int? LinearScaleValue { get; set; }
    public FileUpload? FileUpload { get; set; }
    public QuestionSnapshot QuestionSnapshot { get; set; } = new();
}

public class FileUpload
{
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; } = 0;
    public string MimeType { get; set; } = string.Empty;
}

public class QuestionSnapshot
{
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<QuestionOption> Options { get; set; } = new();
}
