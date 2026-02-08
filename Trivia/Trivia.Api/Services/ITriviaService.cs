using Trivia.Api.Common.Results;
using Trivia.Api.Models.Dtos;

namespace Trivia.Api.Services;

public interface ITriviaService
{
    Task<Result<IReadOnlyList<QuestionDto>>> GetQuestionsAsync(int amount);
}