using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GoogleFormsClone.Models;

public class Response
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("formId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string FormId { get; set; } = string.Empty;

    [BsonElement("submittedBy")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? SubmittedBy { get; set; }

    [BsonElement("submittedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("metadata")]
    public ResponseMetadata Metadata { get; set; } = new();

    [BsonElement("status")]
    public string Status { get; set; } = "submitted";

    [BsonElement("answers")]
    public List<Answer> Answers { get; set; } = new();

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class ResponseMetadata
{
    [BsonElement("ipAddress")]
    public string IpAddress { get; set; } = string.Empty;

    [BsonElement("userAgent")]
    public string UserAgent { get; set; } = string.Empty;

    [BsonElement("durationSeconds")]
    public double DurationSeconds { get; set; } = 0;

    [BsonElement("language")]
    public string Language { get; set; } = string.Empty;

    [BsonElement("referrer")]
    public string Referrer { get; set; } = string.Empty;
}

public class Answer
{
    [BsonElement("questionId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string QuestionId { get; set; } = string.Empty;

    [BsonElement("answerText")]
    public string? AnswerText { get; set; }

    [BsonElement("selectedOptions")]
    public List<string>? SelectedOptions { get; set; }

    [BsonElement("linearScaleValue")]
    public int? LinearScaleValue { get; set; }

    [BsonElement("fileUpload")]
    public FileUpload? FileUpload { get; set; }

    [BsonElement("questionSnapshot")]
    public QuestionSnapshot QuestionSnapshot { get; set; } = new();
}

public class FileUpload
{
    [BsonElement("fileUrl")]
    public string FileUrl { get; set; } = string.Empty;

    [BsonElement("fileName")]
    public string FileName { get; set; } = string.Empty;

    [BsonElement("fileSize")]
    public long FileSize { get; set; } = 0;

    [BsonElement("mimeType")]
    public string MimeType { get; set; } = string.Empty;
}

public class QuestionSnapshot
{
    [BsonElement("questionText")]
    public string QuestionText { get; set; } = string.Empty;

    [BsonElement("questionType")]
    public string QuestionType { get; set; } = string.Empty;

    [BsonElement("options")]
    public List<QuestionOption> Options { get; set; } = new();
}

