using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoogleFormsClone.Models;
using GoogleFormsClone.Services;
using GoogleFormsClone.DTOs.Forms;
using GoogleFormsClone.Mappers;
using FluentValidation;
using System.Security.Claims;


namespace GoogleFormsClone.Controllers;
[ApiController]
[Route("api/forms")]
[Authorize]
public class FormsController : ControllerBase
{
    private readonly IFormService _formService;

    public FormsController(IFormService formService)
    {
        _formService = formService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Form>>> GetForms()
    {
        var forms = await _formService.GetAllFormsAsync();
        return Ok(forms);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Form>> GetForm(string id)
    {
        var form = await _formService.GetFormAsync(id);
        if (form == null) return NotFound();
        return Ok(form);
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
    public async Task<ActionResult<Form>> UpdateForm(string id, [FromBody] Form form)
    {
        if (id != form.Id) return BadRequest("ID mismatch");

        var updatedForm = await _formService.UpdateFormAsync(id, form);
        if (updatedForm == null) return NotFound();

        if (string.IsNullOrWhiteSpace(form.Title))
            return BadRequest("Title is required");

        if (form.Questions == null || !form.Questions.Any())
            return BadRequest("At least one question is required");
        

        return Ok(updatedForm);
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

        var deleted = await _formService.DeleteFormAsync(id);
        return deleted ? NoContent() : NotFound();
    }
    
    [HttpGet("me")]
    public async Task<ActionResult<List<Form>>> GetMyForms()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        var forms = await _formService.GetUserFormsAsync(currentUserId);
        return Ok(forms);
    }
}

