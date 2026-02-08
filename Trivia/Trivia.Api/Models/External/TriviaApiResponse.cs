using System.Text.Json.Serialization;

namespace Trivia.Api.Models.External;

public abstract class TriviaApiResponse
{
    [JsonPropertyName("response_code")]
    public int ResponseCode { get; set; }
}