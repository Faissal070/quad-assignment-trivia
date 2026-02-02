namespace Trivia.Api.Models.External
{
    public class TriviaApiResponse
    {
        public int ResponseCode { get; set; }
        public List<TriviaApiItem> Results { get; set; } = [];
    }
}
