namespace Trivia.Api.Storage;

public interface ICorrectAnswerStore
{
    void AddCorrectAnswer(Guid quizId, Guid questionId, string correctAnswer);
    string? GetCorrectAnswer(Guid quizId, Guid questionId);
    bool QuizExists(Guid quizId);
    bool RemoveQuiz(Guid quizId);
}
