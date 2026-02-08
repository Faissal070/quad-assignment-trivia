using Trivia.Api.Common.Results;
using Trivia.Api.Models.Dtos;
using Trivia.Api.Models.Queries;

namespace Trivia.Api.Services;

public interface ITriviaService
{
    Task<Result<IReadOnlyList<QuestionDto>>> GetQuestionsAsync(GetQuestionsQuery query, string sessionId);
}