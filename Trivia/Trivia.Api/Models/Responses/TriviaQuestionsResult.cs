using System.Net;
using Trivia.Api.Enums;
using Trivia.Api.Models.Dtos;

namespace Trivia.Api.Models.Responses
{
    public class TriviaQuestionsResult
    {
        public TriviaApiResponseCodeEnum? ResponseCodeDescription{ get; set; }
        public IEnumerable<TriviaQuestionsDto>? Questions { get; set; }
        public string? ErrorMessage { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
    }
}
