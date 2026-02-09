namespace Trivia.Api.Storage;

public interface ICorrectAnswerStore
{
    void AddCorrectAnswer(Guid guizId, Guid questionId, string correctAnswer);
    string? GetCorrectAnswer(Guid quizId, Guid questionId);
    bool RemoveQuiz(Guid quizId);
    bool QuizExists(Guid quizId);
}
