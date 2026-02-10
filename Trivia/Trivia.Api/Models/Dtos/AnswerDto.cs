namespace Trivia.Api.Models.Dtos;

public class AnswerDto
{
    public Guid QuestionId { get; set; }
    public string? SelectedAnswer { get; set; }
}
