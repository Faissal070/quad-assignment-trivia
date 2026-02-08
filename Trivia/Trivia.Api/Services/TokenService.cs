using Trivia.Api.Client;
using Trivia.Api.Common.Enums;
using Trivia.Api.Storage;

namespace Trivia.Api.Services;

public class TokenService : ITokenService
{
    private readonly ITriviaTokenStorage _tokenStorage; 
    private readonly ITriviaApiClient _triviaApiClient;

    public TokenService(ITriviaApiClient triviaApiClient, ITriviaTokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
        _triviaApiClient = triviaApiClient;
    }

    public async Task<string?> GetTokenAsync(string sessionId)
    {
        var token = _tokenStorage.GetToken(sessionId);

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

        _tokenStorage.SaveToken(sessionId, tokenResponse.Token);
        return tokenResponse.Token;
    }

    public async Task HandleTokenResponseAsync(string sessionId, string token, int responseCode)
    {
        switch (responseCode)
        {
            case (int)TriviaApiResponseCodeEnum.TokenNotFound:
                _tokenStorage.ClearToken(sessionId);
                break;

            case (int)TriviaApiResponseCodeEnum.TokenEmpty:
                await ResetTokenAsync(sessionId, token);
                break;

            default:
                break;
        }
    }

    private async Task ResetTokenAsync(string sessionId, string token)
    {
        var resetResponseCode = await _triviaApiClient.ResetTokenAsync(token);

        if (resetResponseCode.ResponseCode == (int)TriviaApiResponseCodeEnum.Success)
        {
            return;
        }

        _tokenStorage.ClearToken(sessionId);
    }
}