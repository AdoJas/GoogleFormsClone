using GoogleFormsClone.Models;
using GoogleFormsClone.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoogleFormsClone.Controllers;

[ApiController]
[Route("api/response")]
public class ResponseController : ControllerBase
{
    private readonly ResponseService _responseService;

    public ResponseController(ResponseService responseService)
    {
        _responseService = responseService;
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
    public async Task<IActionResult> CreateResponse([FromBody] Response response)
    {
        if (response == null || string.IsNullOrEmpty(response.FormId))
            return BadRequest(new { error = "FormId is required." });

        var created = await _responseService.CreateResponseAsync(response);
        return CreatedAtAction(nameof(GetResponseById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResponse(string id, [FromBody] Response response)
    {
        if (response == null)
            return BadRequest(new { error = "Invalid response data." });

        var updated = await _responseService.UpdateResponseAsync(id, response);
        if (!updated)
            return NotFound(new { error = "Response not found." });

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResponse(string id)
    {
        var deleted = await _responseService.DeleteResponseAsync(id);
        if (!deleted)
            return NotFound(new { error = "Response not found." });

        return NoContent();
    }
}
