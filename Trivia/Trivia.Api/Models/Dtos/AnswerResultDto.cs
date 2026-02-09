namespace Trivia.Api.Models.Dtos;

public class AnswerResultDto
{
    public Guid QuestionId { get; set; }
    public bool? IsCorrect { get; set; }
}
