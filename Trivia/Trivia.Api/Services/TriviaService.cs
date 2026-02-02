using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using Trivia.Api.Configuration;
using Trivia.Api.Enums;
using Trivia.Api.Models.Dtos;
using Trivia.Api.Models.External;
using Trivia.Api.Models.Responses;
using Trivia.Api.Storage;

namespace Trivia.Api.Services
{
    public class TriviaService : ITriviaService
    {
        private readonly TriviaApiSettings _triviaApiSettings;
        private readonly ICorrectAnswerStore _correctAnswerStore;
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private const string ApiError = "Trivia API error";

        public TriviaService(IOptions<TriviaApiSettings> triviaApiSettings, ICorrectAnswerStore correctAnswerStore ,HttpClient httpClient)
        {
            _triviaApiSettings = triviaApiSettings.Value;
            _correctAnswerStore = correctAnswerStore;
            _httpClient = httpClient;
        }

        public async Task<TriviaQuestionsResult> GetQuestionsAsync(int amount)
        {
            var response = await _httpClient.GetAsync($"{_triviaApiSettings.BaseUrl}/api.php?amount={amount}");
            
            if(!response.IsSuccessStatusCode)
            {
                return new TriviaQuestionsResult
                {
                    StatusCode = response.StatusCode,
                    ErrorMessage = $"{ApiError}: {(int)response.StatusCode} {response.ReasonPhrase}"
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TriviaApiResponse>(json, _jsonOptions);

            if (result.Results == null)
            {
                return new TriviaQuestionsResult
                {
                    ResponseCodeDescription = (TriviaApiResponseCodeEnum)result.ResponseCode,
                    StatusCode = HttpStatusCode.BadGateway,
                    ErrorMessage = "Invalid response from Trivia API"
                };
            }

            var questions = MapQuestions(result!.Results);

            return new TriviaQuestionsResult
            {
                ResponseCodeDescription = (TriviaApiResponseCodeEnum)result.ResponseCode,
                Questions = questions
            };
        }

        private IEnumerable<TriviaQuestionsDto> MapQuestions(List<TriviaApiItem> triviaItems)
        {
            var filteredQuestions = new List<TriviaQuestionsDto>();

            foreach (var question in triviaItems)
            {
                var id = Guid.NewGuid();
                
                var questionDto = new TriviaQuestionsDto
                {
                    Type = question.Type,
                    Difficulty = question.Difficulty,
                    Category = question.Category,
                    Question = WebUtility.HtmlDecode(question.Question),
                    Incorrect_Answers = question.IncorrectAnswers.ToList()
                };

                filteredQuestions.Add(questionDto);
                _correctAnswerStore.AddCorrectAnswer(id, question.CorrectAnswer);
            }   

            return filteredQuestions;
        }
    }
}
