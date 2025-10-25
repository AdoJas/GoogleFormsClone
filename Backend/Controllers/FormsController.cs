using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoogleFormsClone.Models;
using GoogleFormsClone.Services;
using GoogleFormsClone.Mappers;
using FluentValidation;
using System.Security.Claims;
using GoogleFormsClone.DTOs.Forms;
using GoogleFormsClone.DTOs.File;
using GoogleFormsClone.DTOs.Response;


namespace GoogleFormsClone.Controllers;
[ApiController]
[Route("api/forms")]
[Authorize]
public class FormsController : ControllerBase
{
    private readonly IFormService _formService;
    private readonly FileService _fileService;
    private readonly ResponseService _responseService;


    public FormsController(IFormService formService, FileService fileService, ResponseService responseService)
    {
        _formService = formService;
        _fileService = fileService;
        _responseService = responseService;
    }
    
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<FormWithFilesDto>> GetForm(string id, [FromQuery] string? password = null)
    {
        var formWithFiles = await _formService.GetFormWithFilesAsync(id);
        if (formWithFiles == null)
            return NotFound("Form not found.");

        var userId = User.FindFirstValue("sub"); 
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        bool isAuthenticated = User.Identity?.IsAuthenticated == true;

        bool isOwner = !string.IsNullOrEmpty(userId) &&
                       formWithFiles.CreatedBy != null &&
                       string.Equals(userId, formWithFiles.CreatedBy, StringComparison.OrdinalIgnoreCase);
        bool isAdmin = string.Equals(userRole, "admin", StringComparison.OrdinalIgnoreCase);

        if (isAuthenticated && (isOwner || isAdmin))
            return Ok(formWithFiles);

        if (formWithFiles.AccessControl.IsPublic)
            return Ok(formWithFiles);

        if (formWithFiles.AccessControl.RequirePassword)
        {
            if (string.IsNullOrEmpty(password) || password != formWithFiles.AccessControl.AccessPassword)
            {
                return Ok(new
                {
                    id = formWithFiles.Id,
                    title = formWithFiles.Title,
                    description = "This form is password protected.",
                    accessControl = new
                    {
                        isPublic = formWithFiles.AccessControl.IsPublic,
                        requirePassword = true
                    },
                    settings = new
                    {
                        showProgress = formWithFiles.Settings.ShowProgress
                    },
                    questions = new List<object>()
                });
            }

            return Ok(formWithFiles); 
        }

        return Forbid();
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<Form>>> GetUserForms(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

        if (currentUserRole != "Admin" && currentUserId != userId)
            return Forbid("You are not allowed to access this user's forms.");

        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID not found");

        var forms = await _formService.GetUserFormsAsync(userId);
        return Ok(forms);
    }

    [HttpPost]
    public async Task<ActionResult<Form>> CreateForm([FromBody] CreateFormDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required");

        if (dto.Questions == null || !dto.Questions.Any())
            return BadRequest("At least one question is required");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("User ID not found");


        var form = FormMapper.ToModel(dto, userId);

        var createdForm = await _formService.CreateFormAsync(form, userId);
        return CreatedAtAction(nameof(GetForm), new { id = createdForm.Id }, createdForm);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<FormWithFilesDto>> UpdateForm(string id, [FromBody] CreateFormDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required");

        if (dto.Questions == null || !dto.Questions.Any())
            return BadRequest("At least one question is required");

        var existingForm = await _formService.GetFormAsync(id);
        if (existingForm == null)
            return NotFound("Form not found.");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

        if (currentUserRole != "Admin" && existingForm.CreatedBy != currentUserId)
            return Forbid("You are not allowed to edit this form.");

        var updatedModel = FormMapper.ToModel(dto, existingForm.CreatedBy);
        updatedModel.Id = existingForm.Id;
        updatedModel.CreatedAt = existingForm.CreatedAt;
        updatedModel.UpdatedAt = DateTime.UtcNow;

        var updatedForm = await _formService.UpdateFormAsync(id, updatedModel);

        var files = await _fileService.GetFilesByAssociatedWithAsync(id);

        var formWithFiles = FormMapper.ToWithFilesDto(updatedForm, files);

        return Ok(formWithFiles);
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteForm(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

        var form = await _formService.GetFormAsync(id);
        if (form == null)
            return NotFound();

        if (currentUserRole != "Admin" && form.CreatedBy != currentUserId)
            return Forbid("You are not allowed to delete this form.");

        var deleted = await _formService.DeleteFormCascadeAsync(id);
        return deleted ? NoContent() : NotFound();
    }
    
    [HttpGet("me")]
    public async Task<ActionResult<List<Form>>> GetMyForms()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        var forms = await _formService.GetDashboardFormsAsync(currentUserId);
        return Ok(forms);
    }
    
    [HttpPost("verify-access")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyFormAccess([FromBody] VerifyFormAccessDto dto)
    {
        var form = await _formService.GetFormAsync(dto.FormId);
        if (form == null)
            return NotFound("Form not found.");

        if (form.AccessControl.IsPublic)
            return Ok(new { access = "granted" });

        if (!form.AccessControl.RequirePassword)
            return Forbid("This form is private and cannot be accessed anonymously.");

        if (string.IsNullOrEmpty(dto.Password))
            return BadRequest("Password is required.");

        if (form.AccessControl.AccessPassword == dto.Password)
            return Ok(new { access = "granted" });

        return Unauthorized("Invalid password.");
    }
    [HttpGet("{formId}/stats")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFormStats(string formId)
    {
        var form = await _formService.GetFormAsync(formId);
        if (form == null) return NotFound("Form not found.");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserRole = User.FindFirstValue(ClaimTypes.Role);
        var isOwner = form.CreatedBy == currentUserId;
        var isAdmin = currentUserRole == "Admin";

        if (!form.AccessControl.IsPublic && !isOwner && !isAdmin)
            return Forbid("You are not allowed to view these stats.");

        var stats = await _responseService.GetFormStatsAsync(formId);
        return Ok(stats);
    }
}

