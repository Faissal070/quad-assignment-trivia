namespace Trivia.Api.Models.External
{
    public class TriviaApiItem
    {
        public string Type { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public List<string> IncorrectAnswers { get; set; } = [];
    }
}
