namespace Trivia.Api.Storage;

public interface ICorrectAnswerStore
{
    void AddCorrectAnswer(Guid questionId, string correctAnswer);
    string? GetCorrectAnswer(Guid questionId);   
}