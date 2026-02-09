using Trivia.Api.Models.External;

namespace Trivia.Api.Models.Result;

public class TriviaApiTokenResponse : TriviaApiResponse
{
    public string? Token { get; set; }
}
