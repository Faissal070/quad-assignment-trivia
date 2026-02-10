namespace Trivia.Api.Models.Dtos;

public class QuestionDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public List<string> Choices { get; set; } = [];
}
