using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoogleFormsClone.Services;
using GoogleFormsClone.DTOs.File;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace GoogleFormsClone.Controllers;

[ApiController]
[Route("api/files")]
[Authorize]
public class FileController : ControllerBase
{
    private readonly FileService _fileService;
    private readonly UserService _userService;

    public FileController(FileService fileService, UserService userService)
    {
        _fileService = fileService;
        _userService = userService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("File is missing.");

        var currentUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        try
        {
            using var stream = request.File.OpenReadStream();

            var fileId = await _fileService.UploadFileAsync(
                stream,
                request.File.FileName,
                request.File.ContentType,
                currentUserId,
                request.AssociatedWith,
                request.AssociatedEntityType
            );

            return Ok(new { fileId });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"File upload failed: {ex}");
            return StatusCode(500, "An error occurred while uploading the file.");
        }
    }

    // -------------------
    // Download a file
    // -------------------
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadFile(string id)
    {
        var file = await _fileService.GetFileByIdAsync(id);
        if (file == null)
            return NotFound();

        try
        {
            var stream = await _fileService.DownloadFileAsync(id);
            if (stream == null)
                return NotFound();

            return new FileStreamResult(stream, file.FileType ?? "application/octet-stream")
            {
                FileDownloadName = file.OriginalName ?? "file",
                EnableRangeProcessing = true
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to download file {id}: {ex}");
            return StatusCode(500, "File download failed");
        }
    }

    // -------------------
    // List files by uploader
    // -------------------
    [HttpGet("uploader/{userId}")]
    public async Task<IActionResult> GetFilesByUploader(string userId)
    {
        var files = await _fileService.GetFilesByUploaderIdAsync(userId);
        return Ok(files);
    }

    // -------------------
    // List files by associated entity
    // -------------------
    [HttpGet("associated")]
    public async Task<IActionResult> GetFilesByAssociatedEntity([FromQuery] string associatedId, [FromQuery] string associatedEntityType)
    {
        if (string.IsNullOrEmpty(associatedId) || string.IsNullOrEmpty(associatedEntityType))
            return BadRequest("Both associatedId and associatedEntityType are required.");

        var files = await _fileService.GetFilesByAssociatedEntityAsync(associatedId, associatedEntityType);
        return Ok(files);
    }

    // -------------------
    // Aggregates
    // -------------------
    [HttpGet("aggregates/filecount")]
    public async Task<IActionResult> GetFileCountPerUser()
    {
        var result = await _fileService.GetFileCountPerUserAsync();
        return Ok(result);
    }

    [HttpGet("aggregates/storageusage")]
    public async Task<IActionResult> GetTotalStorageUsedPerUser()
    {
        var result = await _fileService.GetTotalStorageUsedPerUserAsync();
        return Ok(result);
    }

    // -------------------
    // Update file metadata
    // -------------------
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFile(string id, [FromBody] UpdateFileDto dto)
    {
        var file = await _fileService.GetFileByIdAsync(id);
        if (file == null)
            return NotFound();

        if (dto.OriginalName != null) file.OriginalName = dto.OriginalName;
        if (dto.FileType != null) file.FileType = dto.FileType;
        if (dto.FileSize.HasValue) file.FileSize = dto.FileSize.Value;

        if (dto.AssociatedWith != null) file.AssociatedWith = dto.AssociatedWith;
        if (dto.AssociatedEntityType != null) file.AssociatedEntityType = dto.AssociatedEntityType;

        var updated = await _fileService.UpdateFileAsync(id, file);
        return updated ? NoContent() : StatusCode(500, "Failed to update file");
    }

    // -------------------
    // Delete a file
    // -------------------
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(string id)
    {
        var deleted = await _fileService.DeleteFileAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
