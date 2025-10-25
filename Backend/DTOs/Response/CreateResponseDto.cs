namespace GoogleFormsClone.DTOs.Response
{
    public class CreateResponseDto
    {
        public string FormId { get; set; } = string.Empty;
        public List<AnswerDto> Answers { get; set; } = new();
        public double DurationSeconds { get; set; } = 0;
        public string? Language { get; set; }
    }
    public class VerifyFormAccessDto
    {
        public string FormId { get; set; } = string.Empty;
        public string? Password { get; set; }
    }
    
    public class FormStatsDto
    {
        public string QuestionId { get; set; } = "";
        public int TotalResponses { get; set; }
        public List<string>? TextAnswers { get; set; }
        public Dictionary<string, int>? OptionCounts { get; set; }
        public Dictionary<int, int>? ScaleCounts { get; set; }
        public Dictionary<string, double>? OptionPercents { get; set; }
    }
}