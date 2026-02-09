using Trivia.Api.Models.External;
using Trivia.Api.Models.Queries;
using Trivia.Api.Models.Result;

namespace Trivia.Api.Client;

public interface ITriviaApiClient
{
    Task<TriviaApiQuestionResponse> FetchQuestionsAsync(GetQuestionsQuery query, string token);
    Task<TriviaApiTokenResponse> FetchTokenAsync();
    Task<TriviaApiTokenResponse> ResetTokenAsync(string token);
}
