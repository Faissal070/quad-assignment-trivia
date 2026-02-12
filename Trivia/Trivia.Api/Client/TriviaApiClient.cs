using Microsoft.Extensions.Options;
using System.Text.Json;
using Trivia.Api.Configuration;
using Trivia.Api.Models.External;
using Trivia.Api.Models.Queries;
using Trivia.Api.Models.Result;

namespace Trivia.Api.Client;

public class TriviaApiClient : ITriviaApiClient
{
    private readonly TriviaApiSettings _triviaApiSettings;
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TriviaApiClient(IOptions<TriviaApiSettings> triviaApiSettings, HttpClient httpClient)
    {
        _triviaApiSettings = triviaApiSettings.Value;
        _httpClient = httpClient;
    }

    public async Task<TriviaApiQuestionResponse> FetchQuestionsAsync(GetQuestionsQuery query, string token)
    {
        var url = CreateQuestionsUrl(query, token);
        return await GetAndDeserializeAsync<TriviaApiQuestionResponse>(url);
    }

    public async Task<TriviaApiTokenResponse> FetchTokenAsync()
    {
        var url = $"{_triviaApiSettings.BaseUrl}/api_token.php?command=request";
        return await GetAndDeserializeAsync<TriviaApiTokenResponse>(url);
    }

    public async Task<TriviaApiTokenResponse> ResetTokenAsync(string token)
    {
        var url = $"{_triviaApiSettings.BaseUrl}/api_token.php?command=reset&token={token}";
        return await GetAndDeserializeAsync<TriviaApiTokenResponse>(url);
    }

    private async Task<T> GetAndDeserializeAsync<T>(string url)
    {
        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(content, _jsonOptions)
            ?? throw new InvalidOperationException(
                "Trivia API returned null or invalid JSON.");
    }

    private string CreateQuestionsUrl(GetQuestionsQuery query, string token)
    {
        var url = $"{_triviaApiSettings.BaseUrl}/api.php?amount={query.Amount}";

        if (query.Category.HasValue && query.Category.Value > 0)
        {
            url += $"&category={query.Category}";
        }

        if (!string.IsNullOrWhiteSpace(query.Difficulty))
        {
            url += $"&difficulty={query.Difficulty}";
        }

        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            url += $"&type={query.Type}";
        }

        url += $"&token={token}";

        return url;
    }
}
