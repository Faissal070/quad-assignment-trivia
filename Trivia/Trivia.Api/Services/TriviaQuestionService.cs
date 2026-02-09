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
    private readonly ITokenService _tokenService;
    private readonly ITriviaApiClient _triviaApiClient;
    private readonly ILogger<TriviaAnswerService> _logger;

    private const int MaxTokenRetryAttempts = 2;

    public TriviaQuestionService(
        ICorrectAnswerStore correctAnswerStore,
        ITokenService tokenService,
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

            var responseMessage = TriviaResponseMessageMapper.ToPublicMessage(questions.ResponseCode);

            if (questions.ResponseCode != (int)TriviaApiResponseCodeEnum.Success
                && questions.ResponseCode != (int)TriviaApiResponseCodeEnum.NoResults)
            {
                return Result<IReadOnlyList<QuestionDto>>.Failure(responseMessage);
            }

            var mappedAndStoredQuestions = questions.Results.Count == 0
                ? new List<QuestionDto>()
                : MapAndStoreQuestions(questions.Results, quizId);

            return Result<IReadOnlyList<QuestionDto>>
                .Success(mappedAndStoredQuestions, responseMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching trivia questions");

            return Result<IReadOnlyList<QuestionDto>>.Failure(
                "Something went wrong. Please try again.");
        }
    }

    /// <summary>
    /// Fetches trivia questions using a valid session token and retries once
    /// if the token is reported invalid by the Trivia API.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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

            if (questions.ResponseCode == (int)TriviaApiResponseCodeEnum.TokenNotFound
                || questions.ResponseCode == (int)TriviaApiResponseCodeEnum.TokenEmpty)
            {
                await HandleInvalidTokenAsync(quizId, token, questions.ResponseCode);
                continue;
            }

            return questions;
        }

        throw new InvalidOperationException("Failed to fetch questions after multiple attempts due to token issues.");
    }

    private async Task HandleInvalidTokenAsync(Guid quizId, string token, int responseCode)
    {
        switch (responseCode)
        {
            case (int)TriviaApiResponseCodeEnum.TokenNotFound:
                _tokenService.ClearToken(quizId);
                _correctAnswerStore.RemoveQuiz(quizId);
                break;

            case (int)TriviaApiResponseCodeEnum.TokenEmpty:
                await _tokenService.ResetTokenAsync(quizId, token);
                break;

            default:
                _logger.LogWarning("Unhandled token response code {Code} for quiz {QuizId}.", responseCode, quizId);
                break;
        }
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
            Question = WebUtility.HtmlDecode(trivialItem.Question),
            Choices = allChoices
        };
    }
}
