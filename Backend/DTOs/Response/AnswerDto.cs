namespace GoogleFormsClone.DTOs.Response
{
    public class AnswerDto
    {
        public string QuestionId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "text", "choice", "linear", "file"
        public AnswerValueDto Response { get; set; } = new();
    }

    public class AnswerValueDto
    {
        public string? Text { get; set; }                
        public List<string>? SelectedOptions { get; set; } 
        public int? LinearScaleValue { get; set; }      
        public List<string>? FileIds { get; set; }      
    }
}