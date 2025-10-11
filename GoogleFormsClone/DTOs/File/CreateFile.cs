namespace GoogleFormsClone.DTOs.File;

public class CreateFileDto
{
    public string OriginalName { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public long FileSize { get; set; }
    public string UploadUrl { get; set; } = null!;
    public string UploadedBy { get; set; } = null!;
    public string? AssociatedWith { get; set; }
}