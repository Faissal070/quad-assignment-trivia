using Trivia.Api.Client;
using Trivia.Api.Common.Enums;
using Trivia.Api.Common.Mapper;
using Trivia.Api.Common.Results;
using Trivia.Api.Models.Dtos;
using Trivia.Api.Models.External;
using Trivia.Api.Storage;

namespace Trivia.Api.Services;

public class TriviaService : ITriviaService
{
    private readonly ICorrectAnswerStore _correctAnswerStore;
    private readonly ITokenService _tokenService;
    private readonly ITriviaApiClient _triviaApiClient;

    private const int MaxTokenRetryAttempts = 2;

    public TriviaService(ICorrectAnswerStore correctAnswerStore, ITokenService tokenService,
        ITriviaApiClient triviaApiClient)
    {
        _correctAnswerStore = correctAnswerStore;
        _triviaApiClient = triviaApiClient;
        _tokenService = tokenService;   
    }

    public async Task<Result<IReadOnlyList<QuestionDto>>> GetQuestionsAsync(int amount)
    {
        try
        {
            var questions = await FetchQuestionsWithTokenAsync(amount);
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
            return Result<IReadOnlyList<QuestionDto>>.Failure($"Network error occurred: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return Result<IReadOnlyList<QuestionDto>>.Failure($"Error fetching questions: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<QuestionDto>>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Fetches trivia questions using a valid session token and retries once
    /// if the token is reported invalid by the Trivia API.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task<TriviaApiQuestionResponse> FetchQuestionsWithTokenAsync(int amount)
    {
        for(int i = 0; i < MaxTokenRetryAttempts; i++)
        {
            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                continue;
            }

            var questions = await _triviaApiClient.FetchQuestionsAsync(amount, token);
             
            if (questions.ResponseCode == (int)TriviaApiResponseCodeEnum.TokenNotFound
                || questions.ResponseCode == (int)TriviaApiResponseCodeEnum.TokenEmpty)
            {
                await _tokenService.HandleTokenErrorAsync(token, questions.ResponseCode);
                continue; 
            }

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