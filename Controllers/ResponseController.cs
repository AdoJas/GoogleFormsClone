using GoogleFormsClone.Models;
using Microsoft.AspNetCore.Authorization;
using GoogleFormsClone.Services;
using Microsoft.AspNetCore.Mvc;
using GoogleFormsClone.DTOs.Response;
using System.Security.Claims;
using MongoDB.Driver;


namespace GoogleFormsClone.Controllers;

[ApiController]
[Route("api/response")]
[Authorize]
public class ResponseController : ControllerBase
{
    private readonly ResponseService _responseService;
    private readonly IFormService _formService;

    public ResponseController(ResponseService responseService, IFormService formService)
    {
        _responseService = responseService;
        _formService = formService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllResponses()
    {
        var responses = await _responseService.GetAllResponsesAsync();
        return Ok(responses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetResponseById(string id)
    {
        var response = await _responseService.GetResponseByIdAsync(id);
        if (response == null)
            return NotFound(new { error = "Response not found." });

        return Ok(response);
    }

    [HttpGet("form/{formId}")]
    public async Task<IActionResult> GetResponsesByFormId(string formId)
    {
        var responses = await _responseService.GetByResponseFormIdAsync(formId);
        return Ok(responses);
    }

    [HttpPost]
    public async Task<IActionResult> CreateResponse([FromBody] CreateResponseDto dto)
    {
        if (dto == null || string.IsNullOrEmpty(dto.FormId))
            return BadRequest("FormId is required.");

        var form = await _formService.GetFormAsync(dto.FormId);
        if (form == null)
            return NotFound("Form not found.");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (form.Settings.OneResponsePerUser)
        {
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found.");
            var existingResponses = await _responseService.GetByResponseFormIdAsync(dto.FormId);
            if (existingResponses.Any(r => r.SubmittedBy == userId))
                return BadRequest("You have already submitted a response to this form.");
        }

        foreach (var answer in dto.Answers)
            {
                var question = form.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null)
                    return BadRequest($"Question {answer.QuestionId} does not exist.");

            switch (question.Type)
            {
                case "text":
                    if (question.Required && string.IsNullOrEmpty(answer.Value))
                        return BadRequest($"Answer required for question {question.Id}.");
                    if (question.Validation != null)
                    {
                        if (answer.Value?.Length < question.Validation.MinLength)
                            return BadRequest($"Answer too short for question {question.Id}.");
                        if (answer.Value?.Length > question.Validation.MaxLength)
                            return BadRequest($"Answer too long for question {question.Id}.");
                    }
                    break;

                case "multiple-choice":
                    if (question.Required)
                    {
                        if (question.AllowMultipleSelection == false)
                        {
                            if (string.IsNullOrEmpty(answer.Value))
                            {
                                return BadRequest($"Answer required for question {question.Id}.");
                            }
                            else if (answer.SelectedOptions != null && answer.SelectedOptions.Count > 1)
                            {
                                return BadRequest($"Only one choice allowed for question {question.Id}.");
                            }
                            if (!question.Options.Any(o => o.Text == answer.Value))
                                return BadRequest($"Invalid option selected for question {question.Id}.");
                        }
                        else
                        {
                            if (answer.SelectedOptions != null && answer.SelectedOptions.Count == 0)
                                return BadRequest($"Answer required for question {question.Id}.");

                            if (answer.SelectedOptions != null && !answer.SelectedOptions.All(o => question.Options.Any(qo => qo.Text == o)))
                                return BadRequest($"Invalid option selected for question {question.Id}.");

                        }

                    }
                    break;

                case "linear-scale":
                    if (question.Required && !answer.LinearScaleValue.HasValue)
                        return BadRequest($"Linear scale value required for question {question.Id}.");
                    if (answer.LinearScaleValue.HasValue &&  question.LinearScale != null
                        && (answer.LinearScaleValue < question.LinearScale.MinValue || answer.LinearScaleValue > question.LinearScale.MaxValue))
                        return BadRequest($"Linear scale value out of range for question {question.Id}.");
                    break;
                case "File upload":
                    if (question.Required && (answer.FileUpload == null || answer.FileUpload.Length == 0))
                        return BadRequest($"File upload required for question {question.Id}.");
                    break;
                default:
                    return BadRequest($"Unsupported question type {question.Type} for question {question.Id}.");
                }
            }

            var response = ResponseMapper.ToModel(dto, userId);
            var created = await _responseService.CreateResponseAsync(response);

            return CreatedAtAction(nameof(GetResponseById), new { id = created.Id }, created);
        }
}
