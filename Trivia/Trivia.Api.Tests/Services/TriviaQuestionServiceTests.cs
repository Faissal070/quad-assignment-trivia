using Microsoft.Extensions.Logging;
using Moq;
using Trivia.Api.Client;
using Trivia.Api.Common.Enums;
using Trivia.Api.Models.External;
using Trivia.Api.Models.Queries;
using Trivia.Api.Services;
using Trivia.Api.Storage;

namespace Trivia.Api.Tests.Services;

public class TriviaQuestionServiceTests
{
    private readonly Mock<ICorrectAnswerStore> _correctAnswerStoreMock;
    private readonly Mock<ITriviaTokenService> _tokenServiceMock;
    private readonly Mock<ITriviaApiClient> _triviaApiClientMock;
    private readonly Mock<ILogger<TriviaAnswerService>> _loggerMock;

    private readonly TriviaQuestionService _triviaQuestionService;

    public TriviaQuestionServiceTests()
    {
        _correctAnswerStoreMock = new Mock<ICorrectAnswerStore>();
        _tokenServiceMock = new Mock<ITriviaTokenService>();
        _triviaApiClientMock = new Mock<ITriviaApiClient>();
        _loggerMock = new Mock<ILogger<TriviaAnswerService>>();

        _triviaQuestionService = new TriviaQuestionService(
            _correctAnswerStoreMock.Object,
            _tokenServiceMock.Object,
            _triviaApiClientMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetQuestionsAsync_WhenApiResponseIsNotSuccessOrNoResults_ShouldReturnFailure()
    {
        //Arrange
        var query = CreateQuestionQuery();
        var quizId = Guid.NewGuid();

        _tokenServiceMock.Setup(t => t.GetOrCreateTokenAsync(It.IsAny<Guid>()))
            .ReturnsAsync("valid-token");

        _triviaApiClientMock.Setup(c => c.FetchQuestionsAsync(It.IsAny<GetQuestionsQuery>(), It.IsAny<string>()))
            .ReturnsAsync(new TriviaApiQuestionResponse
            {
                ResponseCode = (int)TriviaApiResponseCodeEnum.TokenNotFound
            });

        //Act
        var result = await _triviaQuestionService.GetQuestionsAsync(query, quizId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Session expired. Please start new session.", result.Message);
    }

    [Fact]
    public async Task GetQuestionsAsync_WhenApiResponseIsTokenNotFound_ShouldReturnFailure()
    {
        //Arrange
        var query = CreateQuestionQuery();
        var quizId = Guid.NewGuid();

        _tokenServiceMock.Setup(t => t.GetOrCreateTokenAsync(It.IsAny<Guid>()))
           .ReturnsAsync("valid-token");

        _triviaApiClientMock.Setup(c => c.FetchQuestionsAsync(It.IsAny<GetQuestionsQuery>(), It.IsAny<string>()))
            .ReturnsAsync(new TriviaApiQuestionResponse
            {
                ResponseCode = (int)TriviaApiResponseCodeEnum.TokenNotFound
            });

        //Act
        var result = await _triviaQuestionService.GetQuestionsAsync(query, quizId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Session expired. Please start new session.", result.Message);

        _tokenServiceMock.Verify(t => t.ClearToken(It.IsAny<Guid>()), Times.Once);    
        _correctAnswerStoreMock.Verify(s => s.RemoveQuiz(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetQuestionsAsync_WhenApiResponseIsTokenEmpty_ShouldResetTokenAndTryAgain()
    {
        //Arrange
        var query = CreateQuestionQuery();
        var quizId = Guid.NewGuid();

        _tokenServiceMock.Setup(t => t.GetOrCreateTokenAsync(It.IsAny<Guid>()))
            .ReturnsAsync("Valid token");

        _tokenServiceMock.Setup(t => t.ResetTokenAsync(It.IsAny<Guid>(), It.IsAny<string>()));

        _triviaApiClientMock
            .SetupSequence(c => c.FetchQuestionsAsync(query, It.IsAny<string>()))
            .ReturnsAsync(new TriviaApiQuestionResponse
            {
                ResponseCode = (int)TriviaApiResponseCodeEnum.TokenEmpty
            })
            .ReturnsAsync(new TriviaApiQuestionResponse
            {
                ResponseCode = (int)TriviaApiResponseCodeEnum.Success,
                Results = new List<TriviaApiQuestion>()
            });

        //Act
        var result = await _triviaQuestionService.GetQuestionsAsync(query, quizId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Questions fetched successfully.", result.Message);

        _tokenServiceMock.Verify(t => t.ResetTokenAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetQuestionsAsync_WhenApiResponseIsSuccess_ShouldReturnQuestions()
    {
        //Arrange
        var query = CreateQuestionQuery();
        var quizId = Guid.NewGuid();

        _tokenServiceMock.Setup(t => t.GetOrCreateTokenAsync(It.IsAny<Guid>()))
            .ReturnsAsync("valid-token");

        _triviaApiClientMock.Setup(c => c.FetchQuestionsAsync(It.IsAny<GetQuestionsQuery>(), It.IsAny<string>()))
            .ReturnsAsync(new TriviaApiQuestionResponse
            {
                ResponseCode = (int)TriviaApiResponseCodeEnum.Success,
            });

        //Act
        var result = await _triviaQuestionService.GetQuestionsAsync(query, quizId);

        //Assert    
        Assert.True(result.IsSuccess);
        Assert.Equal("Questions fetched successfully.", result.Message);
    }

    [Fact]
    public async Task GetQuestionsAsync_WhenTokenNotFound_ThrowExceptionAndReturnFailure()
    {
        //Arrange
        var query = CreateQuestionQuery();
        var quizId = Guid.NewGuid();

        _tokenServiceMock.Setup(t => t.GetOrCreateTokenAsync(It.IsAny<Guid>()))
            .ReturnsAsync((string?)null);

        //Act
        var result = await _triviaQuestionService.GetQuestionsAsync(query, quizId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Could not start a session. Please try again later.", result.Message);
    }

    private GetQuestionsQuery CreateQuestionQuery()
    {
        return new GetQuestionsQuery
        {
            Amount = It.IsAny<int>(),
        };
    }
}
