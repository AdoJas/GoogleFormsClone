using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoogleFormsClone.Models;
using GoogleFormsClone.Services;
using GoogleFormsClone.DTOs.Forms;
using GoogleFormsClone.Validators;
using GoogleFormsClone.Mappers;
using FluentValidation;

namespace GoogleFormsClone.Controllers
{
    [ApiController]
    [Route("api/forms")]
    [Authorize]
    public class FormsController : ControllerBase
    {
        private readonly IFormService _formService;
        private readonly IValidator<CreateFormDto> _createFormValidator;

        public FormsController(IFormService formService, IValidator<CreateFormDto> createFormValidator)
        {
            _formService = formService;
            _createFormValidator = createFormValidator;
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
        public async Task<ActionResult<Form>> CreateForm([FromBody] CreateFormDto dto)
        {
            var validation = await _createFormValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var userId = User.FindFirst("sub")?.Value ?? "unknown-user";
            var form = FormMapper.ToModel(dto, userId);

            var createdForm = await _formService.CreateFormAsync(form, userId);
            return CreatedAtAction(nameof(GetForm), new { id = createdForm.Id }, createdForm);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Form>> UpdateForm(string id, [FromBody] Form form)
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
