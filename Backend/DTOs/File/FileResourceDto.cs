namespace GoogleFormsClone.DTOs.File;

public class FileResourceDto
{
    public string Id { get; set; } = null!;
    public string OriginalName { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public long FileSize { get; set; }
    public string UploadUrl { get; set; } = null!;
}