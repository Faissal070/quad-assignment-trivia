using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Trivia.Api.Common.Results;
using Trivia.Api.Controllers;
using Trivia.Api.Models.Dtos;
using Trivia.Api.Models.Queries;
using Trivia.Api.Services;

namespace Trivia.Api.Tests.Controllers;

public class TriviaControllerTests
{
    private readonly Mock<ITriviaQuestionService> _triviaQuestionServiceMock;   
    private readonly Mock<ITriviaAnswerService> _triviaAnswerServiceMock;

    private readonly TriviaController _triviaController;

    public TriviaControllerTests()
    {
        _triviaQuestionServiceMock = new Mock<ITriviaQuestionService>();
        _triviaAnswerServiceMock = new Mock<ITriviaAnswerService>();

        _triviaController = new TriviaController(_triviaQuestionServiceMock.Object, _triviaAnswerServiceMock.Object);
    }

    [Fact]
    public async Task GetQuestions_WhenServiceReturnsFailure_ShouldReturnInternalServerError()
    {
        // Arrange
        var query = new Models.Queries.GetQuestionsQuery { Amount = 5 };
        var quizId = Guid.NewGuid();

        _triviaQuestionServiceMock.Setup(s => s.GetQuestionsAsync(query, quizId))
            .ReturnsAsync(Result<IReadOnlyList<QuestionDto>>.Failure("Service failure"));

        // Act
        var result = await _triviaController.GetQuestions(query, quizId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetQuestions_WhenServiceReturnsSuccess_ShouldReturnOkWithData()
    {
        // Arrange
        var query = new GetQuestionsQuery { Amount = 5 };
        var quizId = Guid.NewGuid();
        var questions = GetQuestions();

        _triviaQuestionServiceMock.Setup(s => s.GetQuestionsAsync(query, quizId))
            .ReturnsAsync(Result<IReadOnlyList<QuestionDto>>.Success(questions, "Success"));

        // Act
        var result = await _triviaController.GetQuestions(query, quizId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedQuestions = Assert.IsAssignableFrom<Result<IReadOnlyList<QuestionDto>>>(okResult.Value);
        Assert.Equal(questions.Count, returnedQuestions.Data!.Count);
    }

    [Fact]
    public async Task SubmitAnswers_WhenServiceReturnsFailure_ShouldReturnInternalServerError()
    {
        // Arrange
        var query = GetSubmitAnswersQuery();

        _triviaAnswerServiceMock.Setup(s => s.SubmitAnswersAsync(It.IsAny<SubmitAnswersQuery>()))
            .ReturnsAsync(Result<IReadOnlyList<AnswerResultDto>>.Failure("Service failure"));

        // Act
        var result = await _triviaController.SubmitAnswers(query);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }

    [Fact]
    public async Task SubmitAnswers_WhenServiceReturnsSuccess_ShouldReturnOkWithData()
    {
        // Arrange
        var query = GetSubmitAnswersQuery();
        var answerResults = GetAnswerResults();

        _triviaAnswerServiceMock.Setup(s => s.SubmitAnswersAsync(It.IsAny<SubmitAnswersQuery>()))
            .ReturnsAsync(Result<IReadOnlyList<AnswerResultDto>>.Success(answerResults, "Success"));
        
        // Act
        var result = await _triviaController.SubmitAnswers(query);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResults = Assert.IsAssignableFrom<Result<IReadOnlyList<AnswerResultDto>>>(okResult.Value);
        Assert.Equal(answerResults.Count, returnedResults.Data!.Count);
    }

    private List<QuestionDto> GetQuestions()
    {
        return new List<QuestionDto>
        {
            new() { Id = Guid.NewGuid(), Question = "Question 1", Choices = new List<string> { "A", "B", "C", "D" } },
            new() { Id = Guid.NewGuid(), Question = "Question 2", Choices = new List<string> { "A", "B", "C", "D" } }
        };
    }

    private SubmitAnswersQuery GetSubmitAnswersQuery()
    {
        return new SubmitAnswersQuery
        {
            QuizId = Guid.NewGuid(),
            Answers = new List<AnswerDto>
            {
                new() { QuestionId = Guid.NewGuid(), SelectedAnswer = "Answer 1" },
                new() { QuestionId = Guid.NewGuid(), SelectedAnswer = "Answer 2" }
            }
        };
    }

    private List<AnswerResultDto> GetAnswerResults()
    {
        return new List<AnswerResultDto>
        {
            new() { QuestionId = Guid.NewGuid(), IsCorrect = true },
            new() { QuestionId = Guid.NewGuid(), IsCorrect = false }
        };
    }
}
