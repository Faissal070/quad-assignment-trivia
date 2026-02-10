using Microsoft.Extensions.Logging;
using Moq;
using Trivia.Api.Common.Constants;
using Trivia.Api.Common.Results;
using Trivia.Api.Models.Dtos;
using Trivia.Api.Models.Queries;
using Trivia.Api.Services;
using Trivia.Api.Storage;

namespace Trivia.Api.Tests.Services;

public class TriviaAnswerServiceTests
{
    private readonly Mock<ICorrectAnswerStore> _correctAnswerStoreMock;
    private readonly Mock<ILogger<TriviaAnswerService>> _loggerMock;

    private readonly TriviaAnswerService _triviaAnswerService;

    public TriviaAnswerServiceTests()
    {
        _correctAnswerStoreMock = new Mock<ICorrectAnswerStore>();
        _loggerMock = new Mock<ILogger<TriviaAnswerService>>();

        _triviaAnswerService = new TriviaAnswerService(_correctAnswerStoreMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task SubmitAnswersAsync_WhenNoAnswersSubmitted_ShouldReturnFailure()
    {
        //Arrange
        var query = new SubmitAnswersQuery
        {
            QuizId = Guid.NewGuid(),
            Answers = new List<AnswerDto>()
        };

        //Act
        var result = await _triviaAnswerService.SubmitAnswersAsync(query);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.IsType<Result<IReadOnlyList<AnswerResultDto>>>(result);
        Assert.Equal("No answers submitted.", result.Message);
    }

    [Fact]
    public async Task SubmitAnswersAsync_WhenQuizNotFound_ShouldReturnFailure()
    {
        //Arrange
        var quizId = Guid.NewGuid();
        var query = CreateValidSubmitAnswersQuery(quizId);
        _correctAnswerStoreMock.Setup(store => store.QuizExists(quizId)).Returns(false);

        //Act
        var result = await _triviaAnswerService.SubmitAnswersAsync(query);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.IsType<Result<IReadOnlyList<AnswerResultDto>>>(result);
        Assert.Equal(SubmitAnswersConstants.QuizNotFoundOrExpired, result.Message);
    }

    [Fact]
    public async Task SubmitAnswersAsync_WhenValidSubmission_ShouldReturnSuccess()
    {
        //Arrange
        var quizId = Guid.NewGuid();
        var query = CreateValidSubmitAnswersQuery(quizId);

        _correctAnswerStoreMock.Setup(store => store.QuizExists(quizId))
            .Returns(true);

        _correctAnswerStoreMock.Setup(store => store.GetCorrectAnswer(quizId, It.IsAny<Guid>()))
            .Returns("Answer");

        //Act
        var result = await _triviaAnswerService.SubmitAnswersAsync(query);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.IsType<Result<IReadOnlyList<AnswerResultDto>>>(result);
        Assert.NotNull(result.Data);
        Assert.Equal(query.Answers.Count, result.Data.Count);
    }

    [Fact]
    public async Task SubmitAnswersAsync_WhenExceptionThrown_ShouldReturnFailure()
    {
        //Arrange
        var quizId = Guid.NewGuid();
        var query = CreateValidSubmitAnswersQuery(quizId);

        _correctAnswerStoreMock.Setup(x => x.QuizExists(quizId))
            .Returns(true);

        _correctAnswerStoreMock.Setup(x => x.GetCorrectAnswer(quizId, Guid.NewGuid()))
            .Returns((string?)null);

        //Act
        var result = await _triviaAnswerService.SubmitAnswersAsync(query);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.IsType<Result<IReadOnlyList<AnswerResultDto>>>(result);
        Assert.Equal(SubmitAnswersConstants.AnswerValidationFailed, result.Message);
    }

    private SubmitAnswersQuery CreateValidSubmitAnswersQuery(Guid quizId)
    {
        return new SubmitAnswersQuery
        {
            QuizId = quizId,
            Answers = new List<AnswerDto>()
            {
                new AnswerDto
                {
                    QuestionId = Guid.NewGuid(),
                    SelectedAnswer = "Answer 1"
                },
                new AnswerDto
                {
                    QuestionId = Guid.NewGuid(),
                    SelectedAnswer = "Answer 2"
                }
            }
        };
    }
}
