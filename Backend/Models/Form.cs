using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GoogleFormsClone.Models;

public class Form
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("createdBy")]
    public string CreatedBy { get; set; } = null!;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
    
    [BsonElement("headerImageId")]
    public string? HeaderImageId { get; set; }

    [BsonElement("settings")]
    public FormSettings Settings { get; set; } = new();

    [BsonElement("accessControl")]
    public AccessControl AccessControl { get; set; } = new();

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("version")]
    public int Version { get; set; } = 1;

    [BsonElement("questions")]
    public List<Question> Questions { get; set; } = new();

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class FormSettings
{
    [BsonElement("showProgress")]
    public bool ShowProgress { get; set; }

    [BsonElement("collectEmails")]
    public bool CollectEmails { get; set; }

    [BsonElement("oneResponsePerUser")]
    public bool OneResponsePerUser { get; set; }

    [BsonElement("allowResponseEditing")]
    public bool AllowResponseEditing { get; set; }

    [BsonElement("responseEditingDuration")]
    public int ResponseEditingDuration { get; set; }

    [BsonElement("confirmationMessage")]
    public string ConfirmationMessage { get; set; } = string.Empty;
}

public class AccessControl
{
    [BsonElement("isPublic")]
    public bool IsPublic { get; set; }

    [BsonElement("requirePassword")]
    public bool RequirePassword { get; set; }

    [BsonElement("accessPassword")]
    public string AccessPassword { get; set; } = string.Empty;
}

[BsonNoId]
[BsonIgnoreExtraElements]
public class Question
{
    [BsonElement("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("questionText")]
    public string QuestionText { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("required")]
    public bool Required { get; set; }

    [BsonElement("allowMultipleSelection")]
    public bool AllowMultipleSelection { get; set; }

    [BsonElement("options")]
    public List<QuestionOption>? Options { get; set; } = new();

    [BsonElement("linearScale")]
    public LinearScale? LinearScale { get; set; }

    [BsonElement("validation")]
    public QuestionValidation? Validation { get; set; }

    [BsonElement("logic")]
    public QuestionLogic? Logic { get; set; }

    [BsonElement("appearance")]
    public QuestionAppearance? Appearance { get; set; }

    [BsonElement("orderIndex")]
    public int OrderIndex { get; set; }

    [BsonIgnoreIfNull]
    [BsonElement("attachment")]
    public FileResource? Attachment { get; set; }
}

[BsonNoId]
[BsonIgnoreExtraElements]
public class QuestionOption
{
    [BsonElement("id")]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("text")]
    public string Text { get; set; } = string.Empty;

    [BsonElement("orderIndex")]
    public int OrderIndex { get; set; }

    [BsonElement("allowsCustomText")]
    public bool AllowsCustomText { get; set; }
}

public class LinearScale
{
    [BsonElement("minValue")]
    public int MinValue { get; set; }

    [BsonElement("maxValue")]
    public int MaxValue { get; set; }

    [BsonElement("minLabel")]
    public string MinLabel { get; set; } = string.Empty;

    [BsonElement("maxLabel")]
    public string MaxLabel { get; set; } = string.Empty;
}

public class QuestionValidation
{
    [BsonElement("minLength")]
    public int? MinLength { get; set; }

    [BsonElement("maxLength")]
    public int? MaxLength { get; set; }

    [BsonElement("pattern")]
    public string Pattern { get; set; } = string.Empty;

    [BsonElement("minValue")]
    public int? MinValue { get; set; }

    [BsonElement("maxValue")]
    public int? MaxValue { get; set; }

    [BsonElement("fileTypes")]
    public List<string>? FileTypes { get; set; }

    [BsonElement("maxFileSize")]
    public int? MaxFileSize { get; set; }

    [BsonElement("maxFileCount")]
    public int? MaxFileCount { get; set; }
}

public class QuestionLogic
{
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("dependsOn")]
    public string? DependsOn { get; set; }

    [BsonElement("condition")]
    public string Condition { get; set; } = string.Empty;

    [BsonElement("value")]
    public string Value { get; set; } = string.Empty;

    [BsonElement("action")]
    public string Action { get; set; } = string.Empty;
}

public class QuestionAppearance
{
    [BsonElement("placeholder")]
    public string Placeholder { get; set; } = string.Empty;

    [BsonElement("helpText")]
    public string HelpText { get; set; } = string.Empty;

    [BsonElement("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;
}
