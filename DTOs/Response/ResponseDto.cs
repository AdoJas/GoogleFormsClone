namespace GoogleFormsClone.DTOs.Response;

public class SubmitResponseDto
{
    public string FormId { get; set; } = string.Empty;
    public List<AnswerDto> Answers { get; set; } = new();
    public double DurationSeconds { get; set; } = 0;
}

public class AnswerDto
{
    public string QuestionId { get; set; } = string.Empty;
    public string? AnswerText { get; set; }
    public List<string>? SelectedOptions { get; set; }
    public int? LinearScaleValue { get; set; }
    public FileUploadDto? FileUpload { get; set; }
}

public class FileUploadDto
{
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; } = 0;
    public string MimeType { get; set; } = string.Empty;
}