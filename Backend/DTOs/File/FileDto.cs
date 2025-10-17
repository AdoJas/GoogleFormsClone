namespace GoogleFormsClone.DTOs.File;
using System.ComponentModel.DataAnnotations;

public class CreateFileDto
{
    public string OriginalName { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public long FileSize { get; set; }
    public string? AssociatedWith { get; set; }
    public string UploadedBy { get; set; } = null!;
}

public class UploadFileRequest
{
    public IFormFile File { get; set; } = null!;
    public string? AssociatedWith { get; set; }
    public string? AssociatedEntityType { get; set; } // e.g., "Form", "User", "Question"
}

public class UpdateFileDto
{
    public string? OriginalName { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public string? AssociatedWith { get; set; }
    public string? AssociatedEntityType { get; set; } // "Form", "Question", "User"
    public string? UploadUrl { get; set; }
}

public class FileResourceDto
{
    public string Id { get; set; } = null!;
    public string OriginalName { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public long FileSize { get; set; }
}

public class UserFileCount
{
    public string UserId { get; set; } = null!;
    public int FileCount { get; set; }
}

public class UserStorageUsage
{
    public string UserId { get; set; } = null!;
    public long TotalStorage { get; set; }
}