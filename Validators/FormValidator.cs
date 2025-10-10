using FluentValidation;
using GoogleFormsClone.DTOs.Forms;

namespace GoogleFormsClone.Validators;

public class CreateFormDtoValidator : AbstractValidator<CreateFormDto>
{
    public CreateFormDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Form title is required.")
            .MaximumLength(200);

        RuleForEach(x => x.Questions)
            .SetValidator(new QuestionDtoValidator());
    }
}

public class QuestionDtoValidator : AbstractValidator<QuestionDto>
{
    public QuestionDtoValidator()
    {
        RuleFor(q => q.Text)
            .NotEmpty().WithMessage("Question text is required.");

        RuleFor(q => q.Type)
            .NotEmpty().WithMessage("Question type is required.");

        When(q => q.Type is "multipleChoice" or "checkboxes" or "dropdown", () =>
        {
            RuleFor(q => q.Options)
                .NotNull().WithMessage("Options are required for this question type.")
                .Must(o => o!.Count > 0).WithMessage("At least one option must be provided.");
        });

        When(q => q.Type == "linearScale", () =>
        {
            RuleFor(q => q.LinearScale)
                .NotNull().WithMessage("Linear scale configuration is required.");
        });
    }
}