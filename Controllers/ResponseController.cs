using GoogleFormsClone.Models;
using GoogleFormsClone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoogleFormsClone.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResponsesController : ControllerBase
{
    private readonly ResponseService _responseService;
    private readonly FormService _formService;
    private readonly ILogger<ResponsesController> _logger;

    public ResponsesController(
        ResponseService responseService,
        FormService formService,
        ILogger<ResponsesController> logger)
    {
        _responseService = responseService;
        _formService = formService;
        _logger = logger;
    }

    [HttpGet("form/{formId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Response>>> GetFormResponses(string formId)
    {
        var form = await _formService.GetFormByIdAsync(formId);
        if (form == null)
            return NotFound("Form not found");

        if (form.CreatedBy != User.GetUserId())
            return Forbid("You don't have permission to view responses for this form");

        var responses = await _responseService.GetResponsesByFormIdAsync(formId);
        return Ok(responses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response>> GetResponse(string id)
    {
        var response = await _responseService.GetResponseByIdAsync(id);
        if (response == null)
            return NotFound();

        // Check permission - form owner or response submitter
        var form = await _formService.GetFormByIdAsync(response.FormId);
        if (form == null)
            return NotFound("Form not found");

        var currentUserId = User.GetUserId();
        if (form.CreatedBy != currentUserId && response.SubmittedBy != currentUserId)
            return Forbid("You don't have permission to view this response");

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<Response>> SubmitResponse([FromBody] DTOs.Response.SubmitResponseDto submitDto)
    {
        var form = await _formService.GetFormByIdAsync(submitDto.FormId);
        if (form == null)
            return NotFound("Form not found");

        if (!form.IsActive)
            return BadRequest("This form is no longer accepting responses");

        if (form.Settings.RequireLogin && !User.Identity.IsAuthenticated)
            return Unauthorized("You must be logged in to submit this form");

        var metadata = new ResponseMetadata
        {
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
            UserAgent = Request.Headers.UserAgent.ToString(),
            Language = Request.Headers.AcceptLanguage.ToString(),
            Referrer = Request.Headers.Referer.ToString(),
            DurationSeconds = submitDto.DurationSeconds
        };

        var response = await _responseService.CreateResponseAsync(
            submitDto.FormId,
            User.Identity.IsAuthenticated ? User.GetUserId() : null,
            submitDto.Answers,
            metadata);

        return CreatedAtAction(nameof(GetResponse), new { id = response.Id }, response);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteResponse(string id)
    {
        var response = await _responseService.GetResponseByIdAsync(id);
        if (response == null)
            return NotFound();

        var form = await _formService.GetFormByIdAsync(response.FormId);
        if (form == null || form.CreatedBy != User.GetUserId())
            return Forbid("You don't have permission to delete this response");

        await _responseService.DeleteResponseAsync(id);
        return NoContent();
    }
}
