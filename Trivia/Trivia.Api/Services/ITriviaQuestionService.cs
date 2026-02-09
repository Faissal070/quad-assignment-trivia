using Trivia.Api.Common.Results;
using Trivia.Api.Models.Dtos;
using Trivia.Api.Models.Queries;

namespace Trivia.Api.Services;

public interface ITriviaQuestionService
{
    Task<Result<IReadOnlyList<QuestionDto>>> GetQuestionsAsync(GetQuestionsQuery query, Guid quizId);
}
