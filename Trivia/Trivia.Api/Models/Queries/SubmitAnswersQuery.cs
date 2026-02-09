using System.ComponentModel.DataAnnotations;
using Trivia.Api.Models.Dtos;

namespace Trivia.Api.Models.Queries;

public class SubmitAnswersQuery
{
    [Required]
    public Guid QuizId { get; set; }
    public List<AnswerDto> Answers { get; set; } = [];
}
