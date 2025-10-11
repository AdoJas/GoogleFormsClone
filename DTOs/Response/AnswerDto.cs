namespace GoogleFormsClone.DTOs.Response
{
    public class AnswerDto
    {
        public string QuestionId { get; set; } = string.Empty;

        public string? Value { get; set; }
        public List<string>? SelectedOptions { get; set; } 
        public int? LinearScaleValue { get; set; }
        public byte[]? FileUpload { get; set; } = Array.Empty<byte>();

    }
}
