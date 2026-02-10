using Trivia.Api.Client;
using Trivia.Api.Common.Enums;
using Trivia.Api.Storage;

namespace Trivia.Api.Services;

public class TriviaTokenService : ITriviaTokenService
{
    private readonly ITokenStore _tokenStore; 
    private readonly ITriviaApiClient _triviaApiClient;

    public TriviaTokenService(ITriviaApiClient triviaApiClient, ITokenStore tokenStorage)
    {
        _tokenStore = tokenStorage;
        _triviaApiClient = triviaApiClient;
    }

    public async Task<string?> GetOrCreateTokenAsync(Guid quizId)
    {
        var token = _tokenStore.GetToken(quizId);

        if (!string.IsNullOrWhiteSpace(token))
        {
            return token;
        }

        var tokenResponse = await _triviaApiClient.FetchTokenAsync();

        if (tokenResponse.ResponseCode != (int)TriviaApiResponseCodeEnum.Success ||
            string.IsNullOrWhiteSpace(tokenResponse.Token))
        {
            return null;
        }

        _tokenStore.SaveToken(quizId, tokenResponse.Token);
        return tokenResponse.Token;
    }

    public async Task<bool> ResetTokenAsync(Guid quizId, string token)
    {
        var resetResponseCode = await _triviaApiClient.ResetTokenAsync(token);

        if (resetResponseCode.ResponseCode == (int)TriviaApiResponseCodeEnum.Success)
        {
            return true;
        }

        return false;
    }

    public void ClearToken(Guid quizId)
    {
        _tokenStore.RemoveQuizToken(quizId);
    }
}
