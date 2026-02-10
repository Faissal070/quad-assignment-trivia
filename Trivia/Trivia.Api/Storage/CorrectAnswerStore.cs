namespace Trivia.Api.Storage;

public class CorrectAnswerStore : ICorrectAnswerStore
{
    private readonly Dictionary<Guid, Dictionary<Guid, string>> _correctAnswersByQuizId = new();

    public void AddCorrectAnswer(Guid quizId, Guid questionId, string correctAnswer)
    {
        if (!_correctAnswersByQuizId.ContainsKey(quizId))
        {
            _correctAnswersByQuizId[quizId] = new Dictionary<Guid, string>();
        }

        _correctAnswersByQuizId[quizId][questionId] = correctAnswer; 
    }

    public string? GetCorrectAnswer(Guid quizId, Guid questionId)
    {
        return _correctAnswersByQuizId.TryGetValue(quizId, out var answersByQuestionId) &&
              answersByQuestionId.TryGetValue(questionId, out var correctAnswer)
           ? correctAnswer
           : null;
    }

    public bool QuizExists(Guid quizId)
    {
        return _correctAnswersByQuizId.ContainsKey(quizId);
    } 
    
    public bool RemoveQuiz(Guid quizId)
    {
        return _correctAnswersByQuizId.Remove(quizId);
    }
}
