using Trivia.Api.Common.Results;
using Trivia.Api.Models.Dtos;
using Trivia.Api.Models.Queries;
using Trivia.Api.Storage;

namespace Trivia.Api.Services;

public class TriviaAnswerService : ITriviaAnswerService
{
    private readonly ICorrectAnswerStore _correctAnswerStore;
    private readonly ILogger<TriviaAnswerService> _logger;   

    public TriviaAnswerService(ICorrectAnswerStore correctAnswerStore, ILogger<TriviaAnswerService> logger)
    {
        _correctAnswerStore = correctAnswerStore;   
        _logger = logger;
    }

    /// <summary>
    /// Validates and checks submitted quiz answers, returning correctness per question.
    /// Resultmodels is used for clear external notifications and logs for technical issues.
    /// </summary>
    public async Task<Result<IReadOnlyList<AnswerResultDto>>> SubmitAnswersAsync(SubmitAnswersQuery query)
    {
        try
        {
            if (query.Answers.Count == 0)
            {
                return Result<IReadOnlyList<AnswerResultDto>>.Failure("No answers submitted.");
            }

            if (!_correctAnswerStore.QuizExists(query.QuizId))
            {
                _logger.LogError("Quiz with ID {QuizId} not found or expired when submitting answers.", query.QuizId);

                return Result<IReadOnlyList<AnswerResultDto>>.Failure("Quiz not found or expired.");
            }

            var answerResults = CheckAnswers(query.Answers, query.QuizId);

            return Result<IReadOnlyList<AnswerResultDto>>.Success(answerResults, "Answer(s) checked");
        }
        catch(InvalidOperationException ex)
        {
            _logger.LogError(ex, "An error occurred while processing answers for quiz {QuizId}", query.QuizId);

            return Result<IReadOnlyList<AnswerResultDto>>
           .Failure("One or more questions could not be validated.");
        }
    }

    /// <summary>
    /// Checks submitted answers against the stored correct answers for a quiz.
    /// If the query cannot be found, an exception is thrown, as this should never happen under normal circumstances.
    /// <summary>
    private List<AnswerResultDto> CheckAnswers(List<AnswerDto> answerDtos, Guid quizId)
    {
        var answerResults = new List<AnswerResultDto>();
        foreach (var answer in answerDtos)
        {
            var correctAnswer = _correctAnswerStore.GetCorrectAnswer(quizId, answer.QuestionId);
            if (correctAnswer == null)
            {
                throw new InvalidOperationException($"Question not found {answer.QuestionId}");
            }

            answerResults.Add(new AnswerResultDto
            {
                QuestionId = answer.QuestionId,
                IsCorrect = correctAnswer != null ? answer.SelectedAnswer == correctAnswer : null
            });
        }

        return answerResults;
    }
}
