using Trivia.Api.Client;
using Trivia.Api.Common.Enums;
using Trivia.Api.Common.Mapper;
using Trivia.Api.Common.Results;
using Trivia.Api.Models.Dtos;
using Trivia.Api.Models.External;
using Trivia.Api.Models.Queries;
using Trivia.Api.Storage;

namespace Trivia.Api.Services;

public class TriviaService : ITriviaService
{
    private readonly ICorrectAnswerStore _correctAnswerStore;
    private readonly ITokenService _tokenService;
    private readonly ITriviaApiClient _triviaApiClient;
    private readonly ILogger _logger;   

    private const int MaxTokenRetryAttempts = 2;

    public TriviaService(ICorrectAnswerStore correctAnswerStore, ITokenService tokenService,
        ITriviaApiClient triviaApiClient, ILogger logger)
    {
        _correctAnswerStore = correctAnswerStore;
        _triviaApiClient = triviaApiClient;
        _tokenService = tokenService;   
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<QuestionDto>>> GetQuestionsAsync(GetQuestionsQuery query, string sessionId)
    {
        try
        {
            var questions = await FetchQuestionsWithTokenAsync(query, sessionId);
            var responseMessage = TriviaResponseMessageMapper.ToPublicMessage(questions.ResponseCode);

            if (questions.ResponseCode != (int)TriviaApiResponseCodeEnum.Success 
                && questions.ResponseCode != (int)TriviaApiResponseCodeEnum.NoResults )
            {
                return Result<IReadOnlyList<QuestionDto>>.Failure(responseMessage); 
            }

            var mappedAndStoredQuestions = questions.Results.Count == 0
                ? new List<QuestionDto>()
                : MapAndStoreQuestions(questions.Results);

            return Result<IReadOnlyList<QuestionDto>>
                .Success(mappedAndStoredQuestions, responseMessage); 
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Network error while fetching trivia questions");

            return Result<IReadOnlyList<QuestionDto>>.Failure(
                "Trivia service temporarily unavailable.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while fetching trivia questions");

            return Result<IReadOnlyList<QuestionDto>>.Failure(
                "The trivia service returned an unexpected response.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unexpected error while fetching trivia questions");

            return Result<IReadOnlyList<QuestionDto>>.Failure(
                "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Fetches trivia questions using a valid session token and retries once
    /// if the token is reported invalid by the Trivia API.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task<TriviaApiQuestionResponse> FetchQuestionsWithTokenAsync(GetQuestionsQuery query, string sessionId)
    {
        for(int i = 0; i < MaxTokenRetryAttempts; i++)
        {
            var token = await _tokenService.GetTokenAsync(sessionId);

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("Unable to retrieve Trivia API token."); 
            }

            var questions = await _triviaApiClient.FetchQuestionsAsync(query, token);
             
            if (questions.ResponseCode == (int)TriviaApiResponseCodeEnum.TokenNotFound
                || questions.ResponseCode == (int)TriviaApiResponseCodeEnum.TokenEmpty)
            {
                await _tokenService.HandleTokenResponseAsync(sessionId, token, questions.ResponseCode);
                continue; 
            }

            _logger.LogInformation("Successfully fetched trivia questions on attempt {Attempt}. " +
                "Response code: {ResponseCode}", i + 1, questions.ResponseCode);

            return questions;
        }

        throw new InvalidOperationException("Failed to fetch questions after multiple attempts due to token issues.");
    }

    private IReadOnlyList<QuestionDto> MapAndStoreQuestions(List<TriviaApiQuestion> apiQuestions)
    {
        var filteredQuestions = new List<QuestionDto>();

        foreach (var apiQuestion in apiQuestions)
        {
            var allAnswers = apiQuestion.IncorrectAnswers
                .Append(apiQuestion.CorrectAnswer)
                .ToList();

            var triviaQuestionDto = CreateTriviaQuestionsDto(apiQuestion, allAnswers);

            filteredQuestions.Add(triviaQuestionDto);
            _correctAnswerStore.AddCorrectAnswer(triviaQuestionDto.Id, apiQuestion.CorrectAnswer);
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
            Question = trivialItem.Question,
            Choices = allChoices
        };
    }
}