namespace GoogleFormsClone.DTOs.Response;

public class CreateResponseDto
{
    public string FormId { get; set; } = string.Empty;
    public List<AnswerDto> Answers { get; set; } = new();
}
