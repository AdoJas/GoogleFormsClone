using GoogleFormsClone.DTOs.Response;
using GoogleFormsClone.Models;

namespace GoogleFormsClone.Mappers
{
    public static class ResponseMapper
    {
        public static Response ToModel(CreateResponseDto dto, string userId, Form form)
        {
            var answers = new List<Answer>();

            foreach (var a in dto.Answers)
            {
                var q = form.Questions.FirstOrDefault(x => x.Id == a.QuestionId);

                var answer = new Answer
                {
                    QuestionId = a.QuestionId,
                    AnswerText = a.Response?.Text,
                    SelectedOptions = a.Response?.SelectedOptions,
                    LinearScaleValue = a.Response?.LinearScaleValue,
                    QuestionSnapshot = new QuestionSnapshot
                    {
                        QuestionText = q?.QuestionText ?? string.Empty,
                        QuestionType = q?.Type ?? string.Empty,
                        Options = q?.Options ?? new List<QuestionOption>()
                    }
                };

                if (a.Response?.FileIds != null && a.Response.FileIds.Any())
                {
                    var fileId = a.Response.FileIds.First();
                    answer.FileUpload = new FileUpload
                    {
                        FileUrl = $"/api/files/{fileId}",
                        FileName = "Uploaded file",
                        MimeType = "application/octet-stream"
                    };
                }

                answers.Add(answer);
            }

            return new Response
            {
                FormId = dto.FormId,
                SubmittedBy = userId,
                SubmittedAt = DateTime.UtcNow,
                Answers = answers,
                Metadata = new ResponseMetadata
                {
                    DurationSeconds = dto.DurationSeconds,
                    Language = dto.Language ?? "unknown",
                    IpAddress = "captured_at_controller", 
                    UserAgent = "captured_at_controller"
                }
            };
        }
    }
}
