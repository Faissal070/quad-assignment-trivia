using Trivia.Api.Common.Results;
using Trivia.Api.Models.Dtos;
using Trivia.Api.Models.Queries;

namespace Trivia.Api.Services;

public interface ITriviaAnswerService
{
    Task<Result<IReadOnlyList<AnswerResultDto>>> SubmitAnswersAsync(SubmitAnswersQuery query);
}
