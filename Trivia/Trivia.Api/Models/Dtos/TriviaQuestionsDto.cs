namespace Trivia.Api.Models.Dtos
{
    public class TriviaQuestionsDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public List<string> Incorrect_Answers { get; set; } = [];
    }
}
