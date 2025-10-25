using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GoogleFormsClone.DTOs.Response;
using GoogleFormsClone.Mappers;
using GoogleFormsClone.Models;
using GoogleFormsClone.Services;

namespace GoogleFormsClone.Controllers
{
    [ApiController]
    [Route("api/responses")]
    [Authorize]
    public class ResponsesController : ControllerBase
    {
        private readonly ResponseService _responseService;
        private readonly IFormService _formService;

        public ResponsesController(ResponseService responseService, IFormService formService)
        {
            _responseService = responseService;
            _formService = formService;
        }

        [HttpGet("/forms/{formId}/responses")]
        public async Task<IActionResult> GetResponsesByForm(string formId)
        {
            var form = await _formService.GetFormAsync(formId);
            if (form == null) return NotFound("Form not found");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole != "Admin" && form.CreatedBy != currentUserId)
                return Forbid("You are not allowed to view these responses");

            var responses = await _responseService.GetByResponseFormIdAsync(formId);
            return Ok(responses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetResponseById(string id)
        {
            var response = await _responseService.GetResponseByIdAsync(id);
            if (response == null)
                return NotFound(new { error = "Response not found." });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            var form = await _formService.GetFormAsync(response.FormId);
            if (form == null) return NotFound("Parent form not found");

            if (currentUserRole != "Admin" && currentUserId != response.SubmittedBy && form.CreatedBy != currentUserId)
                return Forbid("You are not allowed to view this response.");

            return Ok(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateResponse([FromBody] CreateResponseDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.FormId))
                return BadRequest("FormId is required.");

            var form = await _formService.GetFormAsync(dto.FormId);
            if (form == null)
                return NotFound("Form not found.");
            if (!form.AccessControl.IsPublic)
            {
                if (form.AccessControl.RequirePassword)
                {
                    var providedPassword = Request.Headers["Form-Password"].ToString();

                    if (string.IsNullOrEmpty(providedPassword))
                        return Unauthorized("Password is required to submit this form.");

                    if (form.AccessControl.AccessPassword != providedPassword)
                        return Unauthorized("Invalid form password.");
                }
                else
                {
                    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(currentUserId))
                        return Forbid("This form is private and can only be accessed by its owner.");
                }
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var ua = Request.Headers["User-Agent"].ToString();
            var referrer = Request.Headers["Referer"].ToString() ?? "unknown";

            if (form.Settings.OneResponsePerUser && userId != "anonymous")
            {
                var existing = await _responseService.GetByResponseFormIdAsync(dto.FormId);
                if (existing.Any(r => r.SubmittedBy == userId))
                    return BadRequest("You have already submitted a response to this form.");
            }

            var response = ResponseMapper.ToModel(dto, userId, form);

            response.Metadata.IpAddress = ip;
            response.Metadata.UserAgent = ua;
            response.Metadata.Referrer = referrer;

            var created = await _responseService.CreateResponseAsync(response);

            return CreatedAtAction(nameof(GetResponseById), new { id = created.Id }, created);
        }


        // -------- Helpers ----------
        private static string? ValidateAnswer(Question q, AnswerDto dto)
        {
            var type = (dto.Type ?? q.Type ?? string.Empty).Trim().ToLowerInvariant();

            switch (type)
            {
                case "text":
                    if (q.Required && string.IsNullOrWhiteSpace(dto.Response.Text))
                        return $"Answer required for question {q.Id}.";
                    return null;

                case "multiple-choice":
                case "checkbox":
                    var options = q.Options.Select(o => o.Text).ToHashSet();
                    var selected = dto.Response.SelectedOptions ?? new List<string>();
                    if (q.Required && selected.Count == 0)
                        return $"Answer required for question {q.Id}.";
                    if (!selected.All(options.Contains))
                        return $"Invalid option selected for question {q.Id}.";
                    return null;

                case "linear-scale":
                    if (q.Required && !dto.Response.LinearScaleValue.HasValue)
                        return $"Linear scale value required for question {q.Id}.";
                    if (dto.Response.LinearScaleValue.HasValue && q.LinearScale != null)
                    {
                        var v = dto.Response.LinearScaleValue.Value;
                        if (v < q.LinearScale.MinValue || v > q.LinearScale.MaxValue)
                            return $"Linear scale value out of range for question {q.Id}.";
                    }
                    return null;

                case "file-upload":
                    var ids = dto.Response.FileIds ?? new List<string>();
                    if (q.Required && ids.Count == 0)
                        return $"File upload required for question {q.Id}.";
                    return null;

                default:
                    return $"Unsupported question type {type} for question {q.Id}.";
            }
        }
    }
}
