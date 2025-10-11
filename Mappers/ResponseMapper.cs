using GoogleFormsClone.DTOs.Response;
using GoogleFormsClone.Models;
public static class ResponseMapper
{
public static Response ToModel(CreateResponseDto dto, string userId)
{
    return new Response
    {
        FormId = dto.FormId,
        SubmittedBy = userId,
        SubmittedAt = DateTime.UtcNow,
        Answers = dto.Answers.Select(a => new Answer
        {
            QuestionId = a.QuestionId,
            AnswerText = a.Value,
            SelectedOptions = a.SelectedOptions,
            LinearScaleValue = a.LinearScaleValue
        }).ToList()
    };
}

}
