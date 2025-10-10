using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoogleFormsClone.Models;
using GoogleFormsClone.Services;

namespace GoogleFormsClone.Controllers
{
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
        public async Task<ActionResult<List<Form>>> GetForms()
        {
            var forms = await _formService.GetAllFormsAsync();
            return Ok(forms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Form>> GetForm(string id)
        {
            var form = await _formService.GetFormAsync(id);
            if (form == null) return NotFound();
            return Ok(form);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Form>>> GetUserForms(string userId)
        {
            var forms = await _formService.GetUserFormsAsync(userId);
            return Ok(forms);
        }

        [HttpPost]
        public async Task<ActionResult<Form>> CreateForm(Form form) // TODO:: FromBody take user id, now doesnt work like supposed
        {
            var userId = User.FindFirst("id")?.Value ?? "unknown-user";
            var createdForm = await _formService.CreateFormAsync(form, userId);
            return CreatedAtAction(nameof(GetForm), new { id = createdForm.Id }, createdForm);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Form>> UpdateForm(string id, Form form)
        {
            if (id != form.Id) return BadRequest();

            var updatedForm = await _formService.UpdateFormAsync(id, form);
            if (updatedForm == null) return NotFound();

            return Ok(updatedForm);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteForm(string id)
        {
            var deleted = await _formService.DeleteFormAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
