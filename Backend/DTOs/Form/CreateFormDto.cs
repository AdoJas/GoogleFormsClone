using GoogleFormsClone.DTOs.File;

namespace GoogleFormsClone.DTOs.Forms;

public class CreateFormDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? HeaderImageId { get; set; }
    public bool IsActive { get; set; } = true;
    public FormSettingsDto Settings { get; set; } = new();
    public AccessControlDto AccessControl { get; set; } = new();
    public List<QuestionDto> Questions { get; set; } = new();
}

public class FormSettingsDto
{
    public bool ShowProgress { get; set; }
    public bool CollectEmails { get; set; }
    public bool OneResponsePerUser { get; set; }
    public bool AllowResponseEditing { get; set; }
    public int ResponseEditingDuration { get; set; }
    public string ConfirmationMessage { get; set; } = string.Empty;
}

public class AccessControlDto
{
    public bool IsPublic { get; set; }
    public bool RequirePassword { get; set; }
    public string? AccessPassword { get; set; } = string.Empty;
}

public class QuestionDto
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool Required { get; set; }
    public bool AllowMultipleSelection { get; set; }
    public bool OneChoice { get; set; }
    public List<OptionDto>? Options { get; set; }
    public LinearScaleDto? LinearScale { get; set; }
    public ValidationDto? Validation { get; set; }
    public LogicDto? Logic { get; set; }
    public AppearanceDto? Appearance { get; set; }
}

public class OptionDto
{
    public string Id { get; set; } = string.Empty; 
    public string Text { get; set; } = string.Empty;
    public bool AllowsCustomText { get; set; }
    public int OrderIndex { get; set; } 
}

public class LinearScaleDto
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public string? MinLabel { get; set; }
    public string? MaxLabel { get; set; }
}

public class ValidationDto
{
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string? Pattern { get; set; }
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public List<string>? FileTypes { get; set; }
    public int? MaxFileSize { get; set; }
    public int? MaxFileCount { get; set; }
}

public class LogicDto
{
    public string? DependsOn { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}

public class AppearanceDto
{
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? ImageUrl { get; set; }
}

public class FormWithFilesDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public FileAttachmentDto? HeaderImage { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public FormSettingsDto Settings { get; set; } = new();
    public AccessControlDto AccessControl { get; set; } = new();
    public bool IsActive { get; set; }
    public List<QuestionWithFileDto> Questions { get; set; } = new();
}

public class QuestionWithAttachmentDto : QuestionDto
{
    public FileResourceDto? Attachment { get; set; }
}

public class DashboardFormDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int ResponseCount { get; set; }
}

public class QuestionWithFileDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Required { get; set; }
    public bool AllowMultipleSelection { get; set; }
    public List<OptionDto>? Options { get; set; }
    public FileAttachmentDto? Attachment { get; set; }
}
