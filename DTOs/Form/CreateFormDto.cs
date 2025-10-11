namespace GoogleFormsClone.DTOs.Forms;

public class CreateFormDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<QuestionDto> Questions { get; set; } = new();
}

public class QuestionDto
{
    public string Text { get; set; } = string.Empty; 
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool Required { get; set; } = false;
    public List<OptionDto>? Options { get; set; }
    public bool OneChoice { get; set; } = false;
    public LinearScaleDto? LinearScale { get; set; }
    public ValidationDto? Validation { get; set; }
    public LogicDto? Logic { get; set; }
    public AppearanceDto? Appearance { get; set; }
}

public class OptionDto
{
    public string Text { get; set; } = string.Empty;
    public bool AllowsCustomText { get; set; } = false;
}

public class LinearScaleDto
{
    public int MinValue { get; set; } = 0;
    public int MaxValue { get; set; } = 5;
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