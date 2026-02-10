namespace Trivia.Api.Models.External;

public class TriviaApiQuestionResponse : TriviaApiResponse
{
    public List<TriviaApiQuestion> Results { get; set; } = [];
}
