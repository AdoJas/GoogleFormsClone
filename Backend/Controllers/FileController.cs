using Microsoft.AspNetCore.Mvc;
using GoogleFormsClone.Models;
using GoogleFormsClone.Services;
using GoogleFormsClone.DTOs.File;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace GoogleFormsClone.Controllers;

[ApiController]
[Route("api/file")]
[Authorize]
public class FileController : ControllerBase
{
    private readonly FileService _fileService;

    public FileController(FileService fileService)
    {
        _fileService = fileService;
    }
    

    [HttpGet("{id}")]
    public async Task<ActionResult<FileResourceDto>> GetFileById(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

        var file = await _fileService.GetFileByIdAsync(id);
        
        if (file == null)
            return NotFound();

        var dto = new FileResourceDto
        {
            Id = file.Id,
            OriginalName = file.OriginalName,
            FileType = file.FileType,
            FileSize = file.FileSize,
            UploadUrl = file.UploadUrl
        };
        if (currentUserRole != "Admin" && file.UploadedBy != currentUserId)
            return Forbid("You are not allowed to access this file.");

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<FileResourceDto>> CreateFile([FromBody] CreateFileDto createDto)
    {
        var newFile = new FileResource
        {
            OriginalName = createDto.OriginalName,
            FileType = createDto.FileType,
            FileSize = createDto.FileSize,
            UploadUrl = createDto.UploadUrl,
            UploadedBy = createDto.UploadedBy,
            AssociatedWith = createDto.AssociatedWith,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _fileService.CreateFileAsync(newFile);

        var dto = new FileResourceDto
        {
            Id = newFile.Id,
            OriginalName = newFile.OriginalName,
            FileType = newFile.FileType,
            FileSize = newFile.FileSize,
            UploadUrl = newFile.UploadUrl
        };

        return CreatedAtAction(nameof(GetFileById), new { id = newFile.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFile(string id, [FromBody] UpdateFileDto updateDto)
    {
        var existingFile = await _fileService.GetFileByIdAsync(id);
        if (existingFile == null)
            return NotFound();

        if (updateDto.OriginalName != null) existingFile.OriginalName = updateDto.OriginalName;
        if (updateDto.FileType != null) existingFile.FileType = updateDto.FileType;
        if (updateDto.FileSize.HasValue) existingFile.FileSize = updateDto.FileSize.Value;
        if (updateDto.UploadUrl != null) existingFile.UploadUrl = updateDto.UploadUrl;
        if (updateDto.AssociatedWith != null) existingFile.AssociatedWith = updateDto.AssociatedWith;

        var updated = await _fileService.UpdateFileAsync(id, existingFile);
        return updated ? NoContent() : StatusCode(500, "Failed to update file");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(string id)
    {
        var deleted = await _fileService.DeleteFileAsync(id);
        return deleted ? NoContent() : NotFound();
    }
    
    [HttpGet("uploader/{userId}")]
    public async Task<ActionResult<List<FileResourceDto>>> GetFilesByUploaderId(string userId)
    {
        var files = await _fileService.GetFilesByUploaderIdAsync(userId);
        return Ok(files.Select(ToFileDto));
    }

    [HttpGet("associated/{associatedId}")]
    public async Task<ActionResult<List<FileResourceDto>>> GetFilesByAssociatedEntityId(string associatedId)
    {
        var files = await _fileService.GetFilesByAssociatedEntityIdAsync(associatedId);
        return Ok(files.Select(ToFileDto));
    }

    [HttpGet("daterange")]
    public async Task<ActionResult<List<FileResourceDto>>> GetFilesByDateRange(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var files = await _fileService.GetFilesByDateRangeAsync(from, to);
        return Ok(files.Select(ToFileDto));
    }
    
    [HttpGet("sorted")]
    public async Task<ActionResult<List<FileResourceDto>>> GetFilesSorted(
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] bool ascending = true)
    {
        var files = await _fileService.GetAllFilesSortedByFieldAsync(sortBy, ascending);
        return Ok(files.Select(ToFileDto));
    }
    
    [HttpGet("aggregates/filecount")]
    public async Task<ActionResult<List<UserFileCount>>> GetFileCountPerUser()
    {
        var result = await _fileService.GetFileCountPerUserAsync();
        return Ok(result);
    }

    [HttpGet("aggregates/storageusage")]
    public async Task<ActionResult<List<UserStorageUsage>>> GetTotalStorageUsedPerUser()
    {
        var result = await _fileService.GetTotalStorageUsedPerUserAsync();
        return Ok(result);
    }
    
    private static FileResourceDto ToFileDto(FileResource f)
    {
        return new FileResourceDto
        {
            Id = f.Id,
            OriginalName = f.OriginalName,
            FileType = f.FileType,
            FileSize = f.FileSize,
            UploadUrl = f.UploadUrl
        };
    }
}
