namespace GoogleFormsClone.DTOs.File;

public class UpdateFileDto
{
    public string? OriginalName { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public string? UploadUrl { get; set; }
    public string? AssociatedWith { get; set; }
}