using GoogleFormsClone.DTOs.Forms;
using GoogleFormsClone.Models;

namespace GoogleFormsClone.Mappers;

public static class FormMapper
{
    public static Form ToModel(CreateFormDto dto, string userId)
    {
        return new Form
        {
            CreatedBy = userId,
            Title = dto.Title,
            Description = dto.Description,
            Questions = dto.Questions.Select((q, index) => new Question
            {
                QuestionText = q.Text,
                Description = q.Description ?? string.Empty,
                Type = q.Type,
                Required = q.Required,
                OrderIndex = index,
                Options = q.Options?.Select((o, i) => new QuestionOption
                {
                    Text = o.Text,
                    AllowsCustomText = o.AllowsCustomText,
                    OrderIndex = i
                }).ToList() ?? new List<QuestionOption>(),
                LinearScale = q.LinearScale != null ? new LinearScale
                {
                    MinValue = q.LinearScale.MinValue,
                    MaxValue = q.LinearScale.MaxValue,
                    MinLabel = q.LinearScale.MinLabel ?? string.Empty,
                    MaxLabel = q.LinearScale.MaxLabel ?? string.Empty
                } : null,
                Validation = q.Validation != null ? new QuestionValidation
                {
                    MinLength = q.Validation.MinLength,
                    MaxLength = q.Validation.MaxLength,
                    Pattern = q.Validation.Pattern ?? string.Empty,
                    MinValue = q.Validation.MinValue,
                    MaxValue = q.Validation.MaxValue,
                    FileTypes = q.Validation.FileTypes,
                    MaxFileSize = q.Validation.MaxFileSize,
                    MaxFileCount = q.Validation.MaxFileCount
                } : null,
                Logic = q.Logic != null ? new QuestionLogic
                {
                    DependsOn = q.Logic.DependsOn,
                    Condition = q.Logic.Condition,
                    Value = q.Logic.Value,
                    Action = q.Logic.Action
                } : null,
                Appearance = q.Appearance != null ? new QuestionAppearance
                {
                    Placeholder = q.Appearance.Placeholder ?? string.Empty,
                    HelpText = q.Appearance.HelpText ?? string.Empty,
                    ImageUrl = q.Appearance.ImageUrl ?? string.Empty
                } : null
            }).ToList()
        };
    }
}
