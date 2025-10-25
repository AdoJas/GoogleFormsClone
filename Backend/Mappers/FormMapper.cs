using GoogleFormsClone.DTOs.Forms;
using GoogleFormsClone.DTOs.File;
using GoogleFormsClone.Models;

namespace GoogleFormsClone.Mappers;

public static class FormMapper
{
    // ------------------------
    // DTO → Model
    // ------------------------
    public static Form ToModel(CreateFormDto dto, string userId)
    {
        return new Form
        {
            CreatedBy = userId,
            Title = dto.Title,
            Description = dto.Description,
            IsActive = dto.IsActive,
            HeaderImageId = dto.HeaderImageId, 

            Settings = dto.Settings == null ? new FormSettings() : new FormSettings
            {
                ShowProgress = dto.Settings.ShowProgress,
                CollectEmails = dto.Settings.CollectEmails,
                OneResponsePerUser = dto.Settings.OneResponsePerUser,
                AllowResponseEditing = dto.Settings.AllowResponseEditing,
                ResponseEditingDuration = dto.Settings.ResponseEditingDuration,
                ConfirmationMessage = dto.Settings.ConfirmationMessage ?? string.Empty
            },

            AccessControl = dto.AccessControl == null ? new AccessControl() : new AccessControl
            {
                IsPublic = dto.AccessControl.IsPublic,
                RequirePassword = dto.AccessControl.RequirePassword,
                AccessPassword = dto.AccessControl.AccessPassword ?? string.Empty
            },

            Questions = dto.Questions.Select((q, index) => new Question
            {
                Id = string.IsNullOrEmpty(q.Id) ? Guid.NewGuid().ToString() : q.Id,
                QuestionText = q.Text ?? string.Empty,
                Description = q.Description ?? string.Empty,
                Type = q.Type,
                Required = q.Required,
                OrderIndex = index,
                AllowMultipleSelection = q.AllowMultipleSelection,

                Options = q.Options?.Select((o, i) => new QuestionOption
                {
                    Id = string.IsNullOrEmpty(o.Id) ? Guid.NewGuid().ToString() : o.Id,
                    Text = o.Text,
                    AllowsCustomText = o.AllowsCustomText,
                    OrderIndex = i
                }).ToList() ?? new List<QuestionOption>(),

                LinearScale = q.LinearScale == null ? null : new LinearScale
                {
                    MinValue = q.LinearScale.MinValue,
                    MaxValue = q.LinearScale.MaxValue,
                    MinLabel = q.LinearScale.MinLabel ?? string.Empty,
                    MaxLabel = q.LinearScale.MaxLabel ?? string.Empty
                },

                Validation = q.Validation == null ? null : new QuestionValidation
                {
                    MinLength = q.Validation.MinLength,
                    MaxLength = q.Validation.MaxLength,
                    Pattern = q.Validation.Pattern ?? string.Empty,
                    MinValue = q.Validation.MinValue,
                    MaxValue = q.Validation.MaxValue,
                    FileTypes = q.Validation.FileTypes,
                    MaxFileSize = q.Validation.MaxFileSize,
                    MaxFileCount = q.Validation.MaxFileCount
                },

                Logic = q.Logic == null ? null : new QuestionLogic
                {
                    DependsOn = q.Logic.DependsOn,
                    Condition = q.Logic.Condition,
                    Value = q.Logic.Value,
                    Action = q.Logic.Action
                },

                Appearance = q.Appearance == null ? null : new QuestionAppearance
                {
                    Placeholder = q.Appearance.Placeholder ?? string.Empty,
                    HelpText = q.Appearance.HelpText ?? string.Empty,
                    ImageUrl = q.Appearance.ImageUrl ?? string.Empty
                }
            }).ToList()
        };
    }

    // ------------------------
    // Model + Files → DTO
    // ------------------------
    public static FormWithFilesDto ToWithFilesDto(Form form, List<FileResource> files)
    {
        var headerFile = files.FirstOrDefault(f =>
            f.AssociatedWith == form.Id &&
            f.AssociatedEntityType == "FormHeader");

        var dto = new FormWithFilesDto
        {
            Id = form.Id,
            Title = form.Title,
            Description = form.Description,
            CreatedBy = form.CreatedBy,
            IsActive = form.IsActive,

            HeaderImage = headerFile != null
                ? new FileAttachmentDto
                {
                    Id = headerFile.Id,
                    FileName = headerFile.OriginalName,
                    MimeType = headerFile.FileType,
                    FileUrl = $"https://localhost:7180/api/files/{headerFile.Id}/download",
                    FileSize = headerFile.FileSize
                }
                : null,

            Settings = new FormSettingsDto
            {
                ShowProgress = form.Settings.ShowProgress,
                CollectEmails = form.Settings.CollectEmails,
                OneResponsePerUser = form.Settings.OneResponsePerUser,
                AllowResponseEditing = form.Settings.AllowResponseEditing,
                ResponseEditingDuration = form.Settings.ResponseEditingDuration,
                ConfirmationMessage = form.Settings.ConfirmationMessage
            },

            AccessControl = new AccessControlDto
            {
                IsPublic = form.AccessControl.IsPublic,
                RequirePassword = form.AccessControl.RequirePassword,
                AccessPassword = form.AccessControl.AccessPassword
            },

            Questions = form.Questions.Select(q =>
            {
                var file = files.FirstOrDefault(f =>
                    f.AssociatedWith == q.Id &&
                    f.AssociatedEntityType == "QuestionAttachment");

                return new QuestionWithFileDto
                {
                    Id = q.Id,
                    Type = q.Type,
                    QuestionText = q.QuestionText,
                    Description = q.Description,
                    Required = q.Required,
                    AllowMultipleSelection = q.AllowMultipleSelection,
                    Options = q.Options?.Select(o => new OptionDto
                    {
                        Id = o.Id,
                        Text = o.Text,
                        AllowsCustomText = o.AllowsCustomText,
                        OrderIndex = o.OrderIndex
                    }).ToList(),
                    Attachment = file != null
                        ? new FileAttachmentDto
                        {
                            Id = file.Id,
                            FileName = file.OriginalName,
                            MimeType = file.FileType,
                            FileUrl = $"https://localhost:7180/api/files/{file.Id}/download",
                            FileSize = file.FileSize
                        }
                        : null
                };
            }).ToList()
        };

        return dto;
    }
}
