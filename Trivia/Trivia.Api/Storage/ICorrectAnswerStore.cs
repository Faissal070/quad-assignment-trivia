namespace Trivia.Api.Storage
{
    public interface ICorrectAnswerStore
    {
        public void AddCorrectAnswer(Guid questionId, string correctAnswer);
        public string? GetCorrectAnswer(Guid questionId);   
    }
}
