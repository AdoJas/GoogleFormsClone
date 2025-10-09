using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GoogleFormsClone.Models;

public class Form
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string CreatedBy { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public FormSettings Settings { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public AccessControl AccessControl { get; set; } = new();
    public int Version { get; set; } = 1;
    public List<Question> Questions { get; set; } = new();

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class FormSettings
{
    public bool AllowEditing { get; set; } = false;
    public bool RequireLogin { get; set; } = false;
    public bool OneResponsePerUser { get; set; } = false;
    public bool ShowProgress { get; set; } = false;
    public string ConfirmationMessage { get; set; } = string.Empty;
    public bool CollectEmails { get; set; } = false;
    public bool AllowResponseEditing { get; set; } = false;
    public int ResponseEditingDuration { get; set; } = 0;
}

public class AccessControl
{
    public bool IsPublic { get; set; } = false;
    public bool RequirePassword { get; set; } = false;
    public string AccessPassword { get; set; } = string.Empty;
}

public class Question
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string Type { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Required { get; set; } = false;

    public List<QuestionOption> Options { get; set; } = new();
    public LinearScale? LinearScale { get; set; }
    public QuestionValidation? Validation { get; set; }
    public int OrderIndex { get; set; } = 0;
    public QuestionLogic? Logic { get; set; }
    public QuestionAppearance? Appearance { get; set; }
}

public class QuestionOption
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Text { get; set; } = string.Empty;
    public int OrderIndex { get; set; } = 0;
    public bool AllowsCustomText { get; set; } = false;
}

public class LinearScale
{
    public int MinValue { get; set; } = 0;
    public int MaxValue { get; set; } = 5;
    public string MinLabel { get; set; } = string.Empty;
    public string MaxLabel { get; set; } = string.Empty;
}

public class QuestionValidation
{
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string Pattern { get; set; } = string.Empty;
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public List<string>? FileTypes { get; set; }
    public int? MaxFileSize { get; set; }
    public int? MaxFileCount { get; set; }
}

public class QuestionLogic
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string? DependsOn { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}

public class QuestionAppearance
{
    public string Placeholder { get; set; } = string.Empty;
    public string HelpText { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
