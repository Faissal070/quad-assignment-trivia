
namespace Trivia.Api.Storage
{
    public class CorrectAnswerStore : ICorrectAnswerStore
    {
        private readonly Dictionary<Guid, string> _correctAnswersByQuestionId = new();

        public void AddCorrectAnswer(Guid questionId, string correctAnswer)
        {
            _correctAnswersByQuestionId[questionId] = correctAnswer;
        }

        public string? GetCorrectAnswer(Guid questionId)
        {
            _correctAnswersByQuestionId.TryGetValue(questionId, out var correctAnswer);
            return correctAnswer;
        }
    }
}
