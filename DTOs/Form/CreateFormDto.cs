namespace DefaultNamespace;

public class CreateFormDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public List<QuestionDto> Questions { get; set; }
}