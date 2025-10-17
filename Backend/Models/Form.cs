using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GoogleFormsClone.Models;

public class Form
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("createdBy")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CreatedBy { get; set; } = null!;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("settings")]
    public FormSettings Settings { get; set; } = new();

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("accessControl")]
    public AccessControl AccessControl { get; set; } = new();

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
    [BsonElement("allowEditing")]
    public bool AllowEditing { get; set; } = false;

    [BsonElement("oneResponsePerUser")]
    public bool OneResponsePerUser { get; set; } = false;

    [BsonElement("showProgress")]
    public bool ShowProgress { get; set; } = false;

    [BsonElement("confirmationMessage")]
    public string ConfirmationMessage { get; set; } = string.Empty;

    [BsonElement("collectEmails")]
    public bool CollectEmails { get; set; } = false;

    [BsonElement("allowResponseEditing")]
    public bool AllowResponseEditing { get; set; } = false;

    [BsonElement("responseEditingDuration")]
    public int ResponseEditingDuration { get; set; } = 0;
}

public class AccessControl
{
    [BsonElement("isPublic")]
    public bool IsPublic { get; set; } = false;

    [BsonElement("requirePassword")]
    public bool RequirePassword { get; set; } = false;

    [BsonElement("accessPassword")]
    public string AccessPassword { get; set; } = string.Empty;
}

public class Question
{
    [BsonElement("id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("questionText")]
    public string QuestionText { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("required")]
    public bool Required { get; set; } = false;

    [BsonElement("allowMultipleSelection")]
    public bool AllowMultipleSelection { get; set; } = true;

    [BsonElement("options")]
    public List<QuestionOption> Options { get; set; } = new();

    [BsonElement("linearScale")]
    public LinearScale? LinearScale { get; set; }

    [BsonElement("validation")]
    public QuestionValidation? Validation { get; set; }

    [BsonElement("orderIndex")]
    public int OrderIndex { get; set; } = 0;

    [BsonElement("logic")]
    public QuestionLogic? Logic { get; set; }

    [BsonElement("appearance")]
    public QuestionAppearance? Appearance { get; set; }
}

public class QuestionOption
{
    [BsonElement("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("text")]
    public string Text { get; set; } = string.Empty;

    [BsonElement("orderIndex")]
    public int OrderIndex { get; set; } = 0;

    [BsonElement("allowsCustomText")]
    public bool AllowsCustomText { get; set; } = false;
}

public class LinearScale
{
    [BsonElement("minValue")]
    public int MinValue { get; set; } = 0;

    [BsonElement("maxValue")]
    public int MaxValue { get; set; } = 5;

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
    [BsonElement("dependsOn")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string DependsOn { get; set; } = null!;

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
