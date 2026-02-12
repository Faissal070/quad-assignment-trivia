using System.Net;
using Trivia.Api.Client;
using Trivia.Api.Common.Enums;
using Trivia.Api.Common.Mapper;
using Trivia.Api.Common.Results;
using Trivia.Api.Models.Dtos;
using Trivia.Api.Models.External;
using Trivia.Api.Models.Queries;
using Trivia.Api.Storage;

namespace Trivia.Api.Services;

public class TriviaQuestionService : ITriviaQuestionService
{
    private readonly ICorrectAnswerStore _correctAnswerStore;
    private readonly ITriviaTokenService _tokenService;
    private readonly ITriviaApiClient _triviaApiClient;
    private readonly ILogger<TriviaAnswerService> _logger;

    private const int MaxTokenRetryAttempts = 2;

    public TriviaQuestionService(
        ICorrectAnswerStore correctAnswerStore,
        ITriviaTokenService tokenService,
        ITriviaApiClient triviaApiClient,
        ILogger<TriviaAnswerService> logger)
    {
        _correctAnswerStore = correctAnswerStore;
        _tokenService = tokenService;
        _triviaApiClient = triviaApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves and processes trivia questions for a quiz using a valid token.
    /// Resultmodels is used for clear external notifications and logs for technical issues.
    /// </summary>
    public async Task<Result<IReadOnlyList<QuestionDto>>> GetQuestionsAsync(GetQuestionsQuery query, Guid quizId)
    {
        try
        {
            var questions = await FetchQuestionsWithTokenAsync(query, quizId);
            var responseStatus = (TriviaApiResponseCodeEnum)questions.ResponseCode;
            var message = TriviaResponseMessageMapper.ToPublicMessage(questions.ResponseCode);

            if (responseStatus != TriviaApiResponseCodeEnum.Success &&
                responseStatus != TriviaApiResponseCodeEnum.NoResults)
            {
                return Result<IReadOnlyList<QuestionDto>>.Failure(message);
            }

            var result = questions.Results.Count == 0
                ? new List<QuestionDto>()
                : MapAndStoreQuestions(questions.Results, quizId);

            return Result<IReadOnlyList<QuestionDto>>.Success(result, message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to retrieve or use session token for quiz {QuizId}.", quizId);

            return Result<IReadOnlyList<QuestionDto>>.Failure("Could not start a session. Please try again later.");
        }
    }

    /// <summary>
    /// Starts a new session if the token is invalid or expired, removing the current quiz.
    /// </summary>
    private async Task<TriviaApiQuestionResponse> FetchQuestionsWithTokenAsync(GetQuestionsQuery query, Guid quizId)
    {
        for (var attempt = 0; attempt < MaxTokenRetryAttempts; attempt++)
        {
            var token = await _tokenService.GetOrCreateTokenAsync(quizId);
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("Unable to retrieve Trivia API token.");
            }

            var questions = await _triviaApiClient.FetchQuestionsAsync(query, token);

            if(questions.ResponseCode == (int)TriviaApiResponseCodeEnum.TokenEmpty)
            {
                await _tokenService.ResetTokenAsync(quizId, token);
                continue;
            }

            if (questions.ResponseCode == (int)TriviaApiResponseCodeEnum.TokenNotFound)
            {
                _tokenService.ClearToken(quizId);
                _correctAnswerStore.RemoveQuiz(quizId);
            }

            return questions;
        }

        throw new InvalidOperationException("Failed to fetch questions after multiple attempts due to token issues.");
    }


    private IReadOnlyList<QuestionDto> MapAndStoreQuestions(List<TriviaApiQuestion> apiQuestions, Guid quizId)
    {
        var filteredQuestions = new List<QuestionDto>();

        foreach (var apiQuestion in apiQuestions)
        {
            var allAnswers = apiQuestion.IncorrectAnswers
                .Append(apiQuestion.CorrectAnswer)
                .ToList();

            var triviaQuestionDto = CreateTriviaQuestionsDto(apiQuestion, allAnswers);

            filteredQuestions.Add(triviaQuestionDto);
            _correctAnswerStore.AddCorrectAnswer(quizId, triviaQuestionDto.Id, apiQuestion.CorrectAnswer);
        }

        return filteredQuestions;
    }

    private static QuestionDto CreateTriviaQuestionsDto(TriviaApiQuestion trivialItem, List<string> allChoices)
    {
        allChoices.Shuffle();
        return new QuestionDto
        {
            Id = Guid.NewGuid(),
            Category = trivialItem.Category,
            Type = trivialItem.Type,
            Difficulty = trivialItem.Difficulty,
            Question = NormalizeText(trivialItem.Question),
            Choices = allChoices
        };
    }

    private static string NormalizeText(string value)
    {
        var previous = value;
        var current = WebUtility.HtmlDecode(previous);

        while (current != previous)
        {
            previous = current;
            current = WebUtility.HtmlDecode(previous);
        }

        return current;
    }
}
